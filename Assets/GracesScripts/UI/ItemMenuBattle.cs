using System.Collections;
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

    public void SlideIn()
    {
        anim.SetTrigger("SlideIn");
    }

    public void CloseItemMenu()
    {
        anim.SetTrigger("SlideOut");
        StartCoroutine(DisableAfterSlideAway());
    }

    private IEnumerator DisableAfterSlideAway()
    {
        AnimatorStateInfo stateInfo = this.anim.GetCurrentAnimatorStateInfo(0); // 0 = Base Layer
        while (stateInfo.IsName("ItemMenuSlideOut"))
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        this.gameObject.SetActive(false);
    }
}
