using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static Transform player;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public static float SmoothStep(float edge0, float edge1, float value)
    {
        value = Mathf.Clamp((value - edge0) / (edge1 - edge0), 0f, 1f);
        value = value * value * (3f - 2f * value);
        return value;
    }
}
