using TMPro;
using UnityEngine;

public class ChestStatModifier : MonoBehaviour
{

    [SerializeField] private GameObject FloatingTextPrefab;
    [Header("Modificadores de stats")]
    public int healthBonus;
    public int maxHealthBonus;
    public int attackBonus;
    public int defenseBonus;
    public int spaceMovementBonus;
    public int pointsBonus;

    [Header("Destruir al tocar")]
    public bool disableOnPickup = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        AudioManager.Instance.PlaySFX("chestOpening");

        PlayerData data = PlayerManager.Instance.Data;

        int statIndex = Random.Range(1, 6);

        switch (statIndex)
        {
            case 1:
                PlayerManager.AddHealth(data, healthBonus);
                ShowFloatingText("+" + healthBonus + "HP");
                break;
            case 2:
                PlayerManager.AddMaxHealth(data, maxHealthBonus);
                ShowFloatingText("+" + maxHealthBonus + "MAX-HP");
                break;
            case 3:
                PlayerManager.AddAttack(data, attackBonus);
                ShowFloatingText("+" + attackBonus + "ATK");
                break;
            case 4:
                PlayerManager.AddMovement(data, spaceMovementBonus);
                ShowFloatingText("+" + spaceMovementBonus + "SPD");
                break;
            case 5:
                PlayerManager.AddPoints(data, pointsBonus);
                ShowFloatingText("+" + pointsBonus + "PTS");
                break;
        }

        GameManager.Instance.UpdateUI();

        if (disableOnPickup)
        {
            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().color = Color.gray;
        }
    }

    private void ShowFloatingText(string mensaje)
    {
        Debug.Log("Mostrar texto flotante de puntos");
        GameObject go = Instantiate(FloatingTextPrefab, transform.position, Quaternion.identity, transform);
        go.GetComponent<TextMeshPro>().text = mensaje;
    }
}
