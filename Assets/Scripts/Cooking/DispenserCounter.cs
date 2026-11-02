using UnityEngine;

namespace BurgerGame
{
    public class DispenserCounter : BaseCounter
    {
        [Header("Dispenser Setting")]
        public ItemType dispensedItem = ItemType.BunBottom;
        public Sprite itemSprite;

        private GameObject previewObject;
        private GameObject outlineHolder;
        private readonly Color pinkSalmonOutline = new Color(1.0f, 0.46f, 0.44f, 1.0f);
        private readonly System.Collections.Generic.List<SpriteRenderer> outlineRenderers = new System.Collections.Generic.List<SpriteRenderer>();

        protected override void Awake()
        {
            base.Awake();

            // Meja pengambil bahan dibuat sedikit lebih gelap sebagai pembeda
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(0.72f, 0.70f, 0.68f, 1f);
                originalColor = spriteRenderer.color;
            }

            SetupPreview();
        }

        protected override void Start()
        {
            base.Start();
            // Pastikan sorting order preview item selalu berada di atas counter
            UpdatePreviewVisual();
        }

        private void SetupPreview()
        {
            Transform existing = transform.Find("DispenserPreview");
            if (existing == null) existing = transform.Find("PreviewItem");
            if (existing != null)
            {
                previewObject = existing.gameObject;
            }
            else
            {
                previewObject = new GameObject("DispenserPreview");
                previewObject.transform.SetParent(transform);
                previewObject.AddComponent<SpriteRenderer>();
            }

            UpdatePreviewVisual();
        }

        public void UpdatePreviewVisual()
        {
            if (previewObject == null) return;

            SpriteRenderer sr = previewObject.GetComponent<SpriteRenderer>();
            if (sr == null) sr = previewObject.AddComponent<SpriteRenderer>();

            Sprite sp = itemSprite ?? ItemDatabase.GetSprite(dispensedItem);
            if (sp != null)
            {
                sr.sprite = sp;
                sr.color = Color.white;
            }
            else
            {
                sr.sprite = spriteRenderer != null ? spriteRenderer.sprite : null;
                sr.color = ItemDatabase.GetColor(dispensedItem);
            }

            // Atur sorting order agar SELALU di atas meja counter:
            // Jika meja berada di deret bawah (Y <= -1.8f) atau sortingOrder counter >= 20,
            // sorting order preview item harus 22 (di atas counter 20 dan outline 21)
            int counterOrder = (spriteRenderer != null) ? spriteRenderer.sortingOrder : 1;
            int itemOrder = (counterOrder >= 20 || transform.position.y <= -1.8f) ? 22 : Mathf.Max(6, counterOrder + 3);
            sr.sortingOrder = itemOrder;

            previewObject.transform.localPosition = new Vector3(0, 0.20f, 0);
            Vector3 baseScale = ItemDatabase.GetScale(dispensedItem) * 0.82f;
            previewObject.transform.localScale = baseScale;

            // Generate Pink Salmon Outline di belakang preview item
            SetupOutlineVisual(sp, itemOrder - 1);
            UpdateHighlightVisual(isSelected);

        // Jika item adalah Plate (Piring), buat visual tumpukan piring (stack) agar sangat jelas dan menonjol
        Transform stackTr = previewObject.transform.Find("PlateStackVisual");
        if (dispensedItem == ItemType.Plate)
        {
            GameObject stackObj = stackTr != null ? stackTr.gameObject : new GameObject("PlateStackVisual");
            stackObj.transform.SetParent(previewObject.transform);
            stackObj.transform.localPosition = new Vector3(0, 0.08f, 0);
            stackObj.transform.localScale = Vector3.one;

            SpriteRenderer stackSr = stackObj.GetComponent<SpriteRenderer>();
            if (stackSr == null) stackSr = stackObj.AddComponent<SpriteRenderer>();
            stackSr.sprite = sp;
            stackSr.color = new Color(0.95f, 0.95f, 0.95f, 1f);
            stackSr.sortingOrder = itemOrder + 1; // 23 (tumpukan atas)
        }
        else if (stackTr != null)
        {
            DestroyImmediate(stackTr.gameObject);
        }
    }

