using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static Transform player;
    public static int level = 1;
    static int maxLevels = 7;

    private void Awake()
    {
        
        //maxLevels = SceneManager.sceneCount - 2;
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
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene("Level (" + level + ")", LoadSceneMode.Single);
    }

    public void StartLevelLocal()
    {
        StartLevel();
    }

    public static void Die()
    {
        StartLevel();
        DeathInfo.lastTimeDied = true;
    }

    public static void NextLevel()
    {
        level++;
        if(level > maxLevels)
        {
            level = 1;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            //SceneManager.LoadScene("Credits", LoadSceneMode.Single);
            ToMainMenu();
        } else StartLevel();
    }

    public void NextLevelLocal()
    {
        NextLevel();
    }

    public static void ToMainMenu()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        Destroy(GameObject.Find("music"));
    }

    public void ToMainMenuLocal()
    {
        ToMainMenu();
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
        //if (Input.GetKeyDown(KeyCode.T)) NextLevel();
    }

    public void EndApp()
    {
        Application.Quit();
    }
}
