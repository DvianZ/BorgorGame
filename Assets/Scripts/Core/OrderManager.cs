using System.Collections.Generic;
using UnityEngine;

namespace BurgerGame
{
    public enum GameState
    {
        MainMenu,
        LevelSelect,
        RecipeBook,
        Playing,
        Paused,
        LevelComplete,
        GameOver
    }

    public enum GameLevel
    {
        Level1 = 1,
        Level2 = 2,
        Level3 = 3
    }

    [System.Serializable]
    public class BurgerRecipe
    {
        public string recipeName;
        public string subtitle;
        public int reward;
        public string badge;
        public List<ItemType> requiredIngredients;

        public BurgerRecipe(string name, string subtitle, int reward, string badge, params ItemType[] ingredients)
        {
            this.recipeName = name;
            this.subtitle = subtitle;
            this.reward = reward;
            this.badge = badge;
            this.requiredIngredients = new List<ItemType>(ingredients);
        }

        public bool Matches(List<ItemType> plateIngredients)
        {
            if (plateIngredients == null) return false;
            if (plateIngredients.Count != requiredIngredients.Count) return false;

            // Multiset equality check
            List<ItemType> remaining = new List<ItemType>(plateIngredients);
            foreach (var req in requiredIngredients)
            {
                if (!remaining.Remove(req))
                {
                    return false;
                }
            }
            return remaining.Count == 0;
        }
    }

    [System.Serializable]
    public class ActiveOrder
    {
        public BurgerRecipe recipe;
        public float totalTime = 60f;
        public float remainingTime = 60f;
        public int orderNumber = 1;
        public string customerName = "Pelanggan";

        public ActiveOrder(BurgerRecipe recipe, float time, int number, string name)
        {
            this.recipe = recipe;
            this.totalTime = time;
            this.remainingTime = time;
            this.orderNumber = number;
            this.customerName = name;
        }
    }

    [System.Serializable]
    public class LevelConfig
    {
        public GameLevel level;
        public string levelTitle = "";
        public string subtitle = "";
        public string description = "";
        public string badge = "";
        public int targetScore;
        public float duration;
        public int maxOrders;
        public float spawnInterval;
        public float patienceTime;
        public List<BurgerRecipe> recipes = new List<BurgerRecipe>();
    }

    public enum ToastType
    {
        Info,
        Success,
        Fail,
        Warning
    }

    [System.Serializable]
    public class FeedbackToast
    {
        public string message;
        public Color color;
        public float timer = 2.5f;
        public float maxTimer = 2.5f;
        public ToastType type = ToastType.Info;

        public FeedbackToast(string msg, Color col, ToastType toastType = ToastType.Info)
        {
            message = msg;
            color = col;
            type = toastType;
        }
    }

    public class OrderManager : MonoBehaviour
    {
        public static OrderManager Instance { get; private set; }

        [Header("Current Game State")]
        public GameState currentState = GameState.MainMenu;
        public GameLevel currentLevel = GameLevel.Level1;

        [Header("Round Statistics")]
        public int score = 0;
        public int deliveredCount = 0;
        public int failedCount = 0;
        public float roundTimer = 90f;
        public int starsEarned = 0;

        [Header("Active Level Configuration")]
        [System.NonSerialized] public LevelConfig activeConfig;

        [Header("Opening Transition (Layar Terbuka)")]
        public bool isOpeningTransition = false;
        public float openingTransitionTime = 0f;
        public const float OPENING_TRANSITION_DURATION = 1.4f;

        [System.NonSerialized] public List<ActiveOrder> activeOrders = new List<ActiveOrder>();
        [System.NonSerialized] public List<FeedbackToast> activeToasts = new List<FeedbackToast>();

        public event System.Action<BurgerRecipe, int> OnOrderDelivered;
        public event System.Action<BurgerRecipe> OnOrderFailed;

        private float spawnTimer = 0f;
        private int totalOrderCounter = 0;
        private Dictionary<GameLevel, LevelConfig> levelConfigs = new Dictionary<GameLevel, LevelConfig>();

