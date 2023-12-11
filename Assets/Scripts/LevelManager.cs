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
    public GameObject allStrokes;
    public GameObject pausedScreen;
    public GameObject cloud;

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
        allStrokes.SetActive(false);
        Time.timeScale = 1f;
        StopAllCoroutines();
    }
    public void HoleComplete(GameObject player)
    {
        holeCompleteText.SetActive(true); // Show the text saying the hole has been completed
        allStrokes.SetActive(true);
        int strokes = player.GetComponent<BallMovement>().strokeCount;
        if (PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "Best", strokes) >= strokes || PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "Best", strokes) == 0)
        {
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "Best", strokes);
        }
        strokeText.text = "Strokes: " + strokes + "\t\tBest: " + PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "Best", strokes);
        SetScoreSheet();
        Time.timeScale = 0f; // Stop the scene
    }
    public void SetCurrentStrokes(int currentStrokes)
    {
        currentStrokesText.text = "Strokes: " + currentStrokes;
    }
    public void LoadPlayer()
    {
        currentStrokesText.gameObject.SetActive(true);
        StartCloud();
    }
    public void PauseUnpause()
    {
        if(Time.timeScale == 0f && !holeCompleteText.activeSelf)
        {
            Time.timeScale = 1f;
            pausedScreen.SetActive(false);
            allStrokes.SetActive(false);
        }
        else if (!holeCompleteText.activeSelf)
        {
            Time.timeScale = 0f;
            SetScoreSheet();
            allStrokes.SetActive(true);
            pausedScreen.SetActive(true);
        }
    }
    public void SetScoreSheet()
    {
        string level = "W1L1";
        allStrokesText.text = "";
        while (SceneUtility.GetBuildIndexByScenePath(level) != -1)
        {
            if (PlayerPrefs.GetInt(level + "Best", 0) != 0)
            {
                allStrokesText.text += level + ": " + PlayerPrefs.GetInt(level + "Best", 0) + "\n";
            }
            else
            {
                allStrokesText.text += level + ": " + "X\n";
            }
            level = GetNextLevel(level);
        }
    }
    public void ResetScores()
    {
        string level = "W1L1";
        while (SceneUtility.GetBuildIndexByScenePath(level) != -1)
        {
            if (PlayerPrefs.GetInt(level + "Best", 0) != 0)
            {
                PlayerPrefs.SetInt(level + "Best", 0);
            }
            level = GetNextLevel(level);
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
    public void StartCloud()
    {
        for (int i = 0; i < 3; i++)
        {
            Vector3 temp = new Vector3(Random.Range(-20f, 20f), Random.Range(-10f, 10f), 0);
            Instantiate(cloud, temp, Quaternion.identity);
        }
        StartCoroutine("CloudSummon", 3f);
    }
    IEnumerator CloudSummon(float time)
    {
        while (true)
        {
            Vector3 temp = new Vector3(-25f, Random.Range(-10f, 10f), 0);
            Instantiate(cloud, temp, Quaternion.identity);
            yield return new WaitForSeconds(time);
            time = Random.Range(5f, 7.5f);
        }
    }
    
}
