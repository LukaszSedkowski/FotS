using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;

public class RegistrationForm : MonoBehaviour
{
    public TextMeshProUGUI nickField;
    public TextMeshProUGUI emailField;
    public TextMeshProUGUI passwordField;
    public TextMeshProUGUI repeatPasswordField;
    public TextMeshProUGUI emailValidationField;
    public TextMeshProUGUI passwordValidationField;
    public TextMeshProUGUI responseMessageField;

    private string apiUrl = "https://localhost:7188/api/Auth/register";

    // Reprezentacja danych u¿ytkownika
    private class RegisterRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RepeatPassword { get; set; }
    }

    // Walidacja formularza
    private string CleanInput(string input)
    {
        return input.Replace("\u200B", "").Trim();
    }

    public void EmailValidationMessage()
    {
        string email = emailField.text;
        if (IsValidEmail(email))
        {
            emailValidationField.text = "E-mail poprawny!";
            emailValidationField.color = Color.green;
        }
        else
        {
            emailValidationField.text = "E-mail niepoprawny";
            emailValidationField.color = Color.red;
        }
    }

    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;

        string emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailRegex);
    }

    public void Registration()
    {
        string nick = CleanInput(nickField.text);
        string email = CleanInput(emailField.text);
        string password = CleanInput(passwordField.text);
        string repeatPassword = CleanInput(repeatPasswordField.text);

        if (IsValidInput(nick, email, password, repeatPassword))
        {
            var request = new RegisterRequest
            {
                Username = nick,
                Email = email,
                Password = password,
                RepeatPassword = repeatPassword
            };
            StartCoroutine(SendRegistrationRequest(request));
        }
        else
        {
            responseMessageField.text = "Wszystkie pola musz¹ byæ poprawnie wype³nione.";
            responseMessageField.color = Color.red;
        }
    }

    private bool IsValidInput(string nick, string email, string password, string repeatPassword)
    {
        if (string.IsNullOrEmpty(nick) || !IsValidEmail(email) || !IsValidPassword() || password != repeatPassword)
        {
            return false;
        }
        return true;
    }

    // Wysy³anie ¿¹dania do API
    private IEnumerator SendRegistrationRequest(RegisterRequest request)
    {
        string jsonData = JsonConvert.SerializeObject(request);
        UnityWebRequest webRequest = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            responseMessageField.text = "U¿ytkownik zosta³ zarejestrowany. SprawdŸ e-mail.";
            responseMessageField.color = Color.green;
        }
        else
        {
            responseMessageField.text = $"B³¹d: {webRequest.responseCode}\n{webRequest.downloadHandler.text}";
            responseMessageField.color = Color.red;
        }
    }

    // Walidacja has³a z bardziej szczegó³owymi komunikatami
    private bool IsValidPassword()
    {
        string password = passwordField.text.Replace("\u200B", "").Trim();
        Debug.Log(password.Length);
        if (string.IsNullOrEmpty(password))
            return false;

        if (password.Length < 8)
        {
            passwordValidationField.text = "Has³o jest za krótkie (min. 8 znaków).";
            passwordValidationField.color = Color.red;
            return false;
        }
        if (!Regex.IsMatch(password, @"[A-Z]"))
        {
            passwordValidationField.text = "Has³o musi zawieraæ co najmniej jedn¹ wielk¹ literê.";
            passwordValidationField.color = Color.red;
            return false;
        }
        if (!Regex.IsMatch(password, @"[a-z]"))
        {
            passwordValidationField.text = "Has³o musi zawieraæ co najmniej jedn¹ ma³¹ literê.";
            passwordValidationField.color = Color.red;
            return false;
        }
        if (!Regex.IsMatch(password, @"\d"))
        {
            passwordValidationField.text = "Has³o musi zawieraæ co najmniej jedn¹ cyfrê.";
            passwordValidationField.color = Color.red;
            return false;
        }
        if (!Regex.IsMatch(password, @"[@$!%*?&]"))
        {
            passwordValidationField.text = "Has³o musi zawieraæ co najmniej jeden znak specjalny.";
            passwordValidationField.color = Color.red;
            return false;
        }

        passwordValidationField.text = "";
        return true;
    }

    public void RepeatPasswordValid()
    {
        if (passwordField.text.Replace("\u200B", "").Trim() != repeatPasswordField.text.Replace("\u200B", "").Trim())
        {
            passwordValidationField.text = "Has³a nie s¹ takie same";
        }
        else
        {
            passwordValidationField.text = "";
        }
    }
}
