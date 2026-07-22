using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private SaveManager saveManager;
    [SerializeField] private LevelCurve levelCurve;

    public GameState CurrentState { get; private set; } = GameState.Boot;
    public RunContext CurrentRun { get; private set; }

    private CharacterData pendingCharacter;
    private bool dirty;

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
        if (levelCurve == null)
        {
            levelCurve = Resources.Load<LevelCurve>("LevelCurve");
        }

        CharacterData loaded = saveManager != null ? saveManager.LoadCharacterData() : null;
        if (loaded == null)
        {
            loaded = NewDefaultCharacter();
        }
        pendingCharacter = loaded;
        dirty = false;
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
        ApplyMetaModifiersForRun();
        SetState(GameState.Dungeon);
    }

    public void ContinueOrStartNewRun(int fallbackSeed)
    {
        if (CurrentRun == null)
        {
            CurrentRun = new RunContext(fallbackSeed);
        }
        ApplyMetaModifiersForRun();
        SetState(GameState.Dungeon);
    }

    private void ApplyMetaModifiersForRun()
    {
        RunModifierApplier applier = null;
        try { applier = FindFirstObjectByType<RunModifierApplier>(); } catch { }
        if (applier != null)
        {
            try { applier.ResetForRun(); }
            catch (System.Exception e) { Debug.LogError($"RunModifierApplier.ResetForRun threw: {e.Message}"); }
        }
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
        if (pendingCharacter == null) return;
        pendingCharacter.metaXp += amount;
        if (levelCurve != null)
        {
            pendingCharacter.characterLevel = levelCurve.LevelFromXp(pendingCharacter.characterExperience);
        }
        dirty = true;
    }

    public bool SpendMetaXp(int amount)
    {
        if (amount <= 0) return true;
        if (pendingCharacter == null) return false;
        if (pendingCharacter.metaXp < amount) return false;
        pendingCharacter.metaXp -= amount;
        dirty = true;
        return true;
    }

    public bool AwardExperience(int amount)
    {
        if (amount <= 0) return false;
        if (pendingCharacter == null) return false;
        pendingCharacter.characterExperience += amount;
        if (levelCurve != null)
        {
            pendingCharacter.characterLevel = levelCurve.LevelFromXp(pendingCharacter.characterExperience);
        }
        dirty = true;
        return true;
    }

    public CharacterData GetCharacterSnapshot()
    {
        return pendingCharacter;
    }

    public LevelCurve GetLevelCurve() => levelCurve;

    private void PersistAfterTransition(GameState from, GameState to)
    {
        if (saveManager == null) return;
        if (!dirty) return;

        switch (to)
        {
            case GameState.Reward:
            case GameState.Death:
            case GameState.MainMenu:
                if (pendingCharacter != null)
                {
                    saveManager.SaveCharacterData(pendingCharacter);
                    dirty = false;
                }
                break;
        }
    }

    private static CharacterData NewDefaultCharacter()
    {
        return new CharacterData
        {
            characterName = "Hero",
            characterLevel = 1,
            characterHealth = 100f,
            characterMana = 50f,
            characterExperience = 0,
            metaXp = 0
        };
    }
}
