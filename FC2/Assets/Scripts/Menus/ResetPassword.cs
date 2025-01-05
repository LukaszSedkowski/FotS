using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;

public class ChangePassword : MonoBehaviour
{
    public TextMeshProUGUI currentPasswordField;
    public TextMeshProUGUI newPasswordField;
    public TextMeshProUGUI passwordValidText;

    private string apiUrl = "https://localhost:7188/api/Auth/change-password";

    public void ChangePasswordRequest()
    {
        if (IsValidForm())
        {
            StartCoroutine(SendChangePasswordRequest());
        }
        else
        {
            passwordValidText.text = "Nowe has³o nie spe³nia wymagañ.";
            passwordValidText.color = Color.red;
        }
    }

    private IEnumerator SendChangePasswordRequest()
    {
        var requestData = new
        {
            CurrentPassword = SanitizeInput(currentPasswordField.text),
            NewPassword = SanitizeInput(newPasswordField.text)
        };

        string jsonData = JsonConvert.SerializeObject(requestData);
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST")
        {
            uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData)),
            downloadHandler = new DownloadHandlerBuffer()
        };

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("JwtToken")}");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            passwordValidText.text = "Has³o zosta³o zmienione.";
            passwordValidText.color = Color.green;
            Debug.Log("Has³o zmienione pomyœlnie.");
        }
        else
        {
            Debug.LogError($"B³¹d: {request.responseCode} - {request.downloadHandler.text}");
            passwordValidText.text = "B³¹d podczas zmiany has³a.";
            passwordValidText.color = Color.red;
        }
    }

    private bool IsValidForm()
    {
        string currentPassword = SanitizeInput(currentPasswordField.text);
        string newPassword = SanitizeInput(newPasswordField.text);

        if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword))
        {
            passwordValidText.text = "Wszystkie pola musz¹ byæ wype³nione.";
            passwordValidText.color = Color.red;
            return false;
        }

        if (!IsStrongPassword(newPassword))
        {
            passwordValidText.text = "Has³o musi mieæ co najmniej 8 znaków, zawieraæ wielk¹ i ma³¹ literê, cyfrê oraz znak specjalny.";
            passwordValidText.color = Color.red;
            return false;
        }

        return true;
    }

    private bool IsStrongPassword(string password)
    {
        string passwordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$";
        password = SanitizeInput(password); // Usuñ ukryte znaki i spacje przed walidacj¹
        bool isValid = Regex.IsMatch(password, passwordRegex);
        Debug.Log($"Has³o: '{password}', Spe³nia wymagania: {isValid}");
        return isValid;
    }

    // Metoda do czyszczenia niewidzialnych znaków i spacji
    private string SanitizeInput(string input)
    {
        return input.Replace("\u200B", "").Trim();
    }
}
