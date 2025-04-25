using UnityEngine;
using UnityEngine.UI;

public class InventorySlotImage : MonoBehaviour
{
    public void SetImage(Sprite image)
    {
        this.GetComponent<Image>().sprite = image;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
