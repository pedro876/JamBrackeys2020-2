using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour
{
    [SerializeField] Transform buttonTransform;
    [SerializeField] Material unPressedMat;
    [SerializeField] Material inRangeMat;
    [SerializeField] Material pushedMat;
    /*[SerializeField] Color normalEmission;
    [SerializeField] float normalIntensity;
    [SerializeField] Color inRangeEmission;
    [SerializeField] float inRangeIntensity;
    [SerializeField] Color pushedEmission;
    [SerializeField] float pushedIntensity;*/

    [SerializeField] Vector3 pushDir;
    [SerializeField] float depth = 0.1f;
    [SerializeField] float buttonRange = 0.5f;
    [SerializeField] float pushSpd = 0.2f;
    Vector3 targetPos;
    bool playerInRange = false;
    bool buttonInRange = false;
    bool pushed = false;

    Transform player;
    MeshRenderer renderer;

    [SerializeField] UnityEvent OnPushed;

    private void Start()
    {
        renderer = buttonTransform.GetComponent<MeshRenderer>();
        player = GameManager.player;
        /*normalEmission *= normalIntensity;
        inRangeEmission *= inRangeIntensity;
        pushedEmission *= pushedIntensity;*/

        targetPos = buttonTransform.position + buttonTransform.TransformDirection(pushDir).normalized * depth;
        //buttonMat.SetColor("_EmissionColor", normalEmission);
        renderer.material = unPressedMat;
    }

    private void Update()
    {
        if (!pushed)
        {
            Vector3 dirToPlayer = player.position - transform.position;
            buttonInRange = Vector3.Dot(-dirToPlayer.normalized, player.forward) > buttonRange;

            if (playerInRange && buttonInRange)
            {
                if (Input.GetButtonDown("Push"))
                {
                    PushButton();
                }
                else
                {
                    //Debug.Log("in range not pushed");
                    renderer.material = inRangeMat;
                    //buttonMat.SetColor("_EmissionColor", inRangeEmission);
                    //buttonMat.EnableKeyword("_EMISSION");
                }
            }
            else
            {
                //Debug.Log("not in range");
                renderer.material = unPressedMat;
                //buttonMat.SetColor("_EmissionColor", normalEmission);
                //buttonMat.EnableKeyword("_EMISSION");
            }
        }
    }

    private void FixedUpdate()
    {
        if(pushed)
        {
            buttonTransform.position = Vector3.Lerp(transform.position, targetPos, pushSpd);
        }
    }

    public void PushButton()
    {
        //Debug.Log("Button pushed");
        pushed = true;
        //buttonMat.SetColor("_EmissionColor", pushedEmission);
        //buttonMat.EnableKeyword("_EMISSION");
        renderer.material = pushedMat;
        OnPushed.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        playerInRange = false;
    }
}
