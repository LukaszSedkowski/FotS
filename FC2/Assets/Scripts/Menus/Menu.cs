using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject tournamentMenu;

    public void GuestPlay()
    {
        DisableCamera();
        SceneManager.LoadScene(1);
    }

    void DisableCamera()
    {
        Camera cameraToDisable = GameObject.Find("Main Camera").GetComponent<Camera>();
        if (cameraToDisable != null)
        {
            cameraToDisable.enabled = false;
        }
    }

    private void ResetTournamentData()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Wszystkie dane zosta³y ca³kowicie zresetowane.");
    }

    public void OpenTournamentMenu()
    {
        ResetTournamentData();

        PlayerPrefs.DeleteKey("JwtToken");
        PlayerPrefs.DeleteKey("Username");
        PlayerPrefs.DeleteKey("Email");
        PlayerPrefs.Save();

        Menus.Instance.IsLoggedIn = false;
        Menus.Instance.User = null;

        var loginForm = Menus.Instance.loginForm.GetComponent<LoginForm>();
        loginForm.currentMode = LoginMode.Tournament;

        tournamentMenu.SetActive(true);
        gameObject.SetActive(false);  // Wy³¹czenie bie¿¹cego menu
    }
}
