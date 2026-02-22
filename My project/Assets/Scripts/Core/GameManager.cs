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

        private GameState state = GameState.MainMenu;
        private LevelData currentLevel;
        private int currentLevelId;
        private int shellsCollected, totalShells;
        private int babiesCollected, totalBabies;

        private void Start()
        {
            // Initialize UI callbacks
            mainMenuUI.Initialize(uiManager, OnPlayClicked);
            levelSelectUI.Initialize(uiManager, OnLevelSelected);
            resultScreenUI.Initialize(OnNextLevel, OnReplay, OnGoToMenu);
            creditsUI.Initialize(uiManager);

            // Show main menu
            GoToMenu();
        }

        private void Update()
        {
            if (state == GameState.Editing && Input.GetKeyDown(KeyCode.R))
            {
                RestartCurrentLevel();
            }
        }

        // --- Menu Flow ---

        public void GoToMenu()
        {
            StopAllCoroutines();
            gridManager.OnTileClicked -= OnTileClicked;
            turtle.OnCollectibleReached -= OnCollectibleReached;
            turtle.OnPathCompleted -= OnTurtleArrived;
            state = GameState.MainMenu;
            gridManager.ClearGrid();
            turtle.ResetTurtle();
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

            ConfigureCamera();
            ValidatePath();
        }

        // --- Gameplay ---

        private void OnTileClicked(TileView tileView)
        {
            if (state != GameState.Editing) return;
            if (tileView.IsAnimating) return;

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
            // 1★ = complete, 2★ = all shells, 3★ = all shells + all babies
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
            LoadLevel(currentLevelId);
        }

        // --- Camera ---

        private void ConfigureCamera()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            float aspect = (float)Screen.width / Screen.height;
            float gridHeight = gridManager.Height * 1.0f;
            float gridWidth = gridManager.Width * 1.0f;

            float verticalFit = (gridHeight + 2f) / 2f;
            float horizontalFit = (gridWidth + 1f) / (2f * aspect);

            mainCamera.orthographicSize = Mathf.Max(verticalFit, horizontalFit);
            mainCamera.transform.position = new Vector3(0, 0, -10);
            mainCamera.backgroundColor = new Color(1f, 0.933f, 0.678f); // #FFEEAD
        }
    }
}
