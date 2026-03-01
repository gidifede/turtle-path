using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurtlePath.Grid;
using TurtlePath.Level;
using TurtlePath.Path;
using TurtlePath.Tiles;
using TurtlePath.Turtle;
using TurtlePath.Collectibles;
using TurtlePath.Save;
using TurtlePath.UI;

namespace TurtlePath.Core
{
    public class GameManager : MonoBehaviour
    {
        [Header("References")]
        public GridManager gridManager;
        public TurtleController turtle;
        public Camera mainCamera;

        [Header("UI")]
        public UIManager uiManager;
        public MainMenuUI mainMenuUI;
        public LevelSelectUI levelSelectUI;
        public GameplayUI gameplayUI;
        public ResultScreenUI resultScreenUI;
        public CreditsUI creditsUI;
        public TileInventoryUI inventoryUI;
        public PauseMenuUI pauseMenuUI;

        private GameState state = GameState.MainMenu;
        private LevelData currentLevel;
        private int currentLevelId;
        private int shellsCollected, totalShells;
        private int babiesCollected, totalBabies;
        private bool hasInventory;

        private void Start()
        {
            // Initialize UI callbacks
            mainMenuUI.Initialize(uiManager, OnPlayClicked);
            levelSelectUI.Initialize(uiManager, OnLevelSelected);
            resultScreenUI.Initialize(OnNextLevel, OnReplay, OnGoToMenu);
            creditsUI.Initialize(uiManager);
            if (pauseMenuUI != null)
                pauseMenuUI.Initialize(ResumeGame, OnPauseRestart, GoToMenu);
            if (gameplayUI.pauseButton != null)
                gameplayUI.pauseButton.onClick.AddListener(PauseGame);

            // Show main menu
            GoToMenu();
        }

        private void Update()
        {
            if (state == GameState.Editing)
            {
                if (Input.GetKeyDown(KeyCode.R))
                    RestartCurrentLevel();
                if (Input.GetKeyDown(KeyCode.Escape))
                    TogglePause();
            }
        }

        // --- Menu Flow ---

        public void GoToMenu()
        {
            Time.timeScale = 1f;
            StopAllCoroutines();
            gridManager.OnTileClicked -= OnTileClicked;
            turtle.OnCollectibleReached -= OnCollectibleReached;
            turtle.OnPathCompleted -= OnTurtleArrived;
            state = GameState.MainMenu;
            gridManager.ClearGrid();
            turtle.ResetTurtle();
            HideInventory();
            mainMenuUI.Refresh();
            uiManager.ShowPanel("MainMenu");
        }

        private void OnPlayClicked()
        {
            state = GameState.LevelSelect;
            levelSelectUI.RefreshButtons();
            uiManager.ShowPanel("LevelSelect");
        }

        private void OnLevelSelected(int levelId)
        {
            LoadLevel(levelId);
        }

        // --- Level Loading ---

        public void LoadLevel(int levelId)
        {
            // Cleanup previous
            StopAllCoroutines();
            gridManager.OnTileClicked -= OnTileClicked;
            turtle.OnCollectibleReached -= OnCollectibleReached;
            turtle.OnPathCompleted -= OnTurtleArrived;
            gridManager.ClearGrid();
            turtle.ResetTurtle();
            HideInventory();

            currentLevelId = levelId;
            state = GameState.Editing;

            currentLevel = LevelLoader.LoadLevel(levelId);
            if (currentLevel == null)
            {
                Debug.LogError($"Failed to load level {levelId}");
                GoToMenu();
                return;
            }

            LevelLoader.RandomizeTileRotations(currentLevel);

            // Count collectibles
            totalShells = 0;
            totalBabies = 0;
            shellsCollected = 0;
            babiesCollected = 0;

            if (currentLevel.collectibles != null)
            {
                for (int i = 0; i < currentLevel.collectibles.Length; i++)
                {
                    if (currentLevel.collectibles[i].type == CollectibleType.Shell)
                        totalShells++;
                    else if (currentLevel.collectibles[i].type == CollectibleType.BabyTurtle)
                        totalBabies++;
                }
            }

            gridManager.BuildGrid(currentLevel);
            gridManager.OnTileClicked += OnTileClicked;

            turtle.Initialize(gridManager);
            turtle.OnCollectibleReached += OnCollectibleReached;

            // UI
            uiManager.ShowPanel("Gameplay");
            gameplayUI.ResetCounters(totalShells, totalBabies);

            // Inventory
            hasInventory = currentLevel.inventory != null && currentLevel.inventory.Length > 0;
            if (hasInventory)
                ShowInventory();

            ConfigureCamera();
            ValidatePath();
        }

        // --- Inventory ---

        private void ShowInventory()
        {
            if (inventoryUI == null) return;
            inventoryUI.gameObject.SetActive(true);
            inventoryUI.Setup(currentLevel.inventory, gridManager, mainCamera);
            inventoryUI.OnInventoryChanged += OnInventoryChanged;
        }

        private void HideInventory()
        {
            if (inventoryUI == null) return;
            inventoryUI.OnInventoryChanged -= OnInventoryChanged;
            inventoryUI.Clear();
            inventoryUI.gameObject.SetActive(false);
            hasInventory = false;
        }

        private void OnInventoryChanged()
        {
            StartCoroutine(ValidateAfterFrame());
        }

        // --- Gameplay ---

