using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBars : MonoBehaviour
{
    [SerializeField] private Image healthBarFill;
    private float fillSpeed;
    private Gradient colorGradient;

    public void GMInitialize(float speed, Gradient gradient)
    {
        fillSpeed = speed;
        colorGradient = gradient;
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        float targetFillAmount = currentHealth / maxHealth;
        healthBarFill.fillAmount = Mathf.Lerp(healthBarFill.fillAmount, targetFillAmount, Time.deltaTime * fillSpeed);
        healthBarFill.color = colorGradient.Evaluate(targetFillAmount);
    }
}
