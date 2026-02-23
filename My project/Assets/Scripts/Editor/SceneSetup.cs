using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using TurtlePath.Core;
using TurtlePath.Grid;
using TurtlePath.Turtle;
using TurtlePath.UI;

public class SceneSetup : EditorWindow
{
    // Colors from specs
    private static readonly Color SkyBlue = new Color(0.529f, 0.808f, 0.922f);      // #87CEEB
    private static readonly Color OceanTeal = new Color(0.59f, 0.81f, 0.71f);       // #96CEB4
    private static readonly Color SandWarm = new Color(1f, 0.933f, 0.678f);          // #FFEEAD
    private static readonly Color UIDark = new Color(0.173f, 0.243f, 0.314f);        // #2C3E50
    private static readonly Color BGOffWhite = new Color(0.922f, 0.898f, 0.882f);    // #EBE5E1
    private static readonly Color StarGold = new Color(1f, 0.843f, 0f);              // #FFD700

    [MenuItem("Turtle Path/Setup Game Scene")]
    public static void SetupScene()
    {
        // Clean up existing objects
        DestroyExisting("GameManager");
        DestroyExisting("GridManager");
        DestroyExisting("Turtle");
        DestroyExisting("Canvas");
        DestroyExisting("UIManager");
        DestroyExisting("EventSystem");

        // --- EventSystem (required for UI interaction) ---
        GameObject eventSystemGO = new GameObject("EventSystem");
        eventSystemGO.AddComponent<EventSystem>();
        eventSystemGO.AddComponent<StandaloneInputModule>();

        // --- Main Camera ---
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.orthographic = true;
            cam.transform.position = new Vector3(0, 0, -10);
            cam.backgroundColor = SkyBlue;
            cam.orthographicSize = 5;
        }

        // --- Find Square sprite ---
        Sprite squareSprite = FindSquareSprite();
        if (squareSprite == null)
        {
            Debug.LogError("Could not find Square sprite. Creating fallback.");
            squareSprite = CreateFallbackSprite();
        }

        // --- Load M2-Bis sprites (null if files don't exist yet) ---
        Sprite tileSprite_straight = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Tiles/tile_straight.png");
        Sprite tileSprite_curve = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Tiles/tile_curve.png");
        Sprite cellSprite_nest = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Cells/cell_nest.png");
        Sprite cellSprite_sea = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Cells/cell_sea.png");
        Sprite collectibleSprite_shell = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Collectibles/collectible_shell.png");
        Sprite collectibleSprite_baby = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Collectibles/collectible_baby.png");
        Sprite turtleSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Characters/turtle.png");

        // --- GridManager ---
        GameObject gridManagerGO = new GameObject("GridManager");
        GridManager gridManager = gridManagerGO.AddComponent<GridManager>();
        gridManager.cellSprite = squareSprite;
        gridManager.cellSprite_nest = cellSprite_nest;
        gridManager.cellSprite_sea = cellSprite_sea;
        gridManager.tileSprite_straight = tileSprite_straight;
        gridManager.tileSprite_curve = tileSprite_curve;
        gridManager.collectibleSprite_shell = collectibleSprite_shell;
        gridManager.collectibleSprite_baby = collectibleSprite_baby;

        // --- Turtle ---
        GameObject turtleGO = new GameObject("Turtle");
        TurtleController turtleController = turtleGO.AddComponent<TurtleController>();
        SpriteRenderer turtleSR = turtleGO.AddComponent<SpriteRenderer>();
        turtleSR.sprite = turtleSprite ?? squareSprite;
        turtleSR.color = turtleSprite != null ? Color.white : new Color(0.2f, 0.7f, 0.3f);
        turtleSR.sortingOrder = 10;
        turtleGO.transform.localScale = Vector3.one * 0.6f;
        turtleGO.SetActive(false);
        turtleController.SetFollowerSprite(collectibleSprite_baby ?? squareSprite);

        // --- Canvas ---
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        canvasGO.AddComponent<GraphicRaycaster>();

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(450, 800);
        scaler.matchWidthOrHeight = 0.5f;

        // --- Create all panels ---
        GameObject mainMenuPanel = CreateMainMenuPanel(canvasGO.transform);
        GameObject levelSelectPanel = CreateLevelSelectPanel(canvasGO.transform);
        GameObject gameplayPanel = CreateGameplayPanel(canvasGO.transform);
        GameObject resultPanel = CreateResultPanel(canvasGO.transform);
        GameObject creditsPanel = CreateCreditsPanel(canvasGO.transform);

