using UnityEngine;

namespace BurgerGame
{
    public class StoveCounter : BaseCounter
    {
        public enum StoveState
        {
            Idle,
            Cooking,
            Cooked,
            Burnt
        }

        [Header("Stove Config")]
        public float cookTime = 5.0f;
        public float burnTime = 5.0f;

        [Header("Current State")]
        public StoveState state = StoveState.Idle;
        public float timer = 0f;

        // Visual Progress Bar Components
        private GameObject barRoot;
        private Transform fillTransform;
        private SpriteRenderer fillRenderer;
        private SpriteRenderer bgRenderer;
        private TextMesh statusText;
        private AudioSource stoveAudioSource;

        private static Sprite barWhiteSpriteLeft;
        private static Sprite barWhiteSpriteCenter;

        private static Sprite GetBarSpriteLeft()
        {
            if (barWhiteSpriteLeft == null)
            {
                Texture2D tex = Texture2D.whiteTexture;
                barWhiteSpriteLeft = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0f, 0.5f), tex.width);
            }
            return barWhiteSpriteLeft;
        }

        private static Sprite GetBarSpriteCenter()
        {
            if (barWhiteSpriteCenter == null)
            {
                Texture2D tex = Texture2D.whiteTexture;
                barWhiteSpriteCenter = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), tex.width);
            }
            return barWhiteSpriteCenter;
        }

        protected override void Awake()
        {
            base.Awake();
            CreateDonenessBar();

            stoveAudioSource = gameObject.AddComponent<AudioSource>();
            stoveAudioSource.playOnAwake = false;
            stoveAudioSource.loop = true;
            stoveAudioSource.spatialBlend = 0f;
            stoveAudioSource.bypassEffects = true;
            stoveAudioSource.ignoreListenerPause = true;
        }

        private void CreateDonenessBar()
        {
            if (barRoot != null) return;

            barRoot = new GameObject("MeatDonenessBar");
            barRoot.transform.SetParent(transform);
            barRoot.transform.localPosition = new Vector3(0, 0.85f, 0);

            // 1. Outer Frame (Soft Pastel Border)
            GameObject bg = new GameObject("BG_Frame");
            bg.transform.SetParent(barRoot.transform);
            bg.transform.localPosition = Vector3.zero;
            bg.transform.localScale = new Vector3(1.05f, 0.18f, 1f);
            bgRenderer = bg.AddComponent<SpriteRenderer>();
            bgRenderer.sprite = GetBarSpriteCenter();
            bgRenderer.color = new Color(0.88f, 0.85f, 0.82f, 0.95f);
            bgRenderer.sortingOrder = 25;

            // 2. Inner Track (Slot - Soft Pastel Warm)
            GameObject slot = new GameObject("BG_Track");
            slot.transform.SetParent(barRoot.transform);
            slot.transform.localPosition = Vector3.zero;
            slot.transform.localScale = new Vector3(1.00f, 0.14f, 1f);
            SpriteRenderer slotRenderer = slot.AddComponent<SpriteRenderer>();
            slotRenderer.sprite = GetBarSpriteCenter();
            slotRenderer.color = new Color(0.96f, 0.94f, 0.92f, 0.95f);
            slotRenderer.sortingOrder = 26;

            // 3. Bar Fill (Anchor left, width 0.98 max)
            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(barRoot.transform);
            fill.transform.localPosition = new Vector3(-0.49f, 0, 0); // anchored left
            fill.transform.localScale = new Vector3(0f, 0.11f, 1f);
            fillRenderer = fill.AddComponent<SpriteRenderer>();
            fillRenderer.sprite = GetBarSpriteLeft();
            fillRenderer.color = new Color(1.0f, 0.78f, 0.55f, 1f);
            fillRenderer.sortingOrder = 27;
            fillTransform = fill.transform;

            // 4. Status Text Shadow (soft white shadow for legibility against world objects)
            GameObject textShadowObj = new GameObject("StatusTextShadow");
            textShadowObj.transform.SetParent(barRoot.transform);
            textShadowObj.transform.localPosition = new Vector3(0.01f, 0.23f, 0);
            TextMesh shadowText = textShadowObj.AddComponent<TextMesh>();
            shadowText.text = "MEMASAK...";
            shadowText.fontStyle = FontStyle.Bold;
            shadowText.fontSize = 24;
            shadowText.characterSize = 0.038f;
            shadowText.alignment = TextAlignment.Center;
            shadowText.anchor = TextAnchor.MiddleCenter;
            shadowText.color = new Color(1f, 1f, 1f, 0.8f);
            MeshRenderer smr = textShadowObj.GetComponent<MeshRenderer>();
            smr.sortingOrder = 29;

            // 5. Status Text Main (Hitam Bold!)
            GameObject textObj = new GameObject("StatusText");
            textObj.transform.SetParent(barRoot.transform);
            textObj.transform.localPosition = new Vector3(0, 0.24f, 0);
            statusText = textObj.AddComponent<TextMesh>();
            statusText.text = "MEMASAK...";
            statusText.fontStyle = FontStyle.Bold;
            statusText.fontSize = 24;
            statusText.characterSize = 0.038f;
            statusText.alignment = TextAlignment.Center;
            statusText.anchor = TextAnchor.MiddleCenter;
            statusText.color = new Color(0.12f, 0.12f, 0.12f, 1f);
            MeshRenderer mr = textObj.GetComponent<MeshRenderer>();
            mr.sortingOrder = 30;

            barRoot.SetActive(false);
        }

        private void Update()
        {
            if (!HasItem())
            {
                state = StoveState.Idle;
                timer = 0f;
                if (barRoot != null) barRoot.SetActive(false);
                if (stoveAudioSource != null && stoveAudioSource.isPlaying)
                {
                    stoveAudioSource.Stop();
                }
                return;
            }

            // 1. FASE DAGING MENTAH SEDANG DIMASAK
            if (currentItem.itemType == ItemType.PattyRaw)
            {
                state = StoveState.Cooking;
                timer += Time.deltaTime;
                float progress = Mathf.Clamp01(timer / cookTime);

                // Suara mendesis ketika patty sedang dimasak
                if (stoveAudioSource != null)
                {
                    AudioClip cookClip = AudioManager.Instance != null ? AudioManager.Instance.clipDimasak : null;
                    if (cookClip != null)
                    {
                        if (stoveAudioSource.clip != cookClip)
                        {
                            stoveAudioSource.clip = cookClip;
                        }

                        bool isMuted = AudioManager.Instance != null && AudioManager.Instance.isMuted;
                        stoveAudioSource.volume = isMuted ? 0f : 0.65f;

                        if (Time.timeScale > 0f)
                        {
                            if (!stoveAudioSource.isPlaying) stoveAudioSource.Play();
                        }
                        else
                        {
                            if (stoveAudioSource.isPlaying) stoveAudioSource.Pause();
                        }
                    }
                }

                // Efek mendesis visual halus pada daging
                float sizzle = 1f + Mathf.Sin(Time.time * 25f) * 0.03f;
                currentItem.transform.localScale = ItemDatabase.GetScale(ItemType.PattyRaw) * sizzle;

                // Progress Bar: Warna transisi pastel lembut dari peach ke mint saat mendekati 100%
                Color cookColor = Color.Lerp(new Color(1.0f, 0.78f, 0.55f), new Color(0.65f, 0.90f, 0.75f), progress);
                SetBar(progress, cookColor, $"MEMASAK ({Mathf.RoundToInt(progress * 100)}%)");

                if (timer >= cookTime)
                {
                    currentItem.SetItemType(ItemType.PattyCooked);
                    state = StoveState.Cooked;
                    timer = 0f;

                    // Hentikan suara mendesis memasak
                    if (stoveAudioSource != null && stoveAudioSource.isPlaying)
                    {
                        stoveAudioSource.Stop();
                    }

                    // Suara patty matang!
                    AudioManager.Instance?.PlayPattyCookedSFX();

                    // Efek letup visual saat matang sempurna
                    currentItem.transform.localScale = ItemDatabase.GetScale(ItemType.PattyCooked) * 1.30f;
                }
            }
            // 2. FASE DAGING SUDAH MATANG (HATI-HATI GOSONG!)
            else if (currentItem.itemType == ItemType.PattyCooked)
            {
                state = StoveState.Cooked;
                timer += Time.deltaTime;
                float burnProgress = Mathf.Clamp01(timer / burnTime);

                // Suara memasak berhenti karena sudah matang
                if (stoveAudioSource != null && stoveAudioSource.isPlaying)
                {
                    stoveAudioSource.Stop();
                }

                // Kembalikan scale secara halus setelah letup
                currentItem.transform.localScale = Vector3.Lerp(
                    currentItem.transform.localScale, 
                    ItemDatabase.GetScale(ItemType.PattyCooked), 
                    Time.deltaTime * 6f
                );

                if (burnProgress < 0.5f)
                {
                    // Masih matang segar & siap diangkat! Bar pastel mint penuh
                    float pulse = 0.94f + Mathf.PingPong(Time.time * 2f, 0.12f);
                    Color greenColor = new Color(0.65f * pulse, 0.92f * pulse, 0.75f * pulse);
                    SetBar(1.0f, greenColor, "✓ MATANG! [SIAP ANGKAT]");
                }
                else
                {
                    // Peringatan: mendekati gosong! Berkedip pastel rose & pastel butter
                    bool flash = Mathf.PingPong(Time.time * 8f, 1f) > 0.5f;
                    Color warningColor = flash ? new Color(1.0f, 0.65f, 0.65f) : new Color(1.0f, 0.90f, 0.60f);
                    float timeLeft = Mathf.Max(0f, burnTime - timer);
                    SetBar(burnProgress, warningColor, $"! AWAS GOSONG ({timeLeft:0.0}s) !");

                    // Getar asap saat hampir gosong
                    float smokeJitter = Mathf.Sin(Time.time * 30f) * 0.04f;
                    currentItem.transform.localPosition = new Vector3(smokeJitter, 0, 0);
                }

                if (timer >= burnTime)
                {
                    currentItem.SetItemType(ItemType.PattyBurnt);
                    state = StoveState.Burnt;
                    timer = 0f;
                    currentItem.transform.localPosition = Vector3.zero;
                }
            }
            // 3. FASE DAGING GOSONG
            else if (currentItem.itemType == ItemType.PattyBurnt)
            {
                state = StoveState.Burnt;
                SetBar(1.0f, new Color(0.65f, 0.65f, 0.68f), "X GOSONG (BUANG KE SAMPAH)");
            }
            else
            {
                state = StoveState.Idle;
                if (barRoot != null) barRoot.SetActive(false);
            }
        }

        private void SetBar(float ratio, Color color, string label)
        {
            if (barRoot == null || fillTransform == null) return;

            barRoot.SetActive(true);
            ratio = Mathf.Clamp01(ratio);

            // Update Fill Width (anchor left)
            fillTransform.localScale = new Vector3(0.98f * ratio, 0.11f, 1f);

            if (fillRenderer != null) fillRenderer.color = color;

            if (statusText != null)
            {
                statusText.text = label;
                statusText.fontStyle = FontStyle.Bold;
                statusText.color = new Color(0.12f, 0.12f, 0.12f, 1f); // Hitam bold!
            }

            // Update shadow text
            Transform shadowTr = barRoot.transform.Find("StatusTextShadow");
            if (shadowTr != null)
            {
                TextMesh st = shadowTr.GetComponent<TextMesh>();
                if (st != null)
                {
                    st.text = label;
                    st.fontStyle = FontStyle.Bold;
                }
            }
        }

        public override void Interact(PlayerController player)
        {
            if (!HasItem())
            {
                // Taruh daging mentah atau daging matang
                if (player.HasItem() && (player.GetItem().itemType == ItemType.PattyRaw || player.GetItem().itemType == ItemType.PattyCooked))
                {
                    SetItem(player.TakeItem());
                    timer = 0f;
                }
            }
            else
            {
                if (!player.HasItem())
                {
                    // Ambil dengan tangan kosong
                    player.SetItem(TakeItem());
                    timer = 0f;
                }
                else if (player.GetItem().IsPlate())
                {
                    // Ambil langsung dengan Piring!
                    if (player.GetItem().TryAddIngredientToPlate(currentItem.itemType))
                    {
                        Destroy(TakeItem().gameObject);
                        timer = 0f;
                    }
                }
            }
        }

        public override void ResetCounter()
        {
            base.ResetCounter();
            state = StoveState.Idle;
            timer = 0f;
            if (barRoot != null) barRoot.SetActive(false);
            if (stoveAudioSource != null && stoveAudioSource.isPlaying)
            {
                stoveAudioSource.Stop();
            }
        }

        private void OnDisable()
        {
            if (stoveAudioSource != null && stoveAudioSource.isPlaying)
            {
                stoveAudioSource.Stop();
            }
        }
    }
}
