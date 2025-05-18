using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider fillHealth;

    public Gradient gradient;
    public Image fillImage;

    public void SetHealth(float health)
    {
        fillHealth.value = health;

        fillImage.color = gradient.Evaluate(1f);
    }

    public void SetMaxHealth(float maxHealth)
    {
        fillHealth.maxValue = maxHealth;

        fillImage.color = gradient.Evaluate(fillHealth.normalizedValue);

    }


}
