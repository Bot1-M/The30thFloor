using UnityEngine;

public class ChestStatModifier : MonoBehaviour
{
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

        PlayerData data = PlayerManager.Instance.Data;

        int statIndex = Random.Range(1, 6);

        switch (statIndex)
        {
            case 1:
                PlayerManager.AddHealth(data, healthBonus);
                break;
            case 2:
                PlayerManager.AddMaxHealth(data, maxHealthBonus);
                break;
            case 3:
                PlayerManager.AddAttack(data, attackBonus);
                break;
            case 4:
                PlayerManager.AddMovement(data, spaceMovementBonus);
                break;
            case 5:
                PlayerManager.AddPoints(data, pointsBonus);
                break;
        }

        GameManager.Instance.UpdateUI();

        if (disableOnPickup)
        {
            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().color = Color.gray;
        }
    }
}
