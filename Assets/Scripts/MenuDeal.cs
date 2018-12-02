using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MenuDeal : MonoBehaviour
{
    public GameObject dealPanel;
    private bool isPaused = false;
 
    void Start () 
    {
    
    }

    private void Update()
    {
        if (Input.GetKeyDown("e")) {
            TogglePauseMenu();
        }
    }
    
    public void TogglePauseMenu()
    {
        if (dealPanel.activeSelf)
        {
            dealPanel.SetActive(false);
            Time.timeScale = 1.0f;
            isPaused = false;
        }
        else
        {
            dealPanel.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
        }
    }
}