    private void SetupOutlineVisual(Sprite sp, int outlineOrder)
    {
        if (previewObject == null) return;

        Transform existingOutline = previewObject.transform.Find("OutlineHolder");
        outlineHolder = existingOutline != null ? existingOutline.gameObject : new GameObject("OutlineHolder");
        outlineHolder.name = "OutlineHolder";
        outlineHolder.transform.SetParent(previewObject.transform, false);
        outlineHolder.transform.localPosition = Vector3.zero;
        outlineHolder.transform.localScale = Vector3.one;

        // Bersihkan child lama
        for (int i = outlineHolder.transform.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
                Destroy(outlineHolder.transform.GetChild(i).gameObject);
            else
                DestroyImmediate(outlineHolder.transform.GetChild(i).gameObject);
        }
        outlineRenderers.Clear();

        if (sp == null) return;

        float dist = 0.055f;
        Vector2[] offsets = new Vector2[]
        {
            new Vector2(dist, 0f),
            new Vector2(-dist, 0f),
            new Vector2(0f, dist),
            new Vector2(0f, -dist),
            new Vector2(dist * 0.707f, dist * 0.707f),
            new Vector2(-dist * 0.707f, dist * 0.707f),
            new Vector2(dist * 0.707f, -dist * 0.707f),
            new Vector2(-dist * 0.707f, -dist * 0.707f)
        };

        for (int i = 0; i < offsets.Length; i++)
        {
            GameObject oObj = new GameObject($"Outline_{i}");
            oObj.transform.SetParent(outlineHolder.transform, false);
            oObj.transform.localPosition = new Vector3(offsets[i].x, offsets[i].y, 0f);
            oObj.transform.localScale = Vector3.one;

            SpriteRenderer oSr = oObj.AddComponent<SpriteRenderer>();
            oSr.sprite = sp;
            oSr.color = pinkSalmonOutline;
            oSr.sortingOrder = outlineOrder;
            outlineRenderers.Add(oSr);
        }

        // Outline pink salmon HANYA aktif/muncul ketika didekati (preselect)
        outlineHolder.SetActive(isSelected);
    }

    public override void SetSelected(bool selected)
    {
        base.SetSelected(selected);
        UpdateHighlightVisual(selected);
    }

    private void UpdateHighlightVisual(bool selected)
    {
        if (previewObject == null) return;

        Vector3 baseScale = ItemDatabase.GetScale(dispensedItem) * 0.85f;
        float scaleMul = selected ? 1.15f : 1.0f;
        previewObject.transform.localScale = baseScale * scaleMul;

        // Icon bahan dibuat lebih terang di atas meja gelap
        SpriteRenderer mainSr = previewObject.GetComponent<SpriteRenderer>();
        if (mainSr != null)
        {
            // Default: terang dan kontras; Selected: bersinar ekstra terang
            mainSr.color = selected ? new Color(1.25f, 1.25f, 1.25f, 1f) : new Color(1.10f, 1.10f, 1.10f, 1f);
        }

        // Munculkan outline pink salmon ketika didekati (preselect)
        if (outlineHolder != null)
        {
            outlineHolder.SetActive(selected);
        }
    }

        public void SetDispensedItem(ItemType type, Sprite sprite = null)
        {
            dispensedItem = type;
            itemSprite = sprite ?? ItemDatabase.GetSprite(type);
            UpdatePreviewVisual();
        }

        public override void Interact(PlayerController player)
        {
            if (!player.HasItem())
            {
                // Berikan item ke tangan pemain
                Sprite spriteToUse = itemSprite ?? ItemDatabase.GetSprite(dispensedItem);
                KitchenItem newItem = KitchenItem.Spawn(dispensedItem, null, spriteToUse);
                player.SetItem(newItem);
            }
            else
            {
                // If player is holding a plate, check if can add directly from dispenser
                if (player.GetItem().IsPlate())
                {
                    if (player.GetItem().TryAddIngredientToPlate(dispensedItem))
                    {
                        // Added to plate directly!
                    }
                }
            }
        }
    }
}
