using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.UI;
using System.Collections.Generic;

public enum LoginMode
{
    Standard,
    Tournament
}

public class LoginForm : MonoBehaviour
{
    [Header("Character Selection")]
    public Transform characterGrid;                  // Grid na przyciski postaci
    private List<ChessPieceType> selectedCharacters = new List<ChessPieceType>();  // Wybrane postacie

    [Header("UI References")]
    public TMP_InputField loginEmailField;
    public TMP_InputField loginPasswordField;
    public TextMeshProUGUI validMessage;
    public TextMeshProUGUI teamNumberText;  // Wyœwietla numer logowanej dru¿yny
    public GameObject loginForm;
    public GameObject mainMenu;
    public User[] loggedUsers;

    private string apiUrl = "https://localhost:7188/api/Auth/login";
    private string jwtToken;

    public LoginMode currentMode = LoginMode.Standard;

    private int totalTeams;
    private int currentTeamIndex = 0;

    // Start - inicjalizacja logowania i wyboru postaci
    private void Start()
    {
        if (currentMode == LoginMode.Tournament)
        {
            totalTeams = PlayerPrefs.GetInt("TeamCount", 2);
            loggedUsers = new User[totalTeams];

            // Inicjalizacja przycisków postaci
            InitializeCharacterSelection();
            UpdateTeamNumber();
        }
    }

    // Obs³uga klikniêcia "Zaloguj"
    public void Login()
    {
        if (ValidateFields())
        {
            StartCoroutine(SendLoginRequest());
        }
    }

    // Walidacja pól email/has³o
    private bool ValidateFields()
    {
        if (string.IsNullOrWhiteSpace(loginEmailField.text) || string.IsNullOrWhiteSpace(loginPasswordField.text))
        {
            validMessage.text = "Wype³nij wszystkie pola!";
            validMessage.color = Color.red;
            return false;
        }
        return true;
    }

