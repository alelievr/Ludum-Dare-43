// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using System.Linq;

// public class MenuDeal : MonoBehaviour
// {
//     public GameObject dealPanel;
//     private bool isPaused = false;
//     public GuiButtons guiButtons;
 
//     void Start () 
//     {
//         //  guiButtons = GetComponent<GuiButtons>();
//         // dealPanel.SetActive(false);
    
//     }

//     private void Update()
//     {
//         if (Input.GetKeyDown("e")) {
//             TogglePauseMenu();
//         }
//     }
    
//     public void TogglePauseMenu()
//     {
//         Debug.Log(guiButtons);
//         if (dealPanel.activeSelf)
//         {
//             dealPanel.SetActive(false);
//             Time.timeScale = 1.0f;
//             isPaused = false;
//         }
//         else
//         {
//             dealPanel.SetActive(true);
//             guiButtons.chargeSacrifice = 1;
//             Time.timeScale = 0f;
//             isPaused = true;
//         }
//     }
// }
