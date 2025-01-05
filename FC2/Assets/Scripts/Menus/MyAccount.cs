using TMPro;
using UnityEngine;

public class MyAccount : MonoBehaviour
{
    public GameObject iconeImage;
    public TextMeshProUGUI nickField;
    public TextMeshProUGUI emailField;
    public LoginForm loginForm;

    void Start()
    {
        UpdateAccountInfo();
    }

    // Aktualizuje dane konta po zalogowaniu
    public void UpdateAccountInfo()
    {
        if (Menus.Instance != null && Menus.Instance.User != null)
        {
            nickField.text = Menus.Instance.User.GetNick();
            emailField.text = Menus.Instance.User.GetEmail();
        }
        else
        {
            Debug.LogWarning("Brak zalogowanego u¿ytkownika. Przechodzenie do logowania.");
            OpenLoginForm();
        }
    }

    // Otwiera formularz logowania, jeœli u¿ytkownik nie jest zalogowany
    public void OpenLoginForm()
    {
        loginForm.currentMode = LoginMode.Standard;
        loginForm.gameObject.SetActive(true);
        gameObject.SetActive(false);  // Ukrywa ekran konta, dopóki u¿ytkownik siê nie zaloguje
    }

    // Wylogowanie u¿ytkownika
    public void LogoutButton()
    {
        PlayerPrefs.DeleteKey("JwtToken");
        PlayerPrefs.DeleteKey("Username");
        PlayerPrefs.DeleteKey("Email");
        PlayerPrefs.Save();

        Menus.Instance.IsLoggedIn = false;
        Menus.Instance.User = null;

        // Resetowanie tekstu i powrót do g³ównego menu
        nickField.text = "Brak danych";
        emailField.text = "Brak danych";

        Menus.Instance.myAccount.SetActive(false);
        Menus.Instance.mainMenu.SetActive(true);

        // Otwórz logowanie po wylogowaniu
        OpenLoginForm();
    }
}
