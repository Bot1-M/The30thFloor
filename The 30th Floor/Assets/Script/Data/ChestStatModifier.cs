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
                if (healthBonus != 0)
                {
                    data.currentHealth += healthBonus;
                    Debug.Log($"+{healthBonus} HP");
                }
                break;
            case 2:
                if (maxHealthBonus != 0)
                {
                    data.maxHealth += maxHealthBonus;
                    data.currentHealth = Mathf.Min(data.currentHealth, data.maxHealth);
                    Debug.Log($"+{maxHealthBonus} Max HP");
                }
                break;
            case 3:
                if (attackBonus != 0)
                {
                    data.attack += attackBonus;
                    Debug.Log($"+{attackBonus} ATK");
                }
                break;
            case 4:
                if (spaceMovementBonus != 0)
                {
                    data.spaceMovement += spaceMovementBonus;
                    Debug.Log($"+{spaceMovementBonus} MOV");
                }
                break;
            case 5:
                if (pointsBonus != 0)
                {
                    data.totalPoints += pointsBonus;
                    Debug.Log($"+{pointsBonus} PTS");
                }
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