        // Set initial states
        mainMenuPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
        gameplayPanel.SetActive(false);
        resultPanel.SetActive(false);
        creditsPanel.SetActive(false);

        // --- UIManager ---
        GameObject uiManagerGO = new GameObject("UIManager");
        UIManager uiManager = uiManagerGO.AddComponent<UIManager>();
        uiManager.mainMenuPanel = mainMenuPanel;
        uiManager.levelSelectPanel = levelSelectPanel;
        uiManager.gameplayPanel = gameplayPanel;
        uiManager.resultPanel = resultPanel;
        uiManager.creditsPanel = creditsPanel;

        // --- GameManager ---
        GameObject gameManagerGO = new GameObject("GameManager");
        GameManager gameManager = gameManagerGO.AddComponent<GameManager>();
        gameManager.gridManager = gridManager;
        gameManager.turtle = turtleController;
        gameManager.mainCamera = cam;
        gameManager.uiManager = uiManager;
        gameManager.mainMenuUI = mainMenuPanel.GetComponent<MainMenuUI>();
        gameManager.levelSelectUI = levelSelectPanel.GetComponent<LevelSelectUI>();
        gameManager.gameplayUI = gameplayPanel.GetComponent<GameplayUI>();
        gameManager.resultScreenUI = resultPanel.GetComponent<ResultScreenUI>();
        gameManager.creditsUI = creditsPanel.GetComponent<CreditsUI>();

        // --- Mark dirty ---
        EditorUtility.SetDirty(gameManagerGO);
        EditorUtility.SetDirty(gridManagerGO);
        EditorUtility.SetDirty(turtleGO);
        EditorUtility.SetDirty(canvasGO);
        EditorUtility.SetDirty(uiManagerGO);
        EditorUtility.SetDirty(eventSystemGO);
        if (cam != null) EditorUtility.SetDirty(cam.gameObject);

