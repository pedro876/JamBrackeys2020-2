using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static Transform player;
    public static int level = 1;
    static int maxLevels = 1;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //maxLevels = SceneManager.sceneCount - 2;
    }

    public static float SmoothStep(float edge0, float edge1, float value)
    {
        value = Mathf.Clamp((value - edge0) / (edge1 - edge0), 0f, 1f);
        value = value * value * (3f - 2f * value);
        return value;
    }

    public static void StartLevel()
    {
        SceneManager.LoadScene("Level (" + level + ")", LoadSceneMode.Single);
    }

    public static void Die()
    {
        StartLevel();
    }

    public static void NextLevel()
    {
        level++;
        if(level > maxLevels)
        {
            level = 0;
            SceneManager.LoadScene("Credits", LoadSceneMode.Single);
        } else StartLevel();
    }

    public static void ToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void Update()
    {
        if (Input.GetButtonDown("RestartLevel"))
        {
            StartLevel();
        }
        if (Input.GetButtonDown("Cancel"))
        {
            ToMainMenu();
        }
    }
}
