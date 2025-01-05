using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class CharacterSelectionController : MonoBehaviour
{
    [Header("UI References")]
    public Transform characterGrid;                    // Kontener na przyciski
    public List<GameObject> characterPrefabs;          // Lista prefab�w postaci

    private List<ChessPieceType> selectedCharacters = new List<ChessPieceType>();

    private void Start()
    {
        GenerateCharacterButtons();
    }

    // Generowanie przycisk�w na podstawie prefab�w
    private void GenerateCharacterButtons()
    {
        foreach (Transform child in characterGrid)
        {
            Destroy(child.gameObject);  // Wyczy�� stare przyciski
        }

        foreach (GameObject characterPrefab in characterPrefabs)
        {
            GameObject buttonObj = Instantiate(characterPrefab, characterGrid);
            buttonObj.GetComponent<Button>().onClick.AddListener(() => SelectCharacter(characterPrefab.name));
        }
    }

    // Wyb�r postaci po klikni�ciu
    private void SelectCharacter(string characterName)
    {
        int unitsPerTeam = PlayerPrefs.GetInt("UnitsPerTeam", 1);
        int currentTeamIndex = PlayerPrefs.GetInt("CurrentTeamIndex", 0);

        string key = $"Player_{currentTeamIndex}_Pieces";
        string savedPieces = PlayerPrefs.GetString(key, "");
        List<string> selectedPieces = new List<string>(savedPieces.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries));

        if (selectedPieces.Count < unitsPerTeam)
        {
            selectedPieces.Add(characterName);
            PlayerPrefs.SetString(key, string.Join(",", selectedPieces));
            PlayerPrefs.Save();

            Debug.Log($"Dodano pionek {characterName} do dru�yny {currentTeamIndex + 1}. Aktualny sk�ad: {string.Join(", ", selectedPieces)}");
            DisableButton(characterName);
        }
        else
        {
            Debug.LogWarning($"Dru�yna {currentTeamIndex + 1} osi�gn�a limit jednostek.");
        }
    }

    // Wy��czenie przycisku po wyborze
    private void DisableButton(string characterName)
    {
        foreach (Transform child in characterGrid)
        {
            if (child.name == characterName)
            {
                child.GetComponent<Button>().interactable = false;
                break;
            }
        }
    }
    // Resetowanie przycisk�w po zalogowaniu dru�yny
    public void ResetButtons()
    {
        foreach (Transform child in characterGrid)
        {
            child.GetComponent<Button>().interactable = true;
        }
        selectedCharacters.Clear();  // Wyczy�� wybrane postacie po zalogowaniu
        Debug.Log("Przyciski postaci zosta�y zresetowane.");
    }
}