        Debug.Log("Turtle Path M2: Scene setup completed! Press Play to test.");
    }

    // ========================
    // MAIN MENU PANEL
    // ========================
    private static GameObject CreateMainMenuPanel(Transform parent)
    {
        GameObject panel = CreatePanel("MainMenuPanel", parent, SkyBlue);

        // Title
        GameObject titleGO = CreateText("Title", panel.transform, "Turtle Path",
            new Vector2(0.1f, 0.65f), new Vector2(0.9f, 0.85f), 48, UIDark, TextAlignmentOptions.Center);

        // Turtle count
        GameObject turtleCountGO = CreateText("TurtleCount", panel.transform, "Tartarughe salvate: 0",
            new Vector2(0.1f, 0.55f), new Vector2(0.9f, 0.65f), 24, UIDark, TextAlignmentOptions.Center);

        // Play button
        GameObject playBtnGO = CreateButton("PlayButton", panel.transform, "Gioca",
            new Vector2(0.2f, 0.35f), new Vector2(0.8f, 0.48f), OceanTeal);

        // Credits button
        GameObject creditsBtnGO = CreateButton("CreditsButton", panel.transform, "Credits",
            new Vector2(0.3f, 0.2f), new Vector2(0.7f, 0.3f), SandWarm);

        // Add MainMenuUI component
        MainMenuUI mainMenuUI = panel.AddComponent<MainMenuUI>();
        mainMenuUI.titleText = titleGO.GetComponent<TextMeshProUGUI>();
        mainMenuUI.turtleCountText = turtleCountGO.GetComponent<TextMeshProUGUI>();
        mainMenuUI.playButton = playBtnGO.GetComponent<Button>();
        mainMenuUI.creditsButton = creditsBtnGO.GetComponent<Button>();

        return panel;
    }

    // ========================
    // LEVEL SELECT PANEL
    // ========================
    private static GameObject CreateLevelSelectPanel(Transform parent)
    {
        GameObject panel = CreatePanel("LevelSelectPanel", parent, SkyBlue);

        // Title
        CreateText("Title", panel.transform, "Scegli livello",
            new Vector2(0.1f, 0.8f), new Vector2(0.9f, 0.92f), 36, UIDark, TextAlignmentOptions.Center);

        // 4 level buttons in a 2x2 grid
        Button[] levelButtons = new Button[4];
        TextMeshProUGUI[] levelTexts = new TextMeshProUGUI[4];

        float startX = 0.1f;
        float btnWidth = 0.35f;
        float gapX = 0.1f;
        float startY = 0.45f;
        float btnHeight = 0.15f;
        float gapY = 0.05f;

        for (int i = 0; i < 4; i++)
        {
            int col = i % 2;
            int row = i / 2;

            float x0 = startX + col * (btnWidth + gapX);
            float x1 = x0 + btnWidth;
            float y1 = startY + (1 - row) * (btnHeight + gapY) + btnHeight;
            float y0 = y1 - btnHeight;

            string btnName = $"LevelButton_{i + 1}";
            GameObject btnGO = CreateButton(btnName, panel.transform, $"{i + 1}",
                new Vector2(x0, y0), new Vector2(x1, y1), OceanTeal);

            levelButtons[i] = btnGO.GetComponent<Button>();
            levelTexts[i] = btnGO.GetComponentInChildren<TextMeshProUGUI>();
        }

        // Back button
        GameObject backBtnGO = CreateButton("BackButton", panel.transform, "Indietro",
            new Vector2(0.25f, 0.08f), new Vector2(0.75f, 0.18f), SandWarm);

        // Add LevelSelectUI component
        LevelSelectUI levelSelectUI = panel.AddComponent<LevelSelectUI>();
        levelSelectUI.levelButtons = levelButtons;
        levelSelectUI.levelTexts = levelTexts;
        levelSelectUI.backButton = backBtnGO.GetComponent<Button>();

        return panel;
    }

    // ========================
    // GAMEPLAY PANEL
    // ========================
    private static GameObject CreateGameplayPanel(Transform parent)
    {
        GameObject panel = CreatePanel("GameplayPanel", parent, Color.clear);

        // Make the panel non-blocking for clicks (no Image component on panel itself)
        Image panelImage = panel.GetComponent<Image>();
        if (panelImage != null)
        {
            panelImage.raycastTarget = false;
            panelImage.color = Color.clear;
        }

        // Shell counter (top left)
        GameObject shellGO = CreateText("ShellCounter", panel.transform, "Conchiglie: 0/0",
            new Vector2(0.02f, 0.92f), new Vector2(0.5f, 0.98f), 22, UIDark, TextAlignmentOptions.Left);

        // Baby counter (top right)
        GameObject babyGO = CreateText("BabyCounter", panel.transform, "Baby: 0/0",
            new Vector2(0.5f, 0.92f), new Vector2(0.98f, 0.98f), 22, UIDark, TextAlignmentOptions.Right);

        // Add GameplayUI component
        GameplayUI gameplayUI = panel.AddComponent<GameplayUI>();
        gameplayUI.shellCounterText = shellGO.GetComponent<TextMeshProUGUI>();
        gameplayUI.babyCounterText = babyGO.GetComponent<TextMeshProUGUI>();

        return panel;
    }

    // ========================
    // RESULT PANEL
    // ========================
    private static GameObject CreateResultPanel(Transform parent)
    {
        GameObject panel = CreatePanel("ResultPanel", parent, BGOffWhite);

        // Title
        CreateText("ResultTitle", panel.transform, "Livello completato!",
            new Vector2(0.1f, 0.78f), new Vector2(0.9f, 0.9f), 36, UIDark, TextAlignmentOptions.Center);

        // Stars
        GameObject starsGO = CreateText("Stars", panel.transform, "\u2605\u2605\u2605",
            new Vector2(0.1f, 0.65f), new Vector2(0.9f, 0.78f), 48, StarGold, TextAlignmentOptions.Center);

        // Shells stat
        GameObject shellsGO = CreateText("ShellsStat", panel.transform, "Conchiglie: 0/0",
            new Vector2(0.1f, 0.55f), new Vector2(0.9f, 0.65f), 24, UIDark, TextAlignmentOptions.Center);

        // Babies stat
        GameObject babiesGO = CreateText("BabiesStat", panel.transform, "Baby turtle: 0/0",
            new Vector2(0.1f, 0.47f), new Vector2(0.9f, 0.55f), 24, UIDark, TextAlignmentOptions.Center);

        // Next level button
        GameObject nextBtnGO = CreateButton("NextButton", panel.transform, "Prossimo livello",
            new Vector2(0.15f, 0.3f), new Vector2(0.85f, 0.42f), OceanTeal);

        // Replay button
        GameObject replayBtnGO = CreateButton("ReplayButton", panel.transform, "Rigioca",
            new Vector2(0.15f, 0.17f), new Vector2(0.85f, 0.27f), SandWarm);

        // Menu button
        GameObject menuBtnGO = CreateButton("MenuButton", panel.transform, "Menu",
            new Vector2(0.25f, 0.05f), new Vector2(0.75f, 0.14f), new Color(0.8f, 0.8f, 0.8f));

        // Add ResultScreenUI component
        ResultScreenUI resultUI = panel.AddComponent<ResultScreenUI>();
        resultUI.starsText = starsGO.GetComponent<TextMeshProUGUI>();
        resultUI.shellsText = shellsGO.GetComponent<TextMeshProUGUI>();
        resultUI.babiesText = babiesGO.GetComponent<TextMeshProUGUI>();
        resultUI.nextButton = nextBtnGO.GetComponent<Button>();
        resultUI.replayButton = replayBtnGO.GetComponent<Button>();
        resultUI.menuButton = menuBtnGO.GetComponent<Button>();

        return panel;
    }

    // ========================
    // CREDITS PANEL
    // ========================
    private static GameObject CreateCreditsPanel(Transform parent)
    {
        GameObject panel = CreatePanel("CreditsPanel", parent, BGOffWhite);

        // Title
        CreateText("CreditsTitle", panel.transform, "Credits",
            new Vector2(0.1f, 0.82f), new Vector2(0.9f, 0.92f), 36, UIDark, TextAlignmentOptions.Center);

        // Credits text
        string credits =
            "Turtle Path\n\n" +
            "Tile Sprites: Kenney (CC0)\n" +
            "kenney.nl\n\n" +
            "Beach Tileset: ZRPG (CC-BY-SA)\n" +
            "OpenGameArt.org\n\n" +
            "UI Elements: CraftPix\n" +
            "(royalty-free)\n\n" +
            "Sounds: Kenney Interface Sounds\n" +
            "(CC0)\n\n" +
            "Music: Incompetech\n\n" +
            "Made with Unity";

        CreateText("CreditsBody", panel.transform, credits,
            new Vector2(0.08f, 0.2f), new Vector2(0.92f, 0.8f), 18, UIDark, TextAlignmentOptions.Center);

        // Back button
        GameObject backBtnGO = CreateButton("BackButton", panel.transform, "Indietro",
            new Vector2(0.25f, 0.05f), new Vector2(0.75f, 0.15f), SandWarm);

        // Add CreditsUI component
        CreditsUI creditsUI = panel.AddComponent<CreditsUI>();
        creditsUI.backButton = backBtnGO.GetComponent<Button>();

        return panel;
    }

    // ========================
    // HELPER METHODS
    // ========================

    private static GameObject CreatePanel(string name, Transform parent, Color bgColor)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image image = panel.AddComponent<Image>();
        image.color = bgColor;

        return panel;
    }

    private static GameObject CreateText(string name, Transform parent, string text,
        Vector2 anchorMin, Vector2 anchorMax, int fontSize, Color color, TextAlignmentOptions alignment)
    {
        GameObject textGO = new GameObject(name);
        textGO.transform.SetParent(parent, false);

        RectTransform rect = textGO.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = alignment;
        tmp.enableWordWrapping = true;
        tmp.overflowMode = TextOverflowModes.Ellipsis;

        return textGO;
    }

    private static GameObject CreateButton(string name, Transform parent, string label,
        Vector2 anchorMin, Vector2 anchorMax, Color bgColor)
    {
        GameObject btnGO = new GameObject(name);
        btnGO.transform.SetParent(parent, false);

        RectTransform rect = btnGO.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image image = btnGO.AddComponent<Image>();
        image.color = bgColor;

        Button button = btnGO.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = bgColor;
        colors.highlightedColor = bgColor * 1.1f;
        colors.pressedColor = bgColor * 0.9f;
        colors.disabledColor = new Color(0.6f, 0.6f, 0.6f);
        button.colors = colors;

        // Button label
        GameObject labelGO = new GameObject("Label");
        labelGO.transform.SetParent(btnGO.transform, false);

        RectTransform labelRect = labelGO.AddComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        TextMeshProUGUI tmp = labelGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 24;
        tmp.color = UIDark;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = false;

        return btnGO;
    }

    private static Sprite FindSquareSprite()
    {
        Sprite[] allSprites = Resources.FindObjectsOfTypeAll<Sprite>();
        foreach (Sprite s in allSprites)
        {
            if (s.name == "Square")
                return s;
        }

        Sprite square = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        if (square != null) return square;

        return null;
    }

    private static Sprite CreateFallbackSprite()
    {
        Texture2D tex = new Texture2D(4, 4);
        Color[] colors = new Color[16];
        for (int i = 0; i < 16; i++) colors[i] = Color.white;
        tex.SetPixels(colors);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4);
    }

    private static void DestroyExisting(string name)
    {
        GameObject existing = GameObject.Find(name);
        if (existing != null)
            DestroyImmediate(existing);
    }
}
