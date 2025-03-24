using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject menu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartPauseMenu(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            menu.gameObject.SetActive(!menu.gameObject.activeSelf); 
        }
        else if (context.performed)
        {

        }
        else if (context.canceled)
        {

        }
    }
}
