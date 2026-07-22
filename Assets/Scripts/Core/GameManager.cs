using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private SaveManager saveManager;

    public GameState CurrentState { get; private set; } = GameState.Boot;
    public RunContext CurrentRun { get; private set; }

    private CharacterData pendingCharacter;
    private int metaXpCache;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (saveManager == null)
        {
            saveManager = FindFirstObjectByType<SaveManager>();
        }

        CharacterData loaded = saveManager != null ? saveManager.GetCharacterData() : null;
        if (loaded == null)
        {
            loaded = NewDefaultCharacter();
        }
        pendingCharacter = loaded;
        metaXpCache = 0;
    }

    public void SetState(GameState next)
    {
        if (next == CurrentState) return;

        GameState previous = CurrentState;
        CurrentState = next;
        PersistAfterTransition(previous, next);
        GameEvents.RaiseStateChanged(previous, next);
    }

    public void StartNewRun(int seed)
    {
        CurrentRun = new RunContext(seed);
        SetState(GameState.Dungeon);
    }

    public void ContinueOrStartNewRun(int fallbackSeed)
    {
        if (CurrentRun == null)
        {
            CurrentRun = new RunContext(fallbackSeed);
        }
        SetState(GameState.Dungeon);
    }

    public void AdvanceRoom()
    {
        if (CurrentRun != null)
        {
            CurrentRun.AdvanceRoom();
        }
    }

    public void AwardMetaXp(int amount)
    {
        if (amount <= 0) return;
        metaXpCache += amount;
    }

    public CharacterData GetCharacterSnapshot()
    {
        return pendingCharacter;
    }

    private void PersistAfterTransition(GameState from, GameState to)
    {
        if (saveManager == null) return;

        switch (to)
        {
            case GameState.Reward:
            case GameState.Death:
            case GameState.MainMenu:
                if (pendingCharacter != null)
                {
                    pendingCharacter.characterExperience += metaXpCache;
                    pendingCharacter.characterLevel = ComputeLevel(pendingCharacter.characterExperience);
                    metaXpCache = 0;
                    saveManager.SaveCharacter(pendingCharacter);
                }
                break;
        }
    }

    private static int ComputeLevel(int xp)
    {
        return Mathf.Max(1, xp / 100 + 1);
    }

    private static CharacterData NewDefaultCharacter()
    {
        return new CharacterData
        {
            characterName = "Hero",
            characterLevel = 1,
            characterHealth = 100f,
            characterMana = 50f,
            characterExperience = 0
        };
    }
}
