using UnityEngine;

public enum ChessPieceType
{
    None = 0,
    Hunter = 1,
    Priestess = 2,
    Dog = 3,
    Knight = 4,
    Ogre = 5,
    Skeleton = 6,
    Vampir = 7,
    Werewolf = 8
}

public class ChessPieces : MonoBehaviour
{
    public int team;               // Drużyna (0, 1, 2, 3 itd.)
    public int currentX;           // Aktualna pozycja X
    public int currentY;           // Aktualna pozycja Y
    public ChessPieceType type;    // Typ pionka
    public int Id { get; private set; }  // Unikalne ID pionka

    public int movementRange;
    public int maxMovementRange;   // Maksymalny zasięg ruchu
    public int health;
    public int maxHealth;
    public int attack;
    public int attackRange;
    public int attackCost;

    public bool hasPassiveAbility;

    // Nowe pola dla gracza
    public string username;        // Nazwa gracza
    public string jwtToken;        // Token JWT gracza

    private Vector3 desiredPosition;
    private Vector3 desiredScale;

    // Inicjalizacja pionka z dodatkowymi danymi gracza
    public void Init(ChessPieceType type, int team, int id, string playerName = "", string token = "")
    {
        this.type = type;
        this.team = team;
        this.Id = id;

        // Ustawienie username i tokenu
        this.username = playerName;
        this.jwtToken = token;

        SetStats();  // Ustawienie statystyk pionka

        Debug.Log($"Initialized piece {Id} (Type: {type}) for team {team}. " +
                  $"Player: {username}, Movement Range: {movementRange}, Health: {health}");
    }

    // Domyślne statystyki pionka – można je nadpisać w zależności od klasy pionka
    protected virtual void SetStats()
    {
        health = 100;
        maxHealth = 100;
        Debug.Log($"Ustawiono statystyki: HP {health}/{maxHealth}, Ruch: {movementRange}");
    }

    // Aktywacja umiejętności pasywnej (do nadpisania w podklasach)
    public virtual void TriggerPassiveAbility()
    {
    }

    // Ustawienie pionka na podstawie danych z PlayerPrefs
    public void SetPlayerDataFromPrefs(int playerIndex)
    {
        this.username = PlayerPrefs.GetString($"Player_{playerIndex}_Username", "Unknown");
        this.jwtToken = PlayerPrefs.GetString($"Player_{playerIndex}_Token", "NoToken");

        Debug.Log($"Piece assigned to player {username} with token {jwtToken}");
    }
}
