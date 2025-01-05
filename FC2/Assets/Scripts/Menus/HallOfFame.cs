using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class HallOfFame : MonoBehaviour
{
    public GameObject hallOfFamePanel;
    public GameObject mainMenuPanel;
    public Transform listContainer;  // Kontener do umieszczenia wpisów
    public GameObject listItemPrefab;  // Prefab listy

    private string apiUrl = "https://localhost:7188/api/stats/halloffame";

    void Start()
    {
        FetchHallOfFame();
    }

    public void FetchHallOfFame()
    {
        StartCoroutine(GetHallOfFame());
    }

    private IEnumerator GetHallOfFame()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            List<UserScoreEntry> entries = JsonConvert.DeserializeObject<List<UserScoreEntry>>(request.downloadHandler.text);
            PopulateHallOfFame(entries);
        }
        else
        {
            Debug.LogError($"B³¹d pobierania Hall of Fame: {request.error}");
        }
    }

    private void PopulateHallOfFame(List<UserScoreEntry> entries)
    {
        // Wyczyœæ stare wpisy
        foreach (Transform child in listContainer)
        {
            Destroy(child.gameObject);
        }

        // Dodaj nowe wpisy
        foreach (var entry in entries)
        {
            GameObject listItem = Instantiate(listItemPrefab, listContainer);
            TMP_Text textComponent = listItem.GetComponent<TMP_Text>();

            if (textComponent != null)
            {
                textComponent.text = $"{entry.Username} - {entry.TotalScore} pkt";
            }
            else
            {
                Debug.LogError("Prefab nie zawiera komponentu TMP_Text!");
            }
        }
    }
    public void OpenMainMenu()
    {
        mainMenuPanel.SetActive(true);  // Dezaktywuj MainMenu
        hallOfFamePanel.SetActive(false);  // Aktywuj panel Hall of Fame
    }
}

// Struktura danych do deserializacji
[System.Serializable]
public class UserScoreEntry
{
    public string Username;
    public int TotalScore;
}
