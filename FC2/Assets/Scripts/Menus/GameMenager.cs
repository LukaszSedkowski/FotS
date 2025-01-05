using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<List<ChessPieceType>> selectedCharacters = new List<List<ChessPieceType>>();  // Lista pionków
    public List<string> playerUsernames = new List<string>();  // Lista nazw graczy
    public List<string> playerTokens = new List<string>();     // Lista tokenów JWT

    public int playerCount;
    public int pawnCount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Zachowujemy GameManager miêdzy scenami
        }
        else
        {
            Destroy(gameObject);  // Usuwamy duplikaty GameManagera
        }
    }

    public void SetGameData(int players, int pawns, List<List<ChessPieceType>> characters, List<string> usernames, List<string> tokens)
    {
        playerCount = players;
        pawnCount = pawns;
        selectedCharacters = characters;
        playerUsernames = usernames;
        playerTokens = tokens;
    }
}
