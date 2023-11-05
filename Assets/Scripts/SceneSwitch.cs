using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    public string DefaultScene = "MainMenu";
    public void SwitchScene()
    {
        SceneManager.LoadScene(DefaultScene);
    }
    public void SwitchSceneWithName(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
