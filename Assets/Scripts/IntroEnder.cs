using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroEnder : MonoBehaviour
{
    private void OnEnable()
    {
        SceneTransition.instance.LoadScene("level2");
    }
}
