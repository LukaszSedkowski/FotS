using UnityEngine;

public class Menus : MonoBehaviour
{
    public static Menus Instance { get; private set; }
    public bool IsLoggedIn = false;
    public User User = null;  // Pojedynczy u¿ytkownik dla trybu standardowego
    public User[] loggedUsers;  // Tablica u¿ytkowników dla trybu turniejowego
    public GameObject menu;
    public GameObject mainMenu;
    public GameObject loginForm;
    public GameObject registrationForm;
    public GameObject myAccount;
    public GameObject loginRegistration;
    public GameObject resetPassword;
    public GameObject gameMenu;
    public GameObject tournamentMenu;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // Unikaj duplikacji singletona
        }
        else
        {
            Instance = this;
            int teamCount = PlayerPrefs.GetInt("TeamCount", 2);
            loggedUsers = new User[teamCount];  // Inicjalizacja tablicy dla dru¿yn turniejowych
        }
    }
}
