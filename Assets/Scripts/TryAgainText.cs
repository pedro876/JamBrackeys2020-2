using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TryAgainText : MonoBehaviour
{
    TextMeshProUGUI text;
    [SerializeField] float time = 3f;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        if (DeathInfo.lastTimeDied)
        {
            Debug.Log(DeathInfo.lastTimeDied);
            DeathInfo.lastTimeDied = false;
            text.enabled = true;
            StartCoroutine("Timer");
        }
    }

    private void Update()
    {
        Debug.Log(DeathInfo.lastTimeDied);
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(time);
        text.enabled = false;
    }
}
