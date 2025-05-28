using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private int score;
    [SerializeField] private string playerName;

    public static void SubmitScore()
    {
        MyLeaderboard.SetLeaderboardEntry(PlayerManager.Instance.Data.playerName, PlayerManager.Instance.Data.totalPoints);
    }


}
