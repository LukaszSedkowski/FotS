using UnityEngine;
using TMPro;

public class TournamentMenu : MonoBehaviour
{
    public TMP_Dropdown teamCountDropdown;
    public TMP_Dropdown unitsPerTeamDropdown;
    public GameObject loginForm;
    public GameObject Menu;

    public void OnNextButtonClick()
    {
        // Pobranie liczby dru¿yn i jednostek z dropdownów
        int teamCount = teamCountDropdown.value + 2;  // Opcje: 2, 3, 4 dru¿yny (indeks 0 -> 2)
        int unitsPerTeam = unitsPerTeamDropdown.value + 1;  // Opcje: 1, 2, 3 jednostki na dru¿ynê

        // Zapisanie danych do PlayerPrefs
        PlayerPrefs.SetInt("TeamCount", teamCount);
        PlayerPrefs.SetInt("UnitsPerTeam", unitsPerTeam);
        PlayerPrefs.Save();

        Debug.Log($"Dane zapisane: Dru¿yny = {teamCount}, Jednostki na dru¿ynê = {unitsPerTeam}");

        // Przejœcie do kolejnego etapu (scena wyboru jednostek)
        loginForm.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void OnBackButtonClick()
    {

        Menu.SetActive(true);  // Aktywuj Menu
        this.gameObject.SetActive(false);  // Ukryj TournamentMenu

    }
}
