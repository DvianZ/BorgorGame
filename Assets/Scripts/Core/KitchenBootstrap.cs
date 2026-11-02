using UnityEngine;

namespace BurgerGame
{
    public class KitchenBootstrap : MonoBehaviour
    {
        [Header("Sprites Reference")]
        public Sprite counterSprite;
        public Sprite grillSprite;
        public Sprite cuttingSprite;
        public Sprite deliverySprite;
        public Sprite trashSprite;
        public Sprite plateSprite;
        public Sprite playerSprite;
        public Sprite floorSprite;
        public Sprite whiteBoxSprite;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AutoInit()
        {
            if (FindAnyObjectByType<KitchenBootstrap>() == null)
            {
                GameObject go = new GameObject("KitchenBootstrap");
                go.AddComponent<KitchenBootstrap>();
            }
        }

        private void Awake()
        {
            LoadSprites();
            BuildKitchenIfEmpty();
        }

        private void LoadSprites()
        {
            if (counterSprite == null) counterSprite = Resources.Load<Sprite>("Sprites/box_counter") ?? LoadAssetSprite("box_counter");
            if (grillSprite == null) grillSprite = Resources.Load<Sprite>("Sprites/box_grill") ?? LoadAssetSprite("box_grill");
            if (cuttingSprite == null) cuttingSprite = Resources.Load<Sprite>("Sprites/box_cutting") ?? LoadAssetSprite("box_cutting");
            if (deliverySprite == null) deliverySprite = Resources.Load<Sprite>("Sprites/box_delivery") ?? LoadAssetSprite("box_delivery");
            if (trashSprite == null) trashSprite = Resources.Load<Sprite>("Sprites/box_trash") ?? LoadAssetSprite("box_trash");
            if (plateSprite == null) plateSprite = Resources.Load<Sprite>("Sprites/box_plate") ?? LoadAssetSprite("box_plate");
            if (playerSprite == null) playerSprite = Resources.Load<Sprite>("Sprites/Character/chef_idle_down") ?? Resources.Load<Sprite>("Sprites/box_player") ?? LoadAssetSprite("Character/chef_idle_down") ?? LoadAssetSprite("box_player");
            if (floorSprite == null) floorSprite = Resources.Load<Sprite>("Sprites/tile_floor") ?? LoadAssetSprite("tile_floor");
            if (whiteBoxSprite == null) whiteBoxSprite = Resources.Load<Sprite>("Sprites/box_white") ?? LoadAssetSprite("box_white");
        }

        private Sprite LoadAssetSprite(string name)
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Sprites/{name}.png");
#else
            return null;
#endif
        }

