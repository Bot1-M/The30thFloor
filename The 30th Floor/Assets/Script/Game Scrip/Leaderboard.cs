using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Dan.Main;

public class Leaderboard : MonoBehaviour
{

    [SerializeField] private List<TextMeshProUGUI> playerNames;
    [SerializeField] private List<TextMeshProUGUI> playerScores;

    private string publicLeaderboardKey = "2b5d813befbc43cc69c1ddd167c808a4df6593bdc8737c13250dc1cc07919af8";
    public void GetLeaderboard()
    {
        LeaderboardCreator.GetLeaderboard(publicLeaderboardKey, ((leaderboard) =>
        {
            for (int i = 0; i < playerNames.Count; i++)
            {
                playerNames[i].text = leaderboard[i].Username;
                playerScores[i].text = leaderboard[i].Score.ToString();
            }
        }));
    }

    public void SetLeaderboardEntry(string username, int score)
    {
        LeaderboardCreator.UploadNewEntry(publicLeaderboardKey, username, score, ((_) =>
        {
            GetLeaderboard();
        }));
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
