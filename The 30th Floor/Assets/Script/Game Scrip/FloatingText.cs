using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float duration = 2f; // Duration for which the text will be visible

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.Instance.PlaySFX("pointsAdded"); // Play the floating text sound effect
        Destroy(gameObject, duration); // Destroy the floating text after the specified duration
    }
}
