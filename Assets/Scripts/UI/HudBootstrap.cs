using UnityEngine;
using UnityEngine.UI;

public class HudBootstrap : MonoBehaviour
{
    private static HudBootstrap _instance;

    public static HudBootstrap Instance => _instance;

    private GameObject canvasGo;
    private GameObject hudGo;
    private GameObject deathScreenGo;
    private GameObject rewardScreenGo;
    private GameObject menuScreenGo;

    private HudPanel hudPanel;
    private Text rewardText;
    private Text deathText;
    private Button startButton;

    void OnEnable()
    {
        GameEvents.OnStateChanged += HandleState;
    }

    void OnDisable()
    {
        GameEvents.OnStateChanged -= HandleState;
    }

    void Awake()
    {
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        BuildCommon(rootName: "RuntimeUI");
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureSingleton()
    {
        if (_instance != null) return;
        GameObject go = new GameObject("HudBootstrap");
        go.AddComponent<HudBootstrap>();
    }

    private void BuildCommon(string rootName)
    {
        if (canvasGo != null) return;

        canvasGo = new GameObject(rootName);
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();

        if (canvasGo.transform.parent == null)
        {
            DontDestroyOnLoad(canvasGo);
        }
    }

    private void HandleState(GameState from, GameState to)
    {
        EnsureCore();

        switch (to)
        {
            case GameState.Dungeon:
                BuildHud();
                Hide(deathScreenGo);
                Hide(rewardScreenGo);
                Hide(menuScreenGo);
                if (hudPanel != null) hudPanel.Refresh();
                break;
            case GameState.Reward:
                BuildRewardScreen();
                Hide(hudGo);
                Show(rewardScreenGo);
                break;
            case GameState.Death:
                BuildDeathScreen();
                Hide(hudGo);
                Show(deathScreenGo);
                break;
            case GameState.MainMenu:
                Hide(hudGo);
                Hide(deathScreenGo);
                Hide(rewardScreenGo);
                BuildMenuScreen();
                Show(menuScreenGo);
                break;
            case GameState.Boot:
                HideAll();
                break;
        }
    }

    private void EnsureCore()
    {
        if (canvasGo == null) BuildCommon("RuntimeUI");
    }

    private void BuildHud()
    {
        if (hudGo != null) return;
        hudGo = NewUi("HUD");

        hudPanel = hudGo.AddComponent<HudPanel>();
        CreateBar(hudGo.transform, "HealthBar", new Vector2(-800f, 480f), out Slider healthSlider, out Text healthLabel);
        hudPanel.healthBar = healthSlider;

        CreateBar(hudGo.transform, "ManaBar", new Vector2(-800f, 420f), out Slider manaSlider, out Text manaLabel, color: new Color(0.2f, 0.4f, 1f, 0.5f));
        hudPanel.manaBar = manaSlider;

        CreateBar(hudGo.transform, "XpBar", new Vector2(-800f, 360f), out Slider xpSlider, out Text xpLabel, color: new Color(0.6f, 0.85f, 0.4f, 0.5f));
        hudPanel.xpBar = xpSlider;

        hudPanel.levelValue = CreateText(hudGo.transform, "LevelValue", "+0y", new Vector2(-800f, 530f), FontStyle.Bold, 28);

        hudPanel.roomCounterText = CreateText(hudGo.transform, "RoomCounter", "Room 1", new Vector2(-880f, 470f), FontStyle.Normal, 22);

        hudPanel.metaXpText = CreateText(hudGo.transform, "MetaXp", "MetaXP 0", new Vector2(-880f, -510f), FontStyle.Normal, 22);
    }

    private void BuildMenuScreen()
    {
        if (menuScreenGo != null) return;
        menuScreenGo = NewUi("MenuScreen");
        CreateText(menuScreenGo.transform, "Title", "2D ROGUELITE", new Vector2(0, 200), FontStyle.Bold, 84);

        GameObject btnGo = CreateButton(menuScreenGo.transform, "StartButton", "START RUN", new Vector2(0, 0), out Button btn, 320, 96);
        startButton = btn;
        startButton.onClick.AddListener(StartRun);
        CreateText(menuScreenGo.transform, "Subtitle", "Press the button to begin", new Vector2(0, 110), FontStyle.Normal, 22);
    }

    private void BuildDeathScreen()
    {
        if (deathScreenGo != null) return;
        deathScreenGo = NewUi("DeathScreen");
        deathText = CreateText(deathScreenGo.transform, "DeathLabel", "YOU DIED", new Vector2(0, 200), FontStyle.Bold, 72);

        CreateText(deathScreenGo.transform, "Summary", BuildDeathSummary(), new Vector2(0, 100), FontStyle.Normal, 24);

        GameObject btnGo = CreateButton(deathScreenGo.transform, "RetryButton", "RETRY", new Vector2(0, -50), out Button retry, 280, 80);
        retry.onClick.AddListener(StartRun);

        GameObject quitBtn = CreateButton(deathScreenGo.transform, "QuitButton", "MAIN MENU", new Vector2(0, -150), out Button quit, 280, 80);
        quit.onClick.AddListener(GoMainMenu);
    }

    private void BuildRewardScreen()
    {
        if (rewardScreenGo != null) return;
        rewardScreenGo = NewUi("RewardScreen");
        rewardText = CreateText(rewardScreenGo.transform, "Reward", "RUN COMPLETE", new Vector2(0, 200), FontStyle.Bold, 60);

        CreateText(rewardScreenGo.transform, "Bonus", BuildRewardSummary(), new Vector2(0, 80), FontStyle.Normal, 22);

        GameObject btn = CreateButton(rewardScreenGo.transform, "ContinueButton", "CONTINUE", new Vector2(0, -50), out Button next, 280, 80);
        next.onClick.AddListener(StartNextRun);
    }

    private string BuildDeathSummary()
    {
        if (GameManager.Instance == null) return "";
        CharacterData c = GameManager.Instance.GetCharacterSnapshot();
        if (c == null) return "";
        int room = GameManager.Instance.CurrentRun != null ? GameManager.Instance.CurrentRun.roomIndex + 1 : 1;
        return $"Reached room {room}, level {c.characterLevel}.";
    }

    private string BuildRewardSummary()
    {
        if (GameManager.Instance == null) return "";
        CharacterData c = GameManager.Instance.GetCharacterSnapshot();
        if (c == null) return "";
        return $"Run cleared. Total MetaXP: {c.metaXp}";
    }

    private void StartRun()
    {
        if (GameManager.Instance != null)
        {
            int seed = (int)(System.DateTime.UtcNow.Ticks % int.MaxValue);
            GameManager.Instance.StartNewRun(seed);
        }
    }

    private void StartNextRun()
    {
        if (GameManager.Instance != null)
        {
            int seed = (int)(System.DateTime.UtcNow.Ticks % int.MaxValue);
            GameManager.Instance.StartNewRun(seed);
        }
    }

    private void GoMainMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetState(GameState.MainMenu);
        }
    }

    private void HideAll()
    {
        Hide(hudGo);
        Hide(deathScreenGo);
        Hide(rewardScreenGo);
        Hide(menuScreenGo);
    }

    private GameObject NewUi(string name)
    {
        if (canvasGo == null) BuildCommon("RuntimeUI");
        GameObject go = new GameObject(name);
        go.transform.SetParent(canvasGo.transform, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        go.AddComponent<CanvasGroup>();
        go.SetActive(true);
        return go;
    }

    private static void Hide(GameObject go)
    {
        if (go != null) go.SetActive(false);
    }

    private static void Show(GameObject go)
    {
        if (go != null) go.SetActive(true);
    }

    private static void CreateBar(Transform parent, string name, Vector2 anchoredPos, out Slider slider, out Text label, Color? color = null)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);

        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0f, 0.5f);
        rt.sizeDelta = new Vector2(360f, 28f);
        rt.anchoredPosition = anchoredPos;

        slider = go.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;
        slider.targetGraphic = null;
        slider.interactable = false;

        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(go.transform, false);
        RectTransform bgRt = bg.AddComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.offsetMin = Vector2.zero;
        bgRt.offsetMax = Vector2.zero;
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0f, 0f, 0f, 0.4f);

        GameObject fillArea = new GameObject("FillArea");
        fillArea.transform.SetParent(go.transform, false);
        RectTransform faRt = fillArea.AddComponent<RectTransform>();
        faRt.anchorMin = new Vector2(0f, 0f);
        faRt.anchorMax = new Vector2(1f, 1f);
        faRt.offsetMin = new Vector2(4f, 4f);
        faRt.offsetMax = new Vector2(-4f, -4f);

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        RectTransform fillRt = fill.AddComponent<RectTransform>();
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = Vector2.one;
        fillRt.offsetMin = Vector2.zero;
        fillRt.offsetMax = Vector2.zero;

        Image fillImg = fill.AddComponent<Image>();
        fillImg.color = color ?? new Color(0.85f, 0.2f, 0.2f, 0.85f);

        slider.fillRect = fillRt;
        slider.targetGraphic = fillImg;

        GameObject labelGo = new GameObject("Label");
        labelGo.transform.SetParent(go.transform, false);
        RectTransform lrt = labelGo.AddComponent<RectTransform>();
        lrt.anchorMin = new Vector2(1f, 0.5f);
        lrt.anchorMax = new Vector2(1f, 0.5f);
        lrt.pivot = new Vector2(0f, 0.5f);
        lrt.sizeDelta = new Vector2(120f, 28f);
        lrt.anchoredPosition = new Vector2(10f, 0f);

        label = labelGo.AddComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = 22;
        label.color = Color.white;
        label.alignment = TextAnchor.MiddleLeft;
        label.text = name.Replace("Bar", "");
    }

    private static Text CreateText(Transform parent, string name, string content, Vector2 anchoredPos, FontStyle style, int fontSize)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(600f, 80f);
        rt.anchoredPosition = anchoredPos;

        Text t = go.AddComponent<Text>();
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = fontSize;
        t.fontStyle = style;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = Color.white;
        t.text = content;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        return t;
    }

    private static GameObject CreateButton(Transform parent, string name, string content, Vector2 anchoredPos, out Button btn, float width = 200f, float height = 60f)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(width, height);
        rt.anchoredPosition = anchoredPos;

        Image img = go.AddComponent<Image>();
        img.color = new Color(0.1f, 0.1f, 0.1f, 0.85f);

        btn = go.AddComponent<Button>();
        btn.targetGraphic = img;

        GameObject label = new GameObject("Label");
        label.transform.SetParent(go.transform, false);
        RectTransform rt2 = label.AddComponent<RectTransform>();
        rt2.anchorMin = Vector2.zero;
        rt2.anchorMax = Vector2.one;
        rt2.offsetMin = Vector2.zero;
        rt2.offsetMax = Vector2.zero;

        Text text = label.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 26;
        text.alignment = TextAnchor.MiddleCenter;
        text.text = content;
        text.color = Color.white;

        return go;
    }
}
