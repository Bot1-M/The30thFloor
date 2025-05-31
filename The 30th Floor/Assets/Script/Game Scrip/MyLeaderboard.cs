using Dan.Main;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MyLeaderboard : MonoBehaviour
{
    public static MyLeaderboard Instance { get; private set; }

    [SerializeField] private List<TextMeshProUGUI> playerNames;
    [SerializeField] private List<TextMeshProUGUI> playerScores;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (playerNames.Count != playerScores.Count)
        {
            Debug.LogError("Player names and scores lists must have the same length.");
        }
    }


    private void Start()
    {
        GetLeaderboard();
    }

    public void GetLeaderboard()
    {
        if (playerNames.Count == 0 || playerScores.Count == 0)
        {
            Debug.LogWarning("UI del leaderboard no está lista.");
            return;
        }

        Leaderboards.The30ThFloorLeaderboard.GetEntries(ArrayEntries =>
        {
            foreach (var entry in playerNames)
                entry.text = "";
            foreach (var score in playerScores)
                score.text = "";
            var length = Mathf.Min(playerNames.Count, ArrayEntries.Length);
            for (int i = 0; i < length; i++)
            {
                playerNames[i].text = ArrayEntries[i].Username;
                playerScores[i].text = ArrayEntries[i].Score.ToString();
            }
        });
    }


    public static void SetLeaderboardEntry(string username, int score)
    {
        Leaderboards.The30ThFloorLeaderboard.UploadNewEntry(username, score, isSuccessfull =>
        {
            if (isSuccessfull)
            {
                Debug.Log("Entry uploaded successfully.");
                MyLeaderboard.Instance.GetLeaderboard();

            }
            else
            {
                Debug.LogError("Failed to upload entry.");
            }

        });
    }
}