using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class D3ResearchPanelUI : MonoBehaviour
{
    public GameObject factoryRoot;
    public TMP_Dropdown partDropdown, versionDropdown, teamMkDropdown, teamTraitDropdown;
    public Button addTeamButton, removeTeamButton, queueResearchButton, cancelResearchButton, backButton;
    public TMP_Text teamText, researchText, queueText, noticeText;
    private readonly List<D3ReservedAutomatonState> team = new List<D3ReservedAutomatonState>();
    private readonly SafeDropdownOptionMap<string> partOptions =
        new SafeDropdownOptionMap<string>(StringComparer.Ordinal);
    private readonly SafeDropdownOptionMap<int> versionOptions =
        new SafeDropdownOptionMap<int>();
    private readonly SafeDropdownOptionMap<int> teamMkOptions =
        new SafeDropdownOptionMap<int>();
    private readonly SafeDropdownOptionMap<string> teamTraitOptions =
        new SafeDropdownOptionMap<string>(StringComparer.Ordinal);
    private float refreshTimer;

    private void Awake()
    {
        partOptions.Rebuild(partDropdown, Dimension3Catalog.PartIds,
            PartName, Dimension3Catalog.PartChassis);
        versionOptions.Rebuild(versionDropdown, new[] { 4 }, value => "V" + value, 4);
        teamMkOptions.Rebuild(teamMkDropdown, new[] { 1 }, value => "MK" + value, 1);
        teamTraitOptions.Rebuild(teamTraitDropdown, Dimension3Catalog.TraitIds,
            TraitName, Dimension3Catalog.TraitNormal);
        Listen(addTeamButton, () => ChangeTeam(1));
        Listen(removeTeamButton, () => ChangeTeam(-1));
        Listen(queueResearchButton, QueueResearch);
        Listen(cancelResearchButton, CancelResearch);
        Listen(backButton, Close);
    }
    private void OnEnable() { Refresh(); }
    private void Update() { refreshTimer -= Time.unscaledDeltaTime; if (refreshTimer <= 0) { refreshTimer = .2f; Refresh(); } }
    public void Open() { if (factoryRoot != null) factoryRoot.SetActive(false); gameObject.SetActive(true); Refresh(); }
    public void Close() { gameObject.SetActive(false); if (factoryRoot != null) factoryRoot.SetActive(true); }

    private void ChangeTeam(long delta)
    {
        int mk = teamMkOptions.ResolveOrDefault(teamMkDropdown.value, 1);
        string trait = teamTraitOptions.ResolveOrDefault(
            teamTraitDropdown.value, Dimension3Catalog.TraitNormal);
        D3ReservedAutomatonState member = Find(mk, trait);
        long target = Math.Max(0, (member == null ? 0 : member.amount) + delta);
        long free = D3InventorySystem.GetAvailableAutomatonAmount(GameState.I.dimension3, mk, trait);
        if (delta > 0 && target > free) { Notice("No hay más autómatas libres de ese grupo."); return; }
        if (member == null && target > 0) { member = new D3ReservedAutomatonState { mk = mk, traitId = trait }; team.Add(member); }
        if (member != null) member.amount = target;
        if (member != null && member.amount == 0) team.Remove(member);
        Refresh();
    }
    private void QueueResearch()
    {
        string reason;
        string part = partOptions.ResolveOrDefault(
            partDropdown.value, Dimension3Catalog.PartChassis);
        int version = versionOptions.ResolveOrDefault(versionDropdown.value, 4);
        if (Dimension3System.TryQueueResearch(GameState.I, part, version, team, out reason))
        { team.Clear(); Notice("Investigación " + part + " V" + version + " encolada."); }
        else Notice(reason);
        Refresh();
    }
    private void CancelResearch()
    {
        D3QueueState queue = D3JobQueueSystem.GetQueue(GameState.I.dimension3, Dimension3Catalog.QueueResearch);
        if (queue == null || queue.jobs.Count == 0) { Notice("No hay investigación que cancelar."); return; }
        string reason;
        if (Dimension3System.TryCancelJob(GameState.I, Dimension3Catalog.QueueResearch,
                queue.jobs[queue.jobs.Count - 1].jobId, out reason))
            Notice("Investigación cancelada; investigadores liberados.");
        else Notice(reason);
        Refresh();
    }
    private void Refresh()
    {
        if (GameState.I == null || GameState.I.dimension3 == null) return;
        RefreshOptions();
        D3ResearchTeamModifiers m = D3ResearchSystem.CalculateTeamModifiers(team);
        if (teamText != null) { var b = new StringBuilder("EQUIPO\n"); for (int i=0;i<team.Count;i++) b.AppendLine("MK"+team[i].mk+" "+team[i].traitId+" ×"+team[i].amount); if(team.Count==0)b.AppendLine("Sin investigadores."); b.Append("Potencia: ").Append(m.requirementPower.ToString("0.##")); teamText.text=b.ToString(); }
        if (researchText != null)
        {
            string part = partOptions.ResolveOrDefault(
                partDropdown == null ? 0 : partDropdown.value,
                Dimension3Catalog.PartChassis);
            int version = versionOptions.ResolveOrDefault(
                versionDropdown == null ? 0 : versionDropdown.value, 4);
            D3ResearchDefinition definition =
                Dimension3Catalog.GetResearchDefinition(part, version);
            string requirement;
            D3ResearchSystem.ValidatePrerequisites(
                GameState.I, part, version, out requirement);
            researchText.text = definition == null ? "INVESTIGACIÓN NO DISPONIBLE" :
                "TARJETA · " + PartName(part) + " V" + version +
                "\nDesbloquea: producción " + PartName(part) + " V" + version +
                "\nEquipo reservado · potencia mínima " +
                definition.minimumPower.ToString("0.##") +
                "\nCosto base: " + definition.leCost.ToString("0") + " LE + " +
                definition.tracesCost.ToString("0") + " T" +
                " · Duración base: " + Math.Ceiling(definition.durationSeconds) + " s" +
                "\nRequisito inmediato: " +
                (string.IsNullOrEmpty(requirement) ? "cumplido" : requirement) +
                "\n\nDETALLES DE CADENA\nV4 → V5 de esta pieza → V6 de esta pieza.";
        }
        D3QueueState q=D3JobQueueSystem.GetQueue(GameState.I.dimension3,Dimension3Catalog.QueueResearch);
        if(queueText!=null)queueText.text=q==null||q.jobs.Count==0?"COLA\nSin investigaciones.":"COLA\n"+q.jobs[0].targetId+" — "+Math.Ceiling(q.jobs[0].remainingSeconds)+" s";
        if(queueResearchButton!=null)queueResearchButton.interactable=team.Count>0;
        if(cancelResearchButton!=null)cancelResearchButton.interactable=q!=null&&q.jobs.Count>0;
    }
    private void RefreshOptions()
    {
        string selectedPart = partOptions.ResolveOrDefault(
            partDropdown == null ? 0 : partDropdown.value,
            Dimension3Catalog.PartChassis);
        partOptions.Rebuild(partDropdown, Dimension3Catalog.PartIds,
            PartName, selectedPart);
        int selectedVersion = versionOptions.ResolveOrDefault(
            versionDropdown == null ? 0 : versionDropdown.value, 4);
        var versions = new List<int>();
        for (int version = 4; version <= 6; version++)
        {
            bool relevant = false;
            for (int i = 0; i < Dimension3Catalog.PartIds.Length; i++)
                if (D3ResearchSystem.ValidatePrerequisites(GameState.I,
                        Dimension3Catalog.PartIds[i], version, out _) ||
                    D3ResearchSystem.IsCompleted(GameState.I.dimension3,
                        Dimension3Catalog.PartIds[i], version)) relevant = true;
            if (relevant) versions.Add(version);
        }
        if (versions.Count == 0) versions.Add(4);
        versionOptions.Rebuild(versionDropdown, versions,
            value => "V" + value, selectedVersion);
        int selectedMk = teamMkOptions.ResolveOrDefault(
            teamMkDropdown == null ? 0 : teamMkDropdown.value, 1);
        teamMkOptions.Rebuild(teamMkDropdown,
            D3ProgressivePresentationRules.GetUnlockedAssemblyMks(GameState.I),
            value => "MK" + value, selectedMk);
    }
    private D3ReservedAutomatonState Find(int mk,string trait){for(int i=0;i<team.Count;i++)if(team[i].mk==mk&&team[i].traitId==trait)return team[i];return null;}
    private void Notice(string s){if(noticeText!=null)noticeText.text=s;}
    private static void Options(TMP_Dropdown d,string[] a){if(d==null)return;d.ClearOptions();d.AddOptions(new List<string>(a));}
    private static void Listen(Button b,UnityEngine.Events.UnityAction a){if(b!=null)b.onClick.AddListener(a);}
    private static string PartName(string id)
    {
        if (id == Dimension3Catalog.PartChassis) return "Chasis";
        if (id == Dimension3Catalog.PartMotor) return "Sistema Motriz";
        if (id == Dimension3Catalog.PartTool) return "Herramienta";
        if (id == Dimension3Catalog.PartControl) return "Módulo de Control";
        return "Regulador";
    }
    private static string TraitName(string id)
    {
        if (id == Dimension3Catalog.TraitFast) return "Rápido";
        if (id == Dimension3Catalog.TraitEfficient) return "Eficiente";
        if (id == Dimension3Catalog.TraitCoordinator) return "Coordinador";
        return "Normal";
    }
}
