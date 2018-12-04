using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class IntroEnder : SceneTransition
{
    private void OnEnable()
    {
        base.LoadScene("level2");
    }
}