        private readonly string[] customerNames = new string[]
        {
            "Prakoso", "Maharani", "Mulyono", "Meli", "Abah", "Wowo", "Samsul", "Trump", "Jeka", "H. Imas"
        };

        private readonly string[] satisfactionPhrases = new string[]
        {
            "SEMPURNA!",
            "LEZAT BANGET!",
            "MANTAP JIWA!",
            "ENAK PARAH!",
            "JUARA RASANYA!",
            "TOP BANGET!",
            "PUAS BANGET!",
            "ISTIMEWA!",
            "BIKIN NAGIH!",
            "KOKI HEBAT!"
        };

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            BuildLevelConfigs();
            currentState = GameState.MainMenu;
            Time.timeScale = 1f;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayMenuBGM();
            }
        }

        private void BuildLevelConfigs()
        {
            levelConfigs.Clear();

            // ---------------- LEVEL 1: KEDAI KAKI LIMA ----------------
            // Tempo santai: Waktu sabar panjang (95s), jeda spawn panjang (16s), maks 2 pesanan
            LevelConfig l1 = new LevelConfig
            {
                level = GameLevel.Level1,
                // levelTitle = "Kedai Kaki Lima",
                // subtitle = "Capai target omset $90 dalam 120 detik dengan burger simpel tepat waktu",
                // description = "Menu terfokus: Burger Biasa & Cheeseburger. Pahami bar kematangan daging dan jangan sampai gosong!",
                // badge = "GOAL: TARGET $90",
                targetScore = 250,
                duration = 120f,
                maxOrders = 2,
                spawnInterval = 16f,
                patienceTime = 40f
            };
            l1.recipes.Add(new BurgerRecipe(
                "Burger Klasik", "Roti + Daging Gurih + Roti", 30, "POPULER",
                ItemType.BunBottom, ItemType.PattyCooked, ItemType.BunTop
            ));
            l1.recipes.Add(new BurgerRecipe(
                "Cheeseburger", "Roti + Daging + Keju Cheddar", 40, "FAVORIT",
                ItemType.BunBottom, ItemType.PattyCooked, ItemType.Cheese, ItemType.BunTop
            ));
            levelConfigs[GameLevel.Level1] = l1;

            // ---------------- LEVEL 2: BURGER DINER ----------------
            // Tempo tenang: Waktu sabar (80s), jeda spawn (14s), maks 3 pesanan
            LevelConfig l2 = new LevelConfig
            {
                level = GameLevel.Level2,
                // levelTitle = "Burger Diner",
                // subtitle = "Capai target omset $160 dalam 150 detik dengan memotong selada & tomat",
                // description = "Pelanggan meminta sayuran potong dan Dobel Keju. Gunakan Meja Potong [F] untuk memotong selada & tomat.",
                // badge = "GOAL: TARGET $160",
                targetScore = 400,
                duration = 180f,
                maxOrders = 3,
                spawnInterval = 19f,
                patienceTime = 60f
            };
            // Termasuk menu L1
            l2.recipes.AddRange(l1.recipes);
            // Tambahan menu baru L2:
            l2.recipes.Add(new BurgerRecipe(
                "Burger Sayur Segar", "Selada Potong + Tomat Iris + Keju", 45, "VEGETARIAN",
                ItemType.BunBottom, ItemType.LettuceChopped, ItemType.TomatoSliced, ItemType.Cheese, ItemType.BunTop
            ));
            l2.recipes.Add(new BurgerRecipe(
                "Dobel Cheeseburger", "2x Daging Panggang + 2x Keju Cheddar", 65, "SUPER GURIH",
                ItemType.BunBottom, ItemType.PattyCooked, ItemType.Cheese, ItemType.PattyCooked, ItemType.Cheese, ItemType.BunTop
            ));
            levelConfigs[GameLevel.Level2] = l2;

            // ---------------- LEVEL 3: MASTER GOURMET RESTAURANT ----------------
            // Waktu sabar (70s), jeda spawn (11s), maks 4 pesanan
            LevelConfig l3 = new LevelConfig
            {
                level = GameLevel.Level3,
                // levelTitle = "Master Gourmet Chef",
                // subtitle = "Capai target omset $260 dalam 180 detik dengan semua variasi burger lengkap",
                // description = "Tantangan penuh! Pesanan resep lengkap komplit & porsi monster dengan tempo yang tetap terkendali.",
                // badge = "GOAL: TARGET $260",
                targetScore = 900,
                duration = 210f,
                maxOrders = 4,
                spawnInterval = 20f,
                patienceTime = 75f
            };
            // Termasuk seluruh menu L2
            l3.recipes.AddRange(l2.recipes);
            // Tambahan menu pamungkas L3:
            l3.recipes.Add(new BurgerRecipe(
                "Burger Spesial Komplit", "Daging + Keju + Selada + Tomat", 80, "DELUXE SUPREME",
                ItemType.BunBottom, ItemType.PattyCooked, ItemType.Cheese, ItemType.LettuceChopped, ItemType.TomatoSliced, ItemType.BunTop
            ));
            l3.recipes.Add(new BurgerRecipe(
                "Monster Meat Burger", "2x Daging + Keju + Selada Krispi", 95, "PORSI MONSTER",
                ItemType.BunBottom, ItemType.PattyCooked, ItemType.PattyCooked, ItemType.Cheese, ItemType.LettuceChopped, ItemType.BunTop
            ));
            levelConfigs[GameLevel.Level3] = l3;
        }

        public LevelConfig GetLevelConfig(GameLevel level)
        {
            if (levelConfigs.TryGetValue(level, out LevelConfig cfg)) return cfg;
            return levelConfigs[GameLevel.Level1];
        }

        public void ResetKitchen()
        {
            // 1. Reset all counters (clear held items, reset stove timers, reset cutting boards)
            BaseCounter[] counters = Object.FindObjectsByType<BaseCounter>();
            foreach (var c in counters)
            {
                if (c != null) c.ResetCounter();
            }

            // 2. Destroy any remaining loose or orphan KitchenItem instances
            KitchenItem[] remainingItems = Object.FindObjectsByType<KitchenItem>();
            foreach (var item in remainingItems)
            {
                if (item != null && item.gameObject != null)
                {
                    Destroy(item.gameObject);
                }
            }

            // 3. Reset Player position, inputs, facing, and held items
            PlayerController player = PlayerController.Instance ?? Object.FindAnyObjectByType<PlayerController>();
            if (player != null)
            {
                player.ResetPlayer();
            }

            // 4. Reset Customer NPC
            CustomerNPC npc = CustomerNPC.Instance ?? Object.FindAnyObjectByType<CustomerNPC>();
            if (npc != null)
            {
                npc.ResetNPC();
            }
        }

        public void StartLevel(GameLevel level)
        {
            currentLevel = level;
            activeConfig = GetLevelConfig(level);

            // Clean reset kitchen items, counters, player, and NPC
            ResetKitchen();

            score = 0;
            deliveredCount = 0;
            failedCount = 0;
            starsEarned = 0;
            totalOrderCounter = 0;
            roundTimer = activeConfig.duration;
            spawnTimer = 0f;

            activeOrders.Clear();
            activeToasts.Clear();

            currentState = GameState.Playing;
            Time.timeScale = 1f;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayCookingBGM();
            }

            // Trigger "Layar Terbuka" (Opening Curtain Animation)
            isOpeningTransition = true;
            openingTransitionTime = 0f;
        }

        public void RestartCurrentLevel()
        {
            StartLevel(currentLevel);
        }

        public void NextLevel()
        {
            if (currentLevel == GameLevel.Level1) StartLevel(GameLevel.Level2);
            else if (currentLevel == GameLevel.Level2) StartLevel(GameLevel.Level3);
            else StartLevel(GameLevel.Level1);
        }

        public void GoToMainMenu()
        {
            currentState = GameState.MainMenu;
            Time.timeScale = 1f;
            isOpeningTransition = false;
            activeOrders.Clear();
            ResetKitchen();

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayMenuBGM();
            }
        }

        public void GoToLevelSelect()
        {
            currentState = GameState.LevelSelect;
            Time.timeScale = 1f;
            isOpeningTransition = false;
            activeOrders.Clear();
            ResetKitchen();

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayMenuBGM();
            }
        }

        public void GoToRecipeBook()
        {
            currentState = GameState.RecipeBook;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayMenuBGM();
            }
        }

        public void TogglePause()
        {
            if (currentState == GameState.Playing)
            {
                currentState = GameState.Paused;
                Time.timeScale = 0f;
            }
            else if (currentState == GameState.Paused)
            {
                currentState = GameState.Playing;
                Time.timeScale = 1f;
            }
        }

        public void AddToast(string msg, Color col, ToastType type = ToastType.Info)
        {
            activeToasts.Add(new FeedbackToast(msg, col, type));
            if (activeToasts.Count > 4) activeToasts.RemoveAt(0);
        }

        private void Update()
        {
            // Update Toasts
            for (int i = activeToasts.Count - 1; i >= 0; i--)
            {
                activeToasts[i].timer -= Time.unscaledDeltaTime;
                if (activeToasts[i].timer <= 0f)
                {
                    activeToasts.RemoveAt(i);
                }
            }

            // Keyboard Shortcuts
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (currentState == GameState.Playing || currentState == GameState.Paused)
                {
                    TogglePause();
                }
                else if (currentState == GameState.LevelSelect || currentState == GameState.RecipeBook)
                {
                    GoToMainMenu();
                }
            }

            if (currentState != GameState.Playing) return;

            // Handle Opening Curtain Transition ("Layar Terbuka")
            if (isOpeningTransition)
            {
                openingTransitionTime += Time.unscaledDeltaTime;
                if (openingTransitionTime >= OPENING_TRANSITION_DURATION)
                {
                    isOpeningTransition = false;
                    // Setelah tirai sepenuhnya naik, baru timer berjalan dan muncul pelanggan
                    SpawnRandomOrder();
                    if (activeConfig != null)
                    {
                        string lvlName = !string.IsNullOrEmpty(activeConfig.levelTitle) ? activeConfig.levelTitle : $"Level {(int)currentLevel}";
                        AddToast($"Memulai {lvlName}! Target: ${activeConfig.targetScore}", new Color(0.2f, 0.9f, 0.4f));
                    }
                }
                // Do not tick down round timer or order decay while screen is opening
                return;
            }

            // Round Timer
            if (roundTimer > 0)
            {
                roundTimer -= Time.deltaTime;
                if (roundTimer <= 0)
                {
                    roundTimer = 0;
                    FinishLevel();
                    return;
                }
            }

            // Order Spawner
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= activeConfig.spawnInterval)
            {
                spawnTimer = 0f;
                if (activeOrders.Count < activeConfig.maxOrders)
                {
                    SpawnRandomOrder();
                }
            }

            // Active orders decay
            for (int i = activeOrders.Count - 1; i >= 0; i--)
            {
                activeOrders[i].remainingTime -= Time.deltaTime;
                if (activeOrders[i].remainingTime <= 0)
                {
                    ActiveOrder failed = activeOrders[i];
                    failedCount++;
                    score = Mathf.Max(0, score - 15);
                    activeOrders.RemoveAt(i);

                    AddToast($"{failed.customerName} KECEWA! Pesanan {failed.recipe.recipeName} Hangus (-$15)", new Color(1.0f, 0.25f, 0.25f), ToastType.Fail);
                    OnOrderFailed?.Invoke(failed.recipe);
                }
            }
        }

        private void FinishLevel()
        {
            Time.timeScale = 0f;

            // Evaluasi Bintang: Pemain WAJIB mencapai minimal targetScore untuk lulus (1 Bintang)!
            // Jika di bawah targetScore (< target), level GAGAL (0 Bintang) dan harus mengulang level.
            int target = activeConfig != null ? activeConfig.targetScore : 100;
            if (score >= Mathf.RoundToInt(target * 1.50f)) starsEarned = 3;
            else if (score >= Mathf.RoundToInt(target * 1.25f)) starsEarned = 2;
            else if (score >= target) starsEarned = 1;
            else starsEarned = 0;

            // Save High Score & Stars in PlayerPrefs
            string scoreKey = $"Burger_HighScore_Level_{(int)currentLevel}";
            string starKey = $"Burger_Stars_Level_{(int)currentLevel}";

            int prevBest = PlayerPrefs.GetInt(scoreKey, 0);
            if (score > prevBest) PlayerPrefs.SetInt(scoreKey, score);

            if (starsEarned > 0)
            {
                int prevStars = PlayerPrefs.GetInt(starKey, 0);
                if (starsEarned > prevStars) PlayerPrefs.SetInt(starKey, starsEarned);
                PlayerPrefs.Save();
                currentState = GameState.LevelComplete;
            }
            else
            {
                PlayerPrefs.Save();
                currentState = GameState.GameOver;
            }
        }

        public void SpawnRandomOrder()
        {
            if (activeConfig == null || activeConfig.recipes.Count == 0) return;

            int randomIndex = Random.Range(0, activeConfig.recipes.Count);
            BurgerRecipe recipe = activeConfig.recipes[randomIndex];

            totalOrderCounter++;
            string cName = customerNames[Random.Range(0, customerNames.Length)];
            activeOrders.Add(new ActiveOrder(recipe, activeConfig.patienceTime, totalOrderCounter, cName));

            // Mainkan suara orderan baru masuk
            AudioManager.Instance?.PlayNewOrderSFX();
        }

        public bool TryDeliverBurger(List<ItemType> plateIngredients)
        {
            if (currentState != GameState.Playing) return false;

            for (int i = 0; i < activeOrders.Count; i++)
            {
                if (activeOrders[i].recipe.Matches(plateIngredients))
                {
                    ActiveOrder delivered = activeOrders[i];
                    int baseReward = delivered.recipe.reward;

                    // Tip random 1 - 10 dolar saja
                    int tip = Random.Range(1, 11);
                    string satisfactionText = satisfactionPhrases[Random.Range(0, satisfactionPhrases.Length)];

                    int totalEarned = baseReward + tip;
                    score += totalEarned;
                    deliveredCount++;

                    activeOrders.RemoveAt(i);

                    // SFX: Suara pesanan benar / selesai disajikan!
                    AudioManager.Instance?.PlayOrderServedSFX();

                    string toastMsg = $"{delivered.customerName}: {satisfactionText} +${totalEarned} (Bonus Tip +${tip})";

                    AddToast(toastMsg, new Color(0.2f, 0.95f, 0.45f), ToastType.Success);
                    OnOrderDelivered?.Invoke(delivered.recipe, totalEarned);
                    return true;
                }
            }

            AddToast("Resep tidak cocok dengan pesanan pelanggan manapun!", new Color(1.0f, 0.75f, 0.2f), ToastType.Warning);
            return false;
        }

        public void PenalizeWaste(KitchenItem item)
        {
            if (item == null || currentState != GameState.Playing) return;

            int penalty = ItemDatabase.CalculateWasteValue(item);
            string itemName = ItemDatabase.GetItemDescription(item);

            if (penalty > 0)
            {
                score = Mathf.Max(0, score - penalty);
                AddToast($"Dibuang: {itemName} (-${penalty})", new Color(1.0f, 0.35f, 0.35f), ToastType.Fail);
            }
        }

        public int GetBestScore(GameLevel lvl)
        {
            return PlayerPrefs.GetInt($"Burger_HighScore_Level_{(int)lvl}", 0);
        }

        public int GetBestStars(GameLevel lvl)
        {
            int bestScore = GetBestScore(lvl);
            LevelConfig cfg = GetLevelConfig(lvl);
            int target = cfg != null ? cfg.targetScore : 100;
            if (bestScore < target) return 0; // Guard agar highscore di bawah target tidak menghasilkan bintang tersimpan
            return PlayerPrefs.GetInt($"Burger_Stars_Level_{(int)lvl}", 0);
        }
    }
}
