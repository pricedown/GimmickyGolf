using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public GameObject holeCompleteText;
    public TextMeshProUGUI strokeText;
    public TextMeshProUGUI currentStrokesText;
    public TextMeshProUGUI allStrokesText;
    public GameObject pausedScreen;

    public void Start()
    {
        DontDestroyOnLoad(this);
        if (instance == null)
        {
            ResetToNormal();
            instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    public void ResetToNormal()
    {
        holeCompleteText.SetActive(false);
        pausedScreen.SetActive(false);
        currentStrokesText.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
    public void HoleComplete(GameObject player)
    {
        holeCompleteText.SetActive(true); // Show the text saying the hole has been completed
        int strokes = player.GetComponent<BallMovement>().strokeCount;
        if (PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "Best", strokes) >= strokes)
        {
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "Best", strokes);
        }
        strokeText.text = "Strokes: " + strokes + "\t\tBest: " + PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "Best", strokes);
        Time.timeScale = 0f; // Stop the scene
    }
    public void SetCurrentStrokes(int currentStrokes)
    {
        currentStrokesText.text = "Strokes: " + currentStrokes;
    }
    public void LoadPlayer()
    {
        currentStrokesText.gameObject.SetActive(true);
    }
    public void PauseUnpause()
    {
        if(Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
            pausedScreen.SetActive(false);
        }
        else if (!holeCompleteText.activeSelf)
        {
            Time.timeScale = 0f;
            string level = "W1L1";
            allStrokesText.text = "";
            while (SceneUtility.GetBuildIndexByScenePath(level) != -1)
            {
                if (PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "Best", 0) != 0)
                {
                    allStrokesText.text += PlayerPrefs.GetInt(level + "Best", 0) + "\n";
                }
                else
                {
                    allStrokesText.text += "X\n";
                }
                level = GetNextLevel(level);
            }
            pausedScreen.SetActive(true);
        }
    }
    public string GetNextLevel(string level)
    {
        string currentLevel = level;
        string numChar = currentLevel[currentLevel.Length - 1] + "";
        string partString = currentLevel.Substring(0, currentLevel.Length - 1);
        int num = int.Parse(numChar) + 1;
        if (num == 1)
        {
            numChar = currentLevel[currentLevel.Length - 2] + "" + currentLevel[currentLevel.Length - 1];
            partString = currentLevel.Substring(0, currentLevel.Length - 2);
        }
        num = int.Parse(numChar) + 1;
        string nextLevel = partString + num;
        if (SceneUtility.GetBuildIndexByScenePath(nextLevel) != -1)
        {
            return nextLevel;
        }
        else
        {
            return "0";
        }
    }
}
