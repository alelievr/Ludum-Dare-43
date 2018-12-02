using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GuiButtons : MonoBehaviour
{
    [System.Serializable]
    public class ButtonList {
            public PlayerController.Key index;
            public GameObject guiButton;
            public GameObject guiLayer;
            public bool actif;
    }
    public List<ButtonList> buttonList = new List<ButtonList>();
    public  AudioSource destroyButtonSound;
    public GameObject dealPanel;
    public Animator     guiButtonsAnimator;
    private bool isPaused = false;


    public int chargeSacrifice = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("e")) {
            TogglePauseMenu();
        }
    }

    public void SetButtonStatus(PlayerController.Key p_button, bool p_status) {
        ButtonList button = buttonList.Where(c => c.index == p_button).FirstOrDefault();
        button.actif = p_status;
        button.guiLayer.SetActive(!p_status);
    }

    public bool GetButtonStatus(PlayerController.Key p_button) {
        ButtonList truc;
        return ((truc = buttonList.Where(c => c.index == p_button).FirstOrDefault()) != null ? truc.actif : false);
    }

    public void DestroyButton(string p_key)
    {
        if (chargeSacrifice > 0)
        {
            guiButtonsAnimator.SetBool("EnableButtons", true);
            chargeSacrifice--;
            ButtonList truc;
            truc = buttonList.Where(c => c.index.ToString() == p_key).FirstOrDefault();
            truc.actif = false;
            truc.guiLayer.SetActive(true);
            destroyButtonSound.Play();
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
            chargeSacrifice++;
            Time.timeScale = 0f;
            isPaused = true;
        }
    }
}
