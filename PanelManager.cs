public class PanelManager
{
    private List<BasePanel> ActivePanelList = new();
    private List<ReservePanelInfo> ReservePanelList = new();
    private bool showingReservePanel;

    public void AddPanelToActiveList(BasePanel panel)
    {
        if (ActivePanelList.Count > 0)
        {
            BasePanel lastPanel = ActivePanelList[ActivePanelList.Count - 1];
            if (lastPanel.Behavior == PanelBehavior.CloseOnNewPanel && lastPanel != panel)
            {
                lastPanel.ClosePanel();
            }
        }

        ActivePanelList.Add(panel);
    }

    public void AddPanelToReserve(ReservePanelInfo reservePanelInfo)
    {
        ReservePanelList.Add(reservePanelInfo);
        if (reservePanelInfo.ActiveNow)
        {
            ActivateReservedPanels();
        }
    }

    public void RemovePopup(BasePanel panel)
    {
        ActivePanelList.Remove(panel);

        var reservePanelToRemove = ReservePanelList.FirstOrDefault(rp => rp.Panel == panel);
        if (reservePanelToRemove != null)
        {
            showingReservePanel = false;
            ReservePanelList.Remove(reservePanelToRemove);
            ActivateReservedPanels();
        }
    }

    public void ActivateReservedPanels()
    {
        if (!IsPanelAvailable()) return;
        if (showingReservePanel) return;

        showingReservePanel = true;
        var panelInfo = ChoosePanelToActivate();
        if (panelInfo != null)
        {
            panelInfo.OnActivate?.Invoke();
            if (panelInfo.AutoOpen)
            {
                panelInfo.Panel.OpenPanel();
            }
        }
    }

    private ReservePanelInfo ChoosePanelToActivate()
    {
        return ReservePanelList.OrderByDescending(panelInfo => panelInfo.Panel.PriorityLevel).FirstOrDefault();
    }

    private bool IsPanelAvailable()
    {
        SceneType activeScene = GameManager.instance.sceneController.ActiveScene;
        return ReservePanelList.Any(panel => panel.Scene == activeScene);
    }
}

public class ReservePanelInfo
{
    public BasePanel Panel { get; }
    public Action OnActivate { get; }
    public SceneType Scene { get; }
    public bool ActiveNow { get; }
    public bool AutoOpen { get; }

    public ReservePanelInfo(BasePanel panel, Action onActivate, SceneType scene, bool activeNow, bool autoOpen)
    {
        Panel = panel;
        OnActivate = onActivate;
        Scene = scene;
        ActiveNow = activeNow;
        AutoOpen = autoOpen;
    }
}

public enum PanelPriority
{
    Unknown = 0,
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4,
    Urgent = 5,
}