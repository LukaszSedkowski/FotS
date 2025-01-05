using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    private Dictionary<int, PlayerStats> playerStats = new Dictionary<int, PlayerStats>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializePlayers(List<int> playerIds)
    {
        foreach (int userId in playerIds)
        {
            if (!playerStats.ContainsKey(userId))
            {
                playerStats[userId] = new PlayerStats(userId);
                Debug.Log($"Zainicjalizowano statystyki dla UserID: {userId}");
            }
        }
    }

    public void RecordKill(int userId)
    {
        if (playerStats.ContainsKey(userId))
        {
            playerStats[userId].AddKill();
            Debug.Log($"UserID {userId} zdoby³ zabójstwo! Wynik: {playerStats[userId].TotalScore}");
        }
    }

    public void RecordDeath(int userId)
    {
        if (playerStats.ContainsKey(userId))
        {
            playerStats[userId].AddDeath();
            Debug.Log($"UserID {userId} straci³ jednostkê. Wynik: {playerStats[userId].TotalScore}");
        }
    }

    public void EndGame(int userId, bool win)
    {
        if (playerStats.ContainsKey(userId))
        {
            if (win)
            {
                playerStats[userId].WinGame();
            }
            else
            {
                playerStats[userId].LoseGame();
            }

            Debug.Log($"UserID {userId} zakoñczy³ grê. Wynik koñcowy: {playerStats[userId].TotalScore}");
            SendStatsToServer(playerStats[userId]);
        }
    }

    public PlayerStats GetPlayerStats(int userId)
    {
        if (playerStats.ContainsKey(userId))
        {
            return playerStats[userId];
        }
        else
        {
            Debug.LogWarning($"Statystyki dla gracza z ID {userId} nie istniej¹.");
            return null;
        }
    }


    private void SendStatsToServer(PlayerStats stats)
    {
        StartCoroutine(SendStatsCoroutine(stats));
    }

    private IEnumerator SendStatsCoroutine(PlayerStats stats)
    {
        string apiUrl = "https://localhost:7188/api/Stats/submit";
        string jsonData = JsonConvert.SerializeObject(stats.ToDictionary());

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Statystyki gracza {stats.UserId} wys³ane pomyœlnie.");
        }
        else
        {
            Debug.LogError($"B³¹d wysy³ania statystyk dla UserID {stats.UserId}: {request.responseCode}");
            Debug.LogError($"OdpowiedŸ serwera: {request.downloadHandler.text}");
        }
    }

    public void SendStatsForAllPlayers()
    {
        int teamCount = PlayerPrefs.GetInt("TeamCount", 2);

        for (int i = 0; i < teamCount; i++)
        {
            int userId = PlayerPrefs.GetInt($"Player_{i}_UserId", -1);

            if (userId != -1 && playerStats.ContainsKey(userId))
            {
                PlayerStats stats = playerStats[userId];
                Debug.Log($"Wysy³anie statystyk: UserID {userId}, Kills: {playerStats[userId].Kills}, Deaths: {playerStats[userId].Deaths}, TotalScore: {playerStats[userId].TotalScore}");
                StartCoroutine(SendStatsCoroutine(stats));
            }
            else
            {
                Debug.LogWarning($"Brak statystyk lub UserID dla dru¿yny {i + 1}");
            }
        }
        SceneManager.LoadScene("MainMenu");
    }

}