        private void BuildKitchenIfEmpty()
        {
            // 1. Camera Setup
            Camera cam = Camera.main;
            if (cam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                cam = camObj.AddComponent<Camera>();
                camObj.tag = "MainCamera";
            }
            cam.orthographic = true;
            cam.orthographicSize = 4.6f; // Generous clearance for top HUD order cards and bottom mobile controls
            cam.transform.position = new Vector3(0.0f, -0.9f, -10f); // Centered on shifted kitchen
            cam.backgroundColor = new Color(0.18f, 0.22f, 0.28f);
            cam.clearFlags = CameraClearFlags.SolidColor;

            if (cam.GetComponent<FixedAspectRatio>() == null)
            {
                cam.gameObject.AddComponent<FixedAspectRatio>();
            }

            // 2. Ensure OrderManager & GameHUD
            if (FindAnyObjectByType<OrderManager>() == null)
            {
                GameObject om = new GameObject("OrderManager");
                om.AddComponent<OrderManager>();
            }

            if (FindAnyObjectByType<GameHUD>() == null)
            {
                GameObject hud = new GameObject("GameHUD");
                hud.AddComponent<GameHUD>();
            }

            // Check if kitchen already spawned
            GameObject existingKitchen = GameObject.Find("KitchenLayout");
            if (existingKitchen != null)
            {
                BaseCounter[] allCounters = existingKitchen.GetComponentsInChildren<BaseCounter>(true);
                DeliveryCounter dc = existingKitchen.GetComponentInChildren<DeliveryCounter>();
                if (allCounters.Length != 16 || dc == null || Mathf.Abs(dc.transform.position.y - (-3.3f)) > 0.2f)
                {
                    DestroyImmediate(existingKitchen);
                    existingKitchen = null;
                }
                else
                {
                    Transform existingFloor = existingKitchen.transform.Find("Floor");
                    if (existingFloor != null && existingFloor.childCount < 200)
                    {
                        DestroyImmediate(existingFloor.gameObject);
                        BuildFloorTiles(existingKitchen);
                    }
                    return;
                }
            }

            GameObject kitchenRoot = new GameObject("KitchenLayout");

            // 3. Floor Tiles (Expanded to 23x12 grid to fully blanket entire canvas in any aspect ratio)
            BuildFloorTiles(kitchenRoot);

            // 4. Counters Setup (Layout 7 Kolom x 5 Baris dengan 4 Sudut Kosong - Total 16 Stasiun)
            // Shifted down by 0.9f so top tables have zero overlap with HUD order tickets
            // BARIS B1 (Deret Atas, Y = 1.5f)
            // K1, B1: KOSONG (Sudut kiri atas)
            CreateDispenser(kitchenRoot, -2.4f, 1.5f, ItemType.Cheese, "KEJU", new Color(1.0f, 0.85f, 0.2f)); // K2, B1: Kiri atas, di kiri Panggangan 1
            CreateStove(kitchenRoot, -1.2f, 1.5f, "PANGGANGAN 1", "Counter_Panggangan_1"); // K3, B1: Kiri Daging
            CreateDispenser(kitchenRoot, 0.0f, 1.5f, ItemType.PattyRaw, "DAGING MENTAH", new Color(0.85f, 0.3f, 0.25f)); // K4, B1: Tengah atas, diapit dua panggangan
            CreateStove(kitchenRoot, 1.2f, 1.5f, "PANGGANGAN 2", "Counter_Panggangan_2"); // K5, B1: Kanan Daging
            CreateClearCounter(kitchenRoot, 2.4f, 1.5f, "MEJA PREP", "Counter_Prep_Atas"); // K6, B1: Kanan Panggangan 2, untuk menaruh patty matang
            // K7, B1: KOSONG (Sudut kanan atas)

            // SISI KIRI - KOLOM K1 (X = -3.6f)
            CreateDispenser(kitchenRoot, -3.6f, 0.3f, ItemType.BunBottom, "ROTI BAWAH", new Color(0.85f, 0.6f, 0.35f)); // K1, B2: Sisi kiri, paling atas
            CreateDispenser(kitchenRoot, -3.6f, -0.9f, ItemType.BunTop, "ROTI ATAS", new Color(0.92f, 0.65f, 0.38f)); // K1, B3: Sisi kiri, di bawah Roti bawah
            CreateClearCounter(kitchenRoot, -3.6f, -2.1f, "MEJA PREP", "Counter_Prep_Kiri"); // K1, B4: Sisi kiri, paling bawah

            // SISI KANAN - KOLOM K7 (X = 3.6f)
            CreateDispenser(kitchenRoot, 3.6f, 0.3f, ItemType.Lettuce, "SELADA", new Color(0.2f, 0.8f, 0.3f)); // K7, B2: Sisi kanan, paling atas
            CreateCuttingCounter(kitchenRoot, 3.6f, -0.9f, "MEJA POTONG"); // K7, B3: Sisi kanan, diapit Selada dan Tomat
            CreateDispenser(kitchenRoot, 3.6f, -2.1f, ItemType.Tomato, "TOMAT", new Color(0.95f, 0.25f, 0.2f)); // K7, B4: Sisi kanan, paling bawah

            // BARIS B5 (Deret Bawah, Y = -3.3f)
            // K1, B5: KOSONG (Sudut kiri bawah)
            CreateClearCounter(kitchenRoot, -2.4f, -3.3f, "MEJA PREP", "Counter_Prep_Bawah_Kiri"); // K2, B5: Bawah, paling kiri
            CreateDispenser(kitchenRoot, -1.2f, -3.3f, ItemType.Plate, "PIRING", new Color(0.95f, 0.95f, 0.95f), plateSprite); // K3, B5: Bawah, di kiri Saji
            CreateDelivery(kitchenRoot, 0.0f, -3.3f, "MEJA SAJI"); // K4, B5: Bawah tengah
            CreateClearCounter(kitchenRoot, 1.2f, -3.3f, "MEJA PREP", "Counter_Prep_Bawah_Kanan"); // K5, B5: Bawah, di kanan Saji
            CreateTrash(kitchenRoot, 2.4f, -3.3f, "SAMPAH"); // K6, B5: Bawah, paling kanan
            // K7, B5: KOSONG (Sudut kanan bawah)

            // 5. PLAYER
            GameObject player = GameObject.Find("Player");
            if (player == null)
            {
                player = new GameObject("Player");
                player.transform.localScale = Vector3.one * 0.95f;

                SpriteRenderer playerSr = player.AddComponent<SpriteRenderer>();
                playerSr.sprite = playerSprite != null ? playerSprite : whiteBoxSprite;
                playerSr.sortingOrder = 15;

                CircleCollider2D col = player.AddComponent<CircleCollider2D>();
                col.radius = 0.38f;
                col.offset = new Vector2(0f, -0.32f);

                Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.gravityScale = 0f;
                rb.freezeRotation = true;
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

                PlayerController pc = player.AddComponent<PlayerController>();
                pc.moveSpeed = 5.2f;
            }
            player.transform.position = new Vector3(0f, -0.9f, 0f);

            // 6. CUSTOMER NPC AT BOTTOM DELIVERY WINDOW
            CustomerNPC customerNPC = FindAnyObjectByType<CustomerNPC>();
            if (customerNPC == null)
            {
                GameObject customer = new GameObject("Customer_NPC");
                customer.transform.SetParent(kitchenRoot.transform);
                customer.transform.position = new Vector3(0.0f, -4.4f, 0f);
                customer.AddComponent<CustomerNPC>();
            }
            else
            {
                customerNPC.transform.position = new Vector3(0.0f, -4.4f, 0f);
            }

            Debug.Log("[KitchenBootstrap] Kitchen successfully constructed and ready to play!");
        }

