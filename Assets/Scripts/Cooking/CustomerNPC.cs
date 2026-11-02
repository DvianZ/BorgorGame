using UnityEngine;

namespace BurgerGame
{
    public class CustomerNPC : MonoBehaviour
    {
        public static CustomerNPC Instance { get; private set; }

        [Header("Mood Sprites")]
        public Sprite happySprite;
        public Sprite neutralSprite;
        public Sprite angrySprite;

        public string currentSpeech { get; private set; } = "";
        public Sprite currentMoodSprite { get; private set; }
        public bool IsPresent { get; private set; } = false;

        private SpriteRenderer spriteRenderer;
        private Vector3 initialPosition;
        public float celebrationTimer { get; private set; } = 0f;
        public bool isHappyCelebration { get; private set; } = false;
        private float arrivalAnimTimer = 0f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            initialPosition = transform.position;

            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }

            transform.localScale = Vector3.one * 0.85f;
            spriteRenderer.sortingOrder = 25;
            spriteRenderer.enabled = false; // Hidden at start until an order arrives

            LoadSprites();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void OnEnable()
        {
            OrderManager om = OrderManager.Instance;
            if (om != null)
            {
                om.OnOrderDelivered += HandleOrderDelivered;
                om.OnOrderFailed += HandleOrderFailed;
            }
        }

        private void OnDisable()
        {
            OrderManager om = OrderManager.Instance;
            if (om != null)
            {
                om.OnOrderDelivered -= HandleOrderDelivered;
                om.OnOrderFailed -= HandleOrderFailed;
            }
        }

        private void LoadSprites()
        {
            if (happySprite == null) happySprite = Resources.Load<Sprite>("Sprites/customer_happy");
            if (neutralSprite == null) neutralSprite = Resources.Load<Sprite>("Sprites/customer_neutral");
            if (angrySprite == null) angrySprite = Resources.Load<Sprite>("Sprites/customer_angry");

            currentMoodSprite = happySprite;
            if (spriteRenderer.sprite == null)
            {
                spriteRenderer.sprite = happySprite;
            }
        }

        private static readonly string[] happyCustomerSpeeches = new string[]
        {
            "Wah lezat sekali {0}-nya! Terima kasih koki!",
            "Mmm enak banget! Koki terbaik!",
            "Rasa {0}-nya mantap jiwa! Kenyang dan puas!",
            "Burger terlezat yang pernah saya makan!",
            "Pelayanannya cepat dan rasanya luar biasa!"
        };

        private void HandleOrderDelivered(BurgerRecipe recipe, int score)
        {
            celebrationTimer = 1.6f;
            isHappyCelebration = true;
            string template = happyCustomerSpeeches[Random.Range(0, happyCustomerSpeeches.Length)];
            currentSpeech = string.Format(template, recipe != null ? recipe.recipeName : "burger");
        }

        private void HandleOrderFailed(BurgerRecipe recipe)
        {
            celebrationTimer = 1.4f;
            isHappyCelebration = false;
            currentSpeech = $"Duh kelamaan nunggu {recipe.recipeName}, saya pergi saja! 😡";
        }

        private float EaseOutBack(float t, float overshoot = 1.70158f)
        {
            t = Mathf.Clamp01(t);
            float t1 = t - 1f;
            return 1f + (overshoot + 1f) * (t1 * t1 * t1) + overshoot * (t1 * t1);
        }

