using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float maxLife = 10f;
    float currentLife;
    bool canBeDamaged = true;
    [SerializeField] float graceTime = 1f;
    [SerializeField] Slider healthSlider;
    [SerializeField] float healthSliderLerp = 0.3f;
    [SerializeField] UnityEvent OnReceiveDamage;

    private void Start()
    {
        currentLife = maxLife;
    }

    private void FixedUpdate()
    {
        healthSlider.value = Mathf.Lerp(healthSlider.value, currentLife / maxLife, healthSliderLerp);
    }

    public void ReceiveDamage()
    {
        if (canBeDamaged)
        {
            canBeDamaged = false;
            OnReceiveDamage.Invoke();
            currentLife--;
            if (currentLife < 1) GameManager.Die();
            else StartCoroutine("GraceTimeCoroutine");
        }
    }

    IEnumerator GraceTimeCoroutine()
    {
        yield return new WaitForSeconds(graceTime);
        canBeDamaged = true;
    }
}
