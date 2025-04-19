using UnityEngine;

public class ItemMenuBattle : Menu
{
    private Animator anim;

    private void Awake()
    {
        anim = this.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void CloseItemMenu()
    {
        anim.SetTrigger("SlideOut");
    }
}