    // Wys³anie ¿¹dania logowania do API
    private IEnumerator SendLoginRequest()
    {
        var requestData = new LoginRequest
        {
            Email = RemoveZeroWidthSpace(loginEmailField.text.Trim()),
            Password = RemoveZeroWidthSpace(loginPasswordField.text.Trim())
        };

        string jsonData = JsonConvert.SerializeObject(requestData);
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST")
        {
            uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData)),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        HandleResponse(request);
    }

    // Obs³uga odpowiedzi z API
    private void HandleResponse(UnityWebRequest request)
    {
        if (request.result == UnityWebRequest.Result.Success)
        {
            var response = request.downloadHandler.text;
            var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(response);
            jwtToken = loginResponse.Token;

            if (currentMode == LoginMode.Standard)
            {
                HandleStandardLogin(loginResponse);
            }
            else if (currentMode == LoginMode.Tournament)
            {
                HandleTournamentLogin(loginResponse);
            }

            ClearLoginFields();
        }
        else
        {
            validMessage.text = "B³¹d logowania.";
            validMessage.color = Color.red;
        }
    }

    // Czyszczenie pól logowania
    private void ClearLoginFields()
    {
        loginEmailField.text = "";
        loginPasswordField.text = "";
        validMessage.text = "";
    }

    // Logowanie standardowe (pojedynczy u¿ytkownik)
    private void HandleStandardLogin(LoginResponse loginResponse)
    {
        //characterGrid.gameObject.SetActive(false);
        PlayerPrefs.SetString("JwtToken", jwtToken);
        PlayerPrefs.SetString("Username", loginResponse.Username);
        PlayerPrefs.SetString("Email", loginResponse.Email);
        PlayerPrefs.Save();

        validMessage.text = "Zalogowano pomyœlnie!";
        validMessage.color = Color.green;

        Menus.Instance.IsLoggedIn = true;
        Menus.Instance.User = new User(loginResponse.Username, "", loginResponse.Email);
        Menus.Instance.mainMenu.SetActive(true);
        Menus.Instance.loginForm.SetActive(false);
    }
    /*private void HandleTournamentLogin(LoginResponse loginResponse)
    {
        if (IsUserAlreadyLogged(loginResponse.Email))
        {
            validMessage.text = "Ten u¿ytkownik jest ju¿ zalogowany.";
            validMessage.color = Color.red;
            return;
        }

        // Logowanie u¿ytkownika
        Menus.Instance.loggedUsers[currentTeamIndex] = new User(loginResponse.Username, "", loginResponse.Email);
        Debug.Log($"Dru¿yna {currentTeamIndex + 1} zalogowana: {loginResponse.Username}");

        // Zapisanie Username i Tokena w PlayerPrefs
        PlayerPrefs.SetString($"Player_{currentTeamIndex}_Username", loginResponse.Username);
        PlayerPrefs.SetString($"Player_{currentTeamIndex}_Token", jwtToken);

        // Zapis wybranych pionków do PlayerPrefs
        List<string> selectedPieces = new List<string>();
        foreach (var piece in selectedCharacters)
        {
            selectedPieces.Add(piece.ToString());
        }

        if (selectedPieces.Count > 0)
        {
            PlayerPrefs.SetString($"Player_{currentTeamIndex}_Pieces", string.Join(",", selectedPieces));
            Debug.Log($"Pionki dru¿yny {currentTeamIndex + 1}: {string.Join(", ", selectedPieces)}");
        }
        else
        {
            Debug.LogWarning($"Dru¿yna {currentTeamIndex + 1} nie wybra³a pionków.");
            PlayerPrefs.SetString($"Player_{currentTeamIndex}_Pieces", "Brak pionków");
        }

        PlayerPrefs.Save();

        // Resetowanie przycisków po zalogowaniu
        FindObjectOfType<CharacterSelectionController>().ResetButtons();

        currentTeamIndex++;

        // Przejœcie do logowania kolejnej dru¿yny lub rozpoczêcie gry
        if (currentTeamIndex < totalTeams)
        {
            UpdateTeamNumber();
        }
        else
        {
            validMessage.text = "Wszystkie dru¿yny zalogowane!";
            validMessage.color = Color.green;
            StartGame();
        }
    }*/

    private void HandleTournamentLogin(LoginResponse loginResponse)
    {
        if (IsUserAlreadyLogged(loginResponse.Email))
        {
            validMessage.text = "Ten u¿ytkownik jest ju¿ zalogowany.";
            validMessage.color = Color.red;
            return;
        }

        Menus.Instance.loggedUsers[currentTeamIndex] = new User(loginResponse.Username, "", loginResponse.Email);
        Debug.Log($"Dru¿yna {currentTeamIndex + 1} zalogowana: {loginResponse.Username}");

        // Zapisanie Username i Tokena w PlayerPrefs
        PlayerPrefs.SetString($"Player_{currentTeamIndex}_Username", loginResponse.Username);
        PlayerPrefs.SetString($"Player_{currentTeamIndex}_Token", jwtToken);
        PlayerPrefs.SetInt($"Player_{currentTeamIndex}_UserId", loginResponse.UserId);

        // Pobierz pionki z PlayerPrefs
        string pieces = PlayerPrefs.GetString($"Player_{currentTeamIndex}_Pieces", "Brak pionków");
        Debug.Log($"Pionki dru¿yny {currentTeamIndex + 1}: {pieces}");

        PlayerPrefs.Save();

        Debug.Log($"Zapisano gracza: {loginResponse.Username} (UserID: {loginResponse.UserId}) do PlayerPrefs dla dru¿yny {currentTeamIndex + 1}");


        // Reset przycisków po zalogowaniu dru¿yny
        FindObjectOfType<CharacterSelectionController>().ResetButtons();

        currentTeamIndex++;

        if (currentTeamIndex < totalTeams)
        {
            UpdateTeamNumber();
        }
        else
        {
            validMessage.text = "Wszystkie dru¿yny zalogowane!";
            validMessage.color = Color.green;
            StartGame();
        }
    }

    // Rozpoczêcie gry po zalogowaniu wszystkich dru¿yn
    private void StartGame()
    {
        Debug.Log("Logowanie zakoñczone. Oto dane turniejowe:");

        int totalTeams = PlayerPrefs.GetInt("TeamCount", 2);
        int unitsPerTeam = PlayerPrefs.GetInt("UnitsPerTeam", 1);

        Debug.Log($"Liczba dru¿yn: {totalTeams}, Pionki na dru¿ynê: {unitsPerTeam}");

        for (int i = 0; i < totalTeams; i++)
        {
            string username = PlayerPrefs.GetString($"Player_{i}_Username", "Unknown");
            string token = PlayerPrefs.GetString($"Player_{i}_Token", "NoToken");
            string pieces = PlayerPrefs.GetString($"Player_{i}_Pieces", "Brak pionków");

            Debug.Log($"Dru¿yna {i + 1}: Gracz: {username}, Token: {token}, Pionki: {pieces}");
        }

        Debug.Log("Wszystkie dru¿yny zalogowane. Rozpoczêcie gry...");
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    // Inicjalizacja przycisków postaci (na starcie)
    private void InitializeCharacterSelection()
    {
        foreach (Transform child in characterGrid)
        {
            Button button = child.GetComponent<Button>();
            if (button != null)
            {
                string characterName = child.name;
                button.onClick.AddListener(() => SelectCharacter(characterName));
            }
        }
    }

    // Wybór postaci dla dru¿yny
    private void SelectCharacter(string characterName)
    {
        if (selectedCharacters.Count < PlayerPrefs.GetInt("UnitsPerTeam", 1))
        {
            ChessPieceType selectedType = (ChessPieceType)System.Enum.Parse(typeof(ChessPieceType), characterName);
            selectedCharacters.Add(selectedType);

            Debug.Log($"Wybrano postaæ: {characterName}");

            DisableButton(characterName);
        }
        else
        {
            Debug.Log("Osi¹gniêto limit jednostek na dru¿ynê.");
        }
    }

    // Dezaktywacja przycisku po wybraniu postaci
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

    // Aktualizacja informacji o numerze dru¿yny
    private void UpdateTeamNumber()
    {
        PlayerPrefs.SetInt("CurrentTeamIndex", currentTeamIndex);
        teamNumberText.text = $"Logowanie dru¿yny {currentTeamIndex + 1} z {totalTeams}";
        Debug.Log($"Logowanie dru¿yny {currentTeamIndex + 1}");
    }

    // Sprawdzenie, czy u¿ytkownik jest ju¿ zalogowany
    private bool IsUserAlreadyLogged(string email)
    {
        foreach (var user in loggedUsers)
        {
            if (user != null && user.GetEmail() == email)
            {
                return true;
            }
        }
        return false;
    }

    // Usuniêcie ukrytych spacji z emaila i has³a
    private string RemoveZeroWidthSpace(string input)
    {
        return input.Replace("\u200B", "").Replace("\u200C", "").Replace("\u200D", "");
    }
}

// Klasa do obs³ugi danych logowania (API)
public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}

[System.Serializable]
public class LoginResponse
{
    public string Token;
    public string Username;
    public string Email;
    public int UserId;
}