        private GameObject CreateBaseCounterObj(GameObject parent, float x, float y, string name, Sprite sprite)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent.transform);
            obj.transform.position = new Vector3(x, y, 0);

            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = sprite != null ? sprite : whiteBoxSprite;
            // Meja deret bawah (Y <= -1.8f) berada di depan layer karakter player (order 20 > 15/17)
            sr.sortingOrder = (y <= -1.8f) ? 20 : 1;

            BoxCollider2D bc = obj.AddComponent<BoxCollider2D>();
            bc.size = new Vector2(1.15f, 1.15f);

            return obj;
        }

        private void AddLabel(GameObject parent, string text, Color color)
        {
            GameObject labelObj = new GameObject("Label");
            labelObj.transform.SetParent(parent.transform);
            labelObj.transform.localPosition = new Vector3(0, -0.42f, 0);

            TextMesh tm = labelObj.AddComponent<TextMesh>();
            tm.text = text;
            tm.fontSize = 28;
            tm.characterSize = 0.045f;
            tm.alignment = TextAlignment.Center;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.color = color;

            MeshRenderer mr = labelObj.GetComponent<MeshRenderer>();
            SpriteRenderer parentSr = parent.GetComponent<SpriteRenderer>();
            mr.sortingOrder = (parentSr != null && parentSr.sortingOrder >= 20) ? 22 : 8;
        }

        private void CreateClearCounter(GameObject parent, float x, float y, string label, string objName = null)
        {
            GameObject obj = CreateBaseCounterObj(parent, x, y, objName ?? $"Counter_{x}_{y}", counterSprite);
            obj.AddComponent<ClearCounter>();
            AddLabel(obj, label, new Color(0.2f, 0.15f, 0.1f));
        }

        private void CreateDispenser(GameObject parent, float x, float y, ItemType type, string label, Color color, Sprite itemSprite = null)
        {
            GameObject obj = CreateBaseCounterObj(parent, x, y, $"Dispenser_{type}", counterSprite);
            DispenserCounter disp = obj.AddComponent<DispenserCounter>();
            Sprite sp = itemSprite ?? ItemDatabase.GetSprite(type);
            disp.SetDispensedItem(type, sp);
            AddLabel(obj, label, color);
        }

        private void CreateCuttingCounter(GameObject parent, float x, float y, string label)
        {
            GameObject obj = CreateBaseCounterObj(parent, x, y, "Counter_Cutting", cuttingSprite);
            obj.AddComponent<CuttingCounter>();
            AddLabel(obj, label, new Color(0.85f, 0.45f, 0.1f));
        }

        private void CreateStove(GameObject parent, float x, float y, string label, string objName = null)
        {
            GameObject obj = CreateBaseCounterObj(parent, x, y, objName ?? $"Counter_Stove_{x}", grillSprite);
            obj.AddComponent<StoveCounter>();
            AddLabel(obj, label, new Color(0.95f, 0.35f, 0.15f));
        }

        private void CreateDelivery(GameObject parent, float x, float y, string label)
        {
            GameObject obj = CreateBaseCounterObj(parent, x, y, $"Counter_Delivery_{y}", deliverySprite);
            obj.AddComponent<DeliveryCounter>();
            AddLabel(obj, label, Color.cyan);
        }

        private void CreateTrash(GameObject parent, float x, float y, string label)
        {
            GameObject obj = CreateBaseCounterObj(parent, x, y, "Counter_Trash", trashSprite);
            obj.AddComponent<TrashCounter>();
            AddLabel(obj, label, Color.red);
        }

        private void BuildFloorTiles(GameObject kitchenRoot)
        {
            GameObject floorRoot = new GameObject("Floor");
            floorRoot.transform.SetParent(kitchenRoot.transform);
            for (int x = -11; x <= 11; x++)
            {
                for (int y = -6; y <= 5; y++)
                {
                    GameObject tile = new GameObject($"Tile_{x}_{y}");
                    tile.transform.SetParent(floorRoot.transform);
                    tile.transform.position = new Vector3(x * 1.2f, y * 1.2f - 0.9f, 1f);
                    SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();
                    sr.sprite = floorSprite != null ? floorSprite : whiteBoxSprite;
                    sr.color = new Color(0.85f, 0.88f, 0.92f, 0.75f);
                    sr.sortingOrder = -10;
                    tile.transform.localScale = Vector3.one * 1.22f;
                }
            }
        }
    }
}
