using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] float height = 6f;
    [SerializeField] float maxTimeToOpen = 3f;
    float timeToChange;
    float timeDif;

    [SerializeField] [Range(0f, 1f)] float curveE0 = 0f;
    [SerializeField] [Range(0f, 1f)] float curveE1 = 1f;

    

    Vector3 defaultPos;
    Vector3 lastPos;
    Vector3 targetPos;

    private void Start()
    {
        defaultPos = transform.position;
        targetPos = defaultPos;
        lastPos = defaultPos;
        timeToChange = maxTimeToOpen;
        timeDif = maxTimeToOpen;
    }

    public void StartOpening()
    {
        targetPos = defaultPos - Vector3.up * height;
        lastPos = transform.position;
        timeToChange = 0f;
        timeDif = (Mathf.Abs(transform.position.y - targetPos.y) / height) * maxTimeToOpen;
    }

    public void StartClosing()
    {
        targetPos = defaultPos;
        lastPos = transform.position;
        timeToChange = 0f;
        timeDif = (Mathf.Abs(transform.position.y - targetPos.y) / height) * maxTimeToOpen;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.R)) StartOpening();
        //if (Input.GetKeyDown(KeyCode.F)) StartClosing();

        if(timeToChange < timeDif)
        {
            timeToChange += Time.deltaTime;
            float yLerp = GameManager.SmoothStep(curveE0, curveE1, timeToChange / timeDif);
            transform.position = Vector3.Lerp(lastPos, targetPos, yLerp);
        }
    }

}
