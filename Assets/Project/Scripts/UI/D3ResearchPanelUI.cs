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
    private float refreshTimer;

    private void Awake()
    {
        Options(partDropdown, new[] { "Chasis", "Sistema Motriz", "Herramienta", "Módulo de Control", "Regulador" });
        Options(versionDropdown, new[] { "V4", "V5", "V6" });
        Options(teamMkDropdown, new[] { "MK1", "MK2", "MK3", "MK4", "MK5", "MK6" });
        Options(teamTraitDropdown, new[] { "Normal", "Rápido", "Eficiente", "Coordinador" });
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
        int mk = teamMkDropdown.value + 1;
        string trait = Dimension3Catalog.TraitIds[teamTraitDropdown.value];
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
        string reason; string part = Dimension3Catalog.PartIds[partDropdown.value]; int version = versionDropdown.value + 4;
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
        D3ResearchTeamModifiers m = D3ResearchSystem.CalculateTeamModifiers(team);
        if (teamText != null) { var b = new StringBuilder("EQUIPO\n"); for (int i=0;i<team.Count;i++) b.AppendLine("MK"+team[i].mk+" "+team[i].traitId+" ×"+team[i].amount); if(team.Count==0)b.AppendLine("Sin investigadores."); b.Append("Potencia: ").Append(m.requirementPower.ToString("0.##")); teamText.text=b.ToString(); }
        if (researchText != null) { var b=new StringBuilder("LÍNEAS\n"); for(int i=0;i<Dimension3Catalog.PartIds.Length;i++){string p=Dimension3Catalog.PartIds[i];int h=3;for(int v=4;v<=6;v++)if(D3ResearchSystem.IsCompleted(GameState.I.dimension3,p,v))h=v;b.AppendLine(p+": V"+h);} researchText.text=b.ToString(); }
        D3QueueState q=D3JobQueueSystem.GetQueue(GameState.I.dimension3,Dimension3Catalog.QueueResearch);
        if(queueText!=null)queueText.text=q==null||q.jobs.Count==0?"COLA\nSin investigaciones.":"COLA\n"+q.jobs[0].targetId+" — "+Math.Ceiling(q.jobs[0].remainingSeconds)+" s";
        if(queueResearchButton!=null)queueResearchButton.interactable=team.Count>0;
        if(cancelResearchButton!=null)cancelResearchButton.interactable=q!=null&&q.jobs.Count>0;
    }
    private D3ReservedAutomatonState Find(int mk,string trait){for(int i=0;i<team.Count;i++)if(team[i].mk==mk&&team[i].traitId==trait)return team[i];return null;}
    private void Notice(string s){if(noticeText!=null)noticeText.text=s;}
    private static void Options(TMP_Dropdown d,string[] a){if(d==null)return;d.ClearOptions();d.AddOptions(new List<string>(a));}
    private static void Listen(Button b,UnityEngine.Events.UnityAction a){if(b!=null)b.onClick.AddListener(a);}
}
