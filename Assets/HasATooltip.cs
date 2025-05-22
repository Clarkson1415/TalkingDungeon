using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HasATooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject toolTip;
    [SerializeField] protected TMP_Text tmp;

    private void Awake()
    {
        this.toolTip.SetActive(false);
    }

    public void ChangeToolTipText(string text)
    {
        tmp.text = text;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        toolTip.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.SetActive(false);
    }
}
