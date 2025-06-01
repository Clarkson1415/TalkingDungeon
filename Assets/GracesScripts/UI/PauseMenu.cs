using Assets.GracesScripts.UI;
using UnityEngine;

public class PauseMenu : MenuWithButtons
{
    [SerializeField] GameObject menu;
    private Animator pauseMenuAnimator;

    // Start is called before the first frame update
    void Awake()
    {
        pauseMenuAnimator = this.GetComponent<Animator>();
    }

    public void StartPauseMenu()
    {
        this.pauseMenuAnimator.SetTrigger("Open");
    }

    public override void Close()
    {
        this.pauseMenuAnimator.SetTrigger("Close");
    }
}
