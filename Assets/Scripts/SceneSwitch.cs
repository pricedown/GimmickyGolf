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
        LevelManager.instance.ResetToNormal();
    }
    public void SwitchSceneWithName(string scene)
    {
        SceneManager.LoadScene(scene);
        LevelManager.instance.ResetToNormal();
    }

    public void NextLevel()
    {
        string currentLevel = SceneManager.GetActiveScene().name;
        string numChar = currentLevel[currentLevel.Length - 1] + "";
        string partString = currentLevel.Substring(0, currentLevel.Length - 1);
        int num = int.Parse(numChar) + 1;
        if(num == 1)
        {
            numChar = currentLevel[currentLevel.Length - 2] + "" + currentLevel[currentLevel.Length - 1];
            partString = currentLevel.Substring(0, currentLevel.Length - 2);
        }
        num = int.Parse(numChar) + 1;
        string nextLevel = partString + num;
        if(SceneUtility.GetBuildIndexByScenePath(nextLevel) != -1)
        {
            SceneManager.LoadScene(nextLevel);
            LevelManager.instance.ResetToNormal();
        } 
        else
        {
            SwitchScene();
        }
    }
}
