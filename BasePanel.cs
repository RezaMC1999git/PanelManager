ublic abstract class BasePanel : MonoBehaviour
{
    public string PanelId;
    public GameObject PanelObject;
    public PanelPriority PriorityLevel;
    public PanelBehavior Behavior;
    public Canvas PanelCanvas;
    public Animator PanelAnimator;
    public Button BlackBgButton;
    public UnityAction OnOpen, OnClose;


    public virtual void OpenPanel()
    {
        OnOpen?.Invoke();
        if(GameManager.instance != null) {
            GameManager.instance.panelManager.AddPanelToActiveList(this);
        }
        PanelObject.SetActive(true);
    }

    public virtual void ClosePanel()
    {
        StartCoroutine(ClosePanelIE());
    }
    private IEnumerator ClosePanelIE()
    {
        float animationTime = 0;
        if (PanelAnimator != null)
        {
            PanelAnimator.Play("close");
            animationTime = PanelAnimator.GetCurrentAnimatorStateInfo(0).length;
        }
        yield return new WaitForSeconds(animationTime);
        PanelObject.SetActive(false);
        if(GameManager.instance != null) {
            GameManager.instance.panelManager.RemovePopup(this);
        }
        OnClose?.Invoke();
    }
}

public enum PanelBehavior
{
    CloseOnNewPanel,
    StayOpen,
    CloseAfterTimeout,
    ManualHandle
}