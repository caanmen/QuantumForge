using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class D3QueuesPanelUI : MonoBehaviour
{
    public GameObject factoryRoot;
    public Button backButton;
    public TMP_Dropdown queueDropdown;
    public TMP_Dropdown jobDropdown;
    public TMP_Text detailText;
    public TMP_Text noticeText;
    public Button cancelSelectedButton;

    private const float ConfirmationSeconds = 5f;
    private string _pendingJobId = "";
    private float _pendingUntil;

    private void Awake()
    {
        if (backButton != null) backButton.onClick.AddListener(Close);
        if (cancelSelectedButton != null)
            cancelSelectedButton.onClick.AddListener(CancelSelected);
        if (queueDropdown != null)
            queueDropdown.onValueChanged.AddListener(_ => Refresh());
        if (jobDropdown != null)
            jobDropdown.onValueChanged.AddListener(_ => RefreshDetail());
        ConfigureQueues();
    }

    public void Open()
    {
        if (factoryRoot != null) factoryRoot.SetActive(false);
        gameObject.SetActive(true);
        Refresh();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        if (factoryRoot != null) factoryRoot.SetActive(true);
    }

    public void Refresh()
    {
        if (jobDropdown == null) return;
        int selected = jobDropdown.value;
        jobDropdown.ClearOptions();
        var labels = new List<string>();
        D3QueueState queue = GetSelectedQueue();
        if (queue != null && queue.jobs != null)
            for (int i = 0; i < queue.jobs.Count; i++)
                labels.Add(BuildShortLabel(queue.jobs[i], i));
        if (labels.Count == 0) labels.Add("Sin trabajos");
        jobDropdown.AddOptions(labels);
        jobDropdown.value = Math.Max(0, Math.Min(selected, labels.Count - 1));
        RefreshDetail();
    }

    private void RefreshDetail()
    {
        D3JobState job = GetSelectedJob();
        if (detailText != null)
            detailText.text = job == null
                ? "Esta cola está vacía."
                : BuildDetail(job);
        if (cancelSelectedButton != null)
            cancelSelectedButton.interactable = job != null;
    }

    private void CancelSelected()
    {
        D3JobState job = GetSelectedJob();
        if (job == null || GameState.I == null) return;
        if (job.started &&
            (_pendingJobId != job.jobId || Time.unscaledTime > _pendingUntil))
        {
            _pendingJobId = job.jobId;
            _pendingUntil = Time.unscaledTime + ConfirmationSeconds;
            Notice("Trabajo activo: no devolverá recursos. Pulsa otra vez en 5 s para confirmar.");
            return;
        }

        bool active = job.started;
        string reason;
        if (D3JobQueueSystem.TryCancelJob(
                GameState.I, GetSelectedQueueId(), job.jobId, out reason))
        {
            Notice(active
                ? "Trabajo activo cancelado sin devolución."
                : "Trabajo pendiente cancelado con devolución completa.");
            _pendingJobId = "";
            Refresh();
        }
        else Notice(reason);
    }

    private void ConfigureQueues()
    {
        if (queueDropdown == null) return;
        queueDropdown.ClearOptions();
        queueDropdown.AddOptions(new List<string>
            { "Producción", "Ensamblaje", "Investigación", "Instalaciones" });
    }

    private string GetSelectedQueueId()
    {
        int index = queueDropdown == null ? 0 : queueDropdown.value;
        return Dimension3Catalog.QueueIds[Math.Max(0,
            Math.Min(index, Dimension3Catalog.QueueIds.Length - 1))];
    }

    private D3QueueState GetSelectedQueue()
    {
        return GameState.I == null || GameState.I.dimension3 == null
            ? null
            : D3JobQueueSystem.GetQueue(GameState.I.dimension3, GetSelectedQueueId());
    }

    private D3JobState GetSelectedJob()
    {
        D3QueueState queue = GetSelectedQueue();
        int index = jobDropdown == null ? 0 : jobDropdown.value;
        return queue == null || queue.jobs == null || index < 0 || index >= queue.jobs.Count
            ? null : queue.jobs[index];
    }

    private static string BuildShortLabel(D3JobState job, int index)
    {
        if (job == null) return "Trabajo inválido";
        return (job.started ? "ACTIVO" : "PENDIENTE") + " " + (index + 1) +
            " — " + Describe(job);
    }

    private static string BuildDetail(D3JobState job)
    {
        double rate = job.usesDynamicBankSpeed && GameState.I != null
            ? D3PowerSystem.GetDynamicWorkRate(GameState.I.dimension3) : 1.0;
        return Describe(job) + "\nEstado: " + (job.started ? "Activo" : "Pendiente") +
            " | Trabajo restante: " + Math.Ceiling(job.remainingSeconds) +
            " | Tiempo estimado actual: " + Math.Ceiling(job.remainingSeconds / rate) + " s" +
            "\nPagado: " + Math.Floor(job.paidLE) + " LE + " +
            Math.Floor(job.paidTraces) + " T";
    }

    private static string Describe(D3JobState job)
    {
        if (job.jobType == Dimension3Catalog.JobPartProduction)
            return job.targetId + " V" + job.version + " ×" + job.quantity;
        if (job.jobType == Dimension3Catalog.JobAssembly)
            return "MK" + job.mk + " " + (job.resultTraitId ?? "Normal") + " ×" + job.quantity;
        if (job.jobType == Dimension3Catalog.JobResearch)
            return "Investigación " + job.targetId;
        return "Instalación " + job.targetId + " N" + job.version;
    }

    private void Notice(string value)
    {
        if (noticeText != null) noticeText.text = value ?? "";
    }
}
