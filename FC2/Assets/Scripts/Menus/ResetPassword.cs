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
            passwordValidText.text = "Nowe has�o nie spe�nia wymaga�.";
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
            passwordValidText.text = "Has�o zosta�o zmienione.";
            passwordValidText.color = Color.green;
            Debug.Log("Has�o zmienione pomy�lnie.");
        }
        else
        {
            Debug.LogError($"B��d: {request.responseCode} - {request.downloadHandler.text}");
            passwordValidText.text = "B��d podczas zmiany has�a.";
            passwordValidText.color = Color.red;
        }
    }

    private bool IsValidForm()
    {
        string currentPassword = SanitizeInput(currentPasswordField.text);
        string newPassword = SanitizeInput(newPasswordField.text);

        if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword))
        {
            passwordValidText.text = "Wszystkie pola musz� by� wype�nione.";
            passwordValidText.color = Color.red;
            return false;
        }

        if (!IsStrongPassword(newPassword))
        {
            passwordValidText.text = "Has�o musi mie� co najmniej 8 znak�w, zawiera� wielk� i ma�� liter�, cyfr� oraz znak specjalny.";
            passwordValidText.color = Color.red;
            return false;
        }

        return true;
    }

    private bool IsStrongPassword(string password)
    {
        string passwordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$";
        password = SanitizeInput(password); // Usu� ukryte znaki i spacje przed walidacj�
        bool isValid = Regex.IsMatch(password, passwordRegex);
        Debug.Log($"Has�o: '{password}', Spe�nia wymagania: {isValid}");
        return isValid;
    }

    // Metoda do czyszczenia niewidzialnych znak�w i spacji
    private string SanitizeInput(string input)
    {
        return input.Replace("\u200B", "").Trim();
    }
}