        private void Update()
        {
            OrderManager om = OrderManager.Instance;

            // 1. REAKSI SELESAI / GAGAL (Celebration / leaving)
            if (celebrationTimer > 0f)
            {
                celebrationTimer -= Time.deltaTime;
                if (isHappyCelebration)
                {
                    currentMoodSprite = happySprite;
                    if (happySprite != null) spriteRenderer.sprite = happySprite;
                    float bounce = Mathf.Abs(Mathf.Sin(Time.time * 12f)) * 0.22f;
                    transform.position = initialPosition + new Vector3(0, bounce, 0);
                }
                else
                {
                    currentMoodSprite = angrySprite;
                    if (angrySprite != null) spriteRenderer.sprite = angrySprite;
                    float shake = Mathf.Sin(Time.time * 30f) * 0.06f;
                    transform.position = initialPosition + new Vector3(shake, 0, 0);
                }

                // Saat timer selebrasi selesai, jika tidak ada pesanan lagi, langsung hilangkan
                if (celebrationTimer <= 0f && (om == null || om.activeOrders.Count == 0))
                {
                    IsPresent = false;
                    if (spriteRenderer != null) spriteRenderer.enabled = false;
                    currentSpeech = "";
                }
                return;
            }

            // 2. TIDAK ADA ORDER AKTIF / MENUNGGU PELANGGAN SELANJUTNYA -> HILANGKAN KARAKTER
            if (om == null || om.currentState != GameState.Playing || om.activeOrders.Count == 0)
            {
                if (IsPresent || (spriteRenderer != null && spriteRenderer.enabled))
                {
                    IsPresent = false;
                    if (spriteRenderer != null) spriteRenderer.enabled = false;
                    currentSpeech = "";
                    transform.position = initialPosition;
                }
                return;
            }

            // 3. ADA ORDER AKTIF -> MUNCULKAN PELANGGAN BARU DENGAN ANIMASI
            if (!IsPresent || (spriteRenderer != null && !spriteRenderer.enabled))
            {
                IsPresent = true;
                if (spriteRenderer != null) spriteRenderer.enabled = true;
                arrivalAnimTimer = 0.4f;
                transform.position = initialPosition;
            }

            // Animasi kedatangan (pop-in bounce halus saat baru datang)
            if (arrivalAnimTimer > 0f)
            {
                arrivalAnimTimer -= Time.deltaTime;
                float progress = 1f - Mathf.Clamp01(arrivalAnimTimer / 0.4f);
                float scaleT = EaseOutBack(progress, 1.8f);
                transform.localScale = Vector3.one * (0.85f * scaleT);
            }
            else
            {
                transform.localScale = Vector3.one * 0.85f;
            }

            // Lihat pesanan paling mendesak (urutan pertama)
            ActiveOrder urgent = om.activeOrders[0];
            float ratio = Mathf.Clamp01(urgent.remainingTime / urgent.totalTime);

            // 4. EKSPRESI SESUAI SISA WAKTU
            if (ratio > 0.55f)
            {
                currentMoodSprite = happySprite;
                if (happySprite != null) spriteRenderer.sprite = happySprite;
                float breath = Mathf.Sin(Time.time * 3f) * 0.04f;
                transform.position = initialPosition + new Vector3(0, breath, 0);
                currentSpeech = $"{urgent.customerName}: \"Tolong siapkan {urgent.recipe.recipeName} yang hangat ya!\" 😊";
            }
            else if (ratio > 0.25f)
            {
                currentMoodSprite = neutralSprite;
                if (neutralSprite != null) spriteRenderer.sprite = neutralSprite;
                transform.position = initialPosition;
                currentSpeech = $"{urgent.customerName}: \"Sudah hampir selesai belum ya? Perut saya lapar...\" ⏳";
            }
            else
            {
                currentMoodSprite = angrySprite;
                if (angrySprite != null) spriteRenderer.sprite = angrySprite;
                float shake = Mathf.Sin(Time.time * 28f) * 0.06f;
                transform.position = initialPosition + new Vector3(shake, 0, 0);
                currentSpeech = $"{urgent.customerName}: \"Lama banget!! Saya hampir kehabisan waktu!!\" 😡";
            }
        }

        public void ResetNPC()
        {
            celebrationTimer = 0f;
            isHappyCelebration = false;
            currentSpeech = "";
            currentMoodSprite = happySprite;
            IsPresent = false;
            if (spriteRenderer != null)
            {
                if (happySprite != null) spriteRenderer.sprite = happySprite;
                spriteRenderer.enabled = false;
            }
            transform.position = initialPosition;
            transform.localScale = Vector3.one * 0.85f;
        }
    }
}
