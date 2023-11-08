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

    public void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }
        ResetToNormal();
    }

    public void ResetToNormal()
    {
        holeCompleteText.SetActive(false);
        Time.timeScale = 1f;
        if(GameObject.FindWithTag("Player") == null)
        {
            currentStrokesText.gameObject.SetActive(false);
        } else
        {
            currentStrokesText.gameObject.SetActive(true);
        }
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
}
