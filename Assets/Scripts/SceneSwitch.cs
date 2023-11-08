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

    public void NextLevel()
    {
        string currentLevel = SceneManager.GetActiveScene().name;
        string numChar = currentLevel[currentLevel.Length - 1] + "";
        int num = int.Parse(numChar) + 1;
        if(num == 1)
        {
            numChar = currentLevel[currentLevel.Length - 2] + "" + currentLevel[currentLevel.Length - 1];
        }
        num = int.Parse(numChar) + 1;
        string nextLevel = SceneManager.GetActiveScene().name + "\b" + num;
        print(nextLevel);
    }
}
