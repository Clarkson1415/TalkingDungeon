using Assets.GracesScripts.UI;
using UnityEngine;

public class PauseMenu : MenuWithButtons
{
    [SerializeField] GameObject menu;
    private GameObject currentHighlighted;
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

    private void Update()
    {
        var highlightedMenuItem = this.UIEventSystem.currentSelectedGameObject;

        if (highlightedMenuItem != currentHighlighted && currentHighlighted != null)
        {
            if (highlightedMenuItem.TryGetComponent<MenuButton>(out var button))
            {
                button.PlayHighlightedSound();
                currentHighlighted = this.UIEventSystem.currentSelectedGameObject;
            }
        }
    }

    public override void Close()
    {
        this.pauseMenuAnimator.SetTrigger("Close");
    }
}