        private void OnTileClicked(TileView tileView)
        {
            if (state != GameState.Editing) return;
            if (tileView.IsAnimating) return;

            // If tile is from inventory, remove it on click (return to inventory)
            if (tileView.FromInventory)
            {
                Vector2Int pos = tileView.Cell.GridPosition;
                TileType? removed = gridManager.RemoveInventoryTile(pos);
                if (removed.HasValue && inventoryUI != null)
                {
                    inventoryUI.ReturnTile(removed.Value);
                }
                StartCoroutine(ValidateAfterFrame());
                return;
            }

            tileView.AnimateRotation();
            StartCoroutine(ValidateAfterFrame());
        }

        private IEnumerator ValidateAfterFrame()
        {
            yield return null;
            ValidatePath();
        }

        private void ValidatePath()
        {
            if (state != GameState.Editing) return;

            HashSet<Vector2Int> connected = PathValidator.FloodFillFromNest(
                gridManager, currentLevel.nestPos);

            gridManager.UpdateCellVisuals(connected);

            if (connected.Contains(currentLevel.seaPos))
            {
                List<Vector2Int> path = PathValidator.FindPath(
                    gridManager, currentLevel.nestPos, currentLevel.seaPos);

                if (path != null)
                {
                    state = GameState.Completed;
                    StartCoroutine(StartTurtleWithDelay(path));
                }
            }
        }

        private IEnumerator StartTurtleWithDelay(List<Vector2Int> path)
        {
            yield return new WaitForSeconds(0.5f);

            state = GameState.Animating;
            turtle.OnPathCompleted += OnTurtleArrived;
            turtle.MoveAlongPath(path);
        }

        // --- Collectibles ---

        private void OnCollectibleReached(CollectibleType type, Vector2Int position)
        {
            if (type == CollectibleType.Shell)
                shellsCollected++;
            else if (type == CollectibleType.BabyTurtle)
                babiesCollected++;

            gameplayUI.UpdateCounters(shellsCollected, totalShells, babiesCollected, totalBabies);
        }

        // --- Completion ---

        private void OnTurtleArrived()
        {
            turtle.OnPathCompleted -= OnTurtleArrived;
            turtle.OnCollectibleReached -= OnCollectibleReached;

            int stars = CalculateStars();

            // Save progress
            SaveManager.SetStars(currentLevelId, stars);
            SaveManager.AddBabyTurtles(babiesCollected);

            // Unlock next level
            int nextId = currentLevelId + 1;
            if (nextId <= LevelLoader.GetTotalLevels())
                SaveManager.UnlockLevel(nextId);

            // Show result
            state = GameState.Result;
            bool hasNext = currentLevelId < LevelLoader.GetTotalLevels();
            uiManager.ShowPanel("Result");
            resultScreenUI.Show(stars, shellsCollected, totalShells, babiesCollected, totalBabies, hasNext);
        }

        public static int CalculateStars(int shellsCollected, int totalShells, int babiesCollected, int totalBabies)
        {
            // 1 star = complete, 2 stars = all shells, 3 stars = all shells + all babies
            if (shellsCollected >= totalShells && babiesCollected >= totalBabies)
                return 3;
            if (shellsCollected >= totalShells)
                return 2;
            return 1;
        }

        private int CalculateStars()
        {
            return CalculateStars(shellsCollected, totalShells, babiesCollected, totalBabies);
        }

        // --- Result Actions ---

        private void OnNextLevel()
        {
            int nextId = currentLevelId + 1;
            if (nextId <= LevelLoader.GetTotalLevels())
                LoadLevel(nextId);
        }

        private void OnReplay()
        {
            RestartCurrentLevel();
        }

        private void OnGoToMenu()
        {
            GoToMenu();
        }

        public void RestartCurrentLevel()
        {
            StopAllCoroutines();
            gridManager.OnTileClicked -= OnTileClicked;
            turtle.OnCollectibleReached -= OnCollectibleReached;
            turtle.OnPathCompleted -= OnTurtleArrived;
            gridManager.ClearGrid();
            turtle.ResetTurtle();
            HideInventory();
            LoadLevel(currentLevelId);
        }

        // --- Pause ---

        private void TogglePause()
        {
            if (uiManager.IsPanelActive("Pause"))
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        public void PauseGame()
        {
            if (state != GameState.Editing) return;
            Time.timeScale = 0f;
            uiManager.ShowPanel("Pause");
        }

        public void ResumeGame()
        {
            Time.timeScale = 1f;
            uiManager.ShowPanel("Gameplay");
            if (hasInventory && inventoryUI != null)
                inventoryUI.gameObject.SetActive(true);
        }

        private void OnPauseRestart()
        {
            Time.timeScale = 1f;
            RestartCurrentLevel();
        }

        // --- Camera ---

        private void ConfigureCamera()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            float aspect = (float)Screen.width / Screen.height;
            float gridHeight = gridManager.Height * 1.0f;
            float gridWidth = gridManager.Width * 1.0f;

            // Extra vertical margin when inventory is present (80px bar at bottom)
            float extraMargin = hasInventory ? 1.0f : 0f;

            float verticalFit = (gridHeight + 2f + extraMargin) / 2f;
            float horizontalFit = (gridWidth + 1f) / (2f * aspect);

            mainCamera.orthographicSize = Mathf.Max(verticalFit, horizontalFit);

            // Offset camera up slightly when inventory is present to center the grid above the bar
            float yOffset = hasInventory ? extraMargin / 2f : 0f;
            mainCamera.transform.position = new Vector3(0, yOffset, -10);
            mainCamera.backgroundColor = new Color(0.529f, 0.808f, 0.922f); // Sky Blue #87CEEB
        }
    }
}
