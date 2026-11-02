using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace BurgerGame.Editor
{
    [InitializeOnLoad]
    public static class KitchenSceneBuilder
    {
        static KitchenSceneBuilder()
        {
            EditorApplication.delayCall += CheckAndAutoBake;
        }

        private static void CheckAndAutoBake()
        {
            if (Application.isPlaying) return;

            GameObject kitchen = GameObject.Find("KitchenLayout");
            Transform floor = kitchen != null ? kitchen.transform.Find("Floor") : null;
            Camera cam = Camera.main;
            BaseCounter[] counters = kitchen != null ? kitchen.GetComponentsInChildren<BaseCounter>() : null;

            // Auto rebuild if kitchen missing, floor not fully expanded (<200 tiles), counter count != 16, or camera not zoomed/positioned correctly
            bool needsRebuild = (kitchen == null || floor == null || floor.childCount < 200 || counters == null || counters.Length != 16 ||
                (cam != null && (Mathf.Abs(cam.orthographicSize - 4.6f) > 0.05f || Mathf.Abs(cam.transform.position.y - (-0.9f)) > 0.05f)));

            if (!needsRebuild && kitchen != null)
            {
                DeliveryCounter[] deliveries = kitchen.GetComponentsInChildren<DeliveryCounter>();
                if (deliveries.Length != 1 || Mathf.Abs(deliveries[0].transform.position.x) > 0.2f || Mathf.Abs(deliveries[0].transform.position.y - (-3.3f)) > 0.2f)
                {
                    needsRebuild = true;
                }
                else
                {
                    DispenserCounter[] dispensers = kitchen.GetComponentsInChildren<DispenserCounter>();
                    foreach (var d in dispensers)
                    {
                        SpriteRenderer dSr = d.GetComponent<SpriteRenderer>();
                        if (dSr != null && dSr.color.r > 0.85f)
                        {
                            needsRebuild = true;
                            break;
                        }

                        if (d.transform.position.y <= -1.8f)
                        {
                            Transform prev = d.transform.Find("PreviewItem") ?? d.transform.Find("DispenserPreview");
                            SpriteRenderer prevSr = prev != null ? prev.GetComponent<SpriteRenderer>() : null;
                            if (prevSr != null && prevSr.sortingOrder < 20)
                            {
                                needsRebuild = true;
                                break;
                            }
                        }
                    }
                }
            }

            if (needsRebuild)
            {
                BakeAllScenes();
            }
        }

        [MenuItem("Burger Game/Bake All Scenes (MainKitchen & dapur)")]
        public static void BakeAllScenes()
        {
            string[] scenePaths = new string[] { "Assets/Scenes/MainKitchen.unity", "Assets/Scenes/dapur.unity" };
            foreach (string p in scenePaths)
            {
                if (System.IO.File.Exists(p))
                {
                    var scene = EditorSceneManager.OpenScene(p, OpenSceneMode.Single);
                    BuildKitchenScene();
                    EditorSceneManager.SaveScene(scene);
                }
            }
            Debug.Log("[KitchenSceneBuilder] Seluruh scene (MainKitchen & dapur) berhasil di-bake dan disimpan!");
        }

        [MenuItem("Burger Game/Setup Kitchen Scene (Bake)")]
        public static void BuildKitchenScene()
        {
            // 1. Camera Setup
            Camera cam = Camera.main;
            if (cam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                cam = camObj.AddComponent<Camera>();
                camObj.tag = "MainCamera";
                camObj.AddComponent<AudioListener>();
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

            // Ensure AudioManager exists in the scene and positioned outside camera view
            AudioManager audioMgr = Object.FindAnyObjectByType<AudioManager>();
            if (audioMgr == null)
            {
                GameObject audioObj = new GameObject("AudioManager");
                audioObj.transform.position = new Vector3(0, 50f, 0);
                audioObj.AddComponent<AudioManager>();
            }
            else
            {
                audioMgr.transform.position = new Vector3(0, 50f, 0);
            }

            // Move Directional Light far outside camera view so its gizmo doesn't overlay the kitchen
            Light[] lights = Object.FindObjectsByType<Light>(FindObjectsInactive.Include);
            foreach (var l in lights)
            {
                if (l != null && l.type == LightType.Directional)
                {
                    l.transform.position = new Vector3(0, 50f, 0);
                }
            }

            // Load Sprites from AssetDatabase
            Sprite counterSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/box_counter.png");
            Sprite grillSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/box_grill.png");
            Sprite cuttingSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/box_cutting.png");
            Sprite deliverySprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/box_delivery.png");
            Sprite trashSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/box_trash.png");
            Sprite plateSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/plate.png") ?? AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/box_plate.png");
            Sprite bunBottomSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/bun_bottom.png");
            Sprite bunTopSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/bun_top.png");
            Sprite pattyRawSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/patty_raw.png");
            Sprite lettuceSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/lettuce.png");
            Sprite cheeseSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/cheese.png");
            Sprite tomatoSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/tomato.png");
            Sprite playerSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Character/chef_idle_down.png") ?? AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/box_player.png");
            Sprite floorSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/tile_floor.png");
            Sprite whiteBoxSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/box_white.png");

            // Clean existing if any
            GameObject existing = GameObject.Find("KitchenLayout");
            if (existing != null) Object.DestroyImmediate(existing);

            GameObject existingPlayer = GameObject.Find("Player");
            if (existingPlayer != null) Object.DestroyImmediate(existingPlayer);

            CustomerNPC[] oldCustomers = Object.FindObjectsByType<CustomerNPC>();
            foreach (var c in oldCustomers)
            {
                if (c != null && c.gameObject != null) Object.DestroyImmediate(c.gameObject);
            }
            GameObject oldCustomer = GameObject.Find("Customer_NPC");
            while (oldCustomer != null)
            {
                Object.DestroyImmediate(oldCustomer);
                oldCustomer = GameObject.Find("Customer_NPC");
            }

            // 2. Kitchen Root
            GameObject kitchenRoot = new GameObject("KitchenLayout");

            // 3. Floor Tiles (Expanded to 23x12 grid to fully cover entire canvas in any aspect ratio)
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

            // 4. Counters Setup (Layout 7 Kolom x 5 Baris dengan 4 Sudut Kosong - Total 16 Stasiun)
            // Shifted down by 0.9f so top tables have zero overlap with HUD order tickets
            // BARIS B1 (Deret Atas, Y = 1.5f)
            // K1, B1: KOSONG (Sudut kiri atas)
            CreateDispenser(kitchenRoot, -2.4f, 1.5f, ItemType.Cheese, "KEJU", new Color(1.0f, 0.85f, 0.2f), counterSprite, cheeseSprite); // K2, B1: Kiri atas, di kiri Panggangan 1
            CreateStove(kitchenRoot, -1.2f, 1.5f, "PANGGANGAN 1", grillSprite, "Counter_Panggangan_1"); // K3, B1: Kiri Daging
            CreateDispenser(kitchenRoot, 0.0f, 1.5f, ItemType.PattyRaw, "DAGING MENTAH", new Color(0.85f, 0.3f, 0.25f), counterSprite, pattyRawSprite); // K4, B1: Tengah atas, diapit dua panggangan
            CreateStove(kitchenRoot, 1.2f, 1.5f, "PANGGANGAN 2", grillSprite, "Counter_Panggangan_2"); // K5, B1: Kanan Daging
            CreateClearCounter(kitchenRoot, 2.4f, 1.5f, "MEJA PREP", counterSprite, "Counter_Prep_Atas"); // K6, B1: Kanan Panggangan 2, untuk menaruh patty matang
            // K7, B1: KOSONG (Sudut kanan atas)

            // SISI KIRI - KOLOM K1 (X = -3.6f)
            CreateDispenser(kitchenRoot, -3.6f, 0.3f, ItemType.BunBottom, "ROTI BAWAH", new Color(0.85f, 0.6f, 0.35f), counterSprite, bunBottomSprite); // K1, B2: Sisi kiri, paling atas
            CreateDispenser(kitchenRoot, -3.6f, -0.9f, ItemType.BunTop, "ROTI ATAS", new Color(0.92f, 0.65f, 0.38f), counterSprite, bunTopSprite); // K1, B3: Sisi kiri, di bawah Roti bawah
            CreateClearCounter(kitchenRoot, -3.6f, -2.1f, "MEJA PREP", counterSprite, "Counter_Prep_Kiri"); // K1, B4: Sisi kiri, paling bawah

            // SISI KANAN - KOLOM K7 (X = 3.6f)
            CreateDispenser(kitchenRoot, 3.6f, 0.3f, ItemType.Lettuce, "SELADA", new Color(0.2f, 0.8f, 0.3f), counterSprite, lettuceSprite); // K7, B2: Sisi kanan, paling atas
            CreateCuttingCounter(kitchenRoot, 3.6f, -0.9f, "MEJA POTONG", cuttingSprite); // K7, B3: Sisi kanan, diapit Selada dan Tomat
            CreateDispenser(kitchenRoot, 3.6f, -2.1f, ItemType.Tomato, "TOMAT", new Color(0.95f, 0.25f, 0.2f), counterSprite, tomatoSprite); // K7, B4: Sisi kanan, paling bawah

            // BARIS B5 (Deret Bawah, Y = -3.3f)
            // K1, B5: KOSONG (Sudut kiri bawah)
            CreateClearCounter(kitchenRoot, -2.4f, -3.3f, "MEJA PREP", counterSprite, "Counter_Prep_Bawah_Kiri"); // K2, B5: Bawah, paling kiri
            CreateDispenser(kitchenRoot, -1.2f, -3.3f, ItemType.Plate, "PIRING", new Color(0.95f, 0.95f, 0.95f), counterSprite, plateSprite); // K3, B5: Bawah, di kiri Saji
            CreateDelivery(kitchenRoot, 0.0f, -3.3f, "MEJA SAJI", deliverySprite); // K4, B5: Bawah tengah
            CreateClearCounter(kitchenRoot, 1.2f, -3.3f, "MEJA PREP", counterSprite, "Counter_Prep_Bawah_Kanan"); // K5, B5: Bawah, di kanan Saji
            CreateTrash(kitchenRoot, 2.4f, -3.3f, "SAMPAH", trashSprite); // K6, B5: Bawah, paling kanan
            // K7, B5: KOSONG (Sudut kanan bawah)

            // 5. Player
            GameObject player = new GameObject("Player");
            player.transform.position = new Vector3(0f, -0.9f, 0f);
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

            // 6. Customer NPC at Bottom Delivery Window
            Sprite happySp = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/customer_happy.png");
            Sprite neutralSp = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/customer_neutral.png");
            Sprite angrySp = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/customer_angry.png");

            GameObject customer = new GameObject("Customer_NPC");
            customer.transform.SetParent(kitchenRoot.transform);
            customer.transform.position = new Vector3(0.0f, -4.4f, 0f);
            CustomerNPC npc = customer.AddComponent<CustomerNPC>();
            npc.happySprite = happySp;
            npc.neutralSprite = neutralSp;
            npc.angrySprite = angrySp;

            // 7. OrderManager & GameHUD
            if (Object.FindAnyObjectByType<OrderManager>() == null)
            {
                GameObject om = new GameObject("OrderManager");
                om.AddComponent<OrderManager>();
            }

            if (Object.FindAnyObjectByType<GameHUD>() == null)
            {
                GameObject hud = new GameObject("GameHUD");
                hud.AddComponent<GameHUD>();
            }

            // Mark Scene Dirty and Save
            var sceneToSave = EditorSceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(sceneToSave);
            if (!string.IsNullOrEmpty(sceneToSave.path))
            {
                EditorSceneManager.SaveScene(sceneToSave);
            }
            Debug.Log("[KitchenSceneBuilder] Dapur, Pemain, dan Karakter Pembeli berhasil di-bake ke dalam scene!");
        }

        private static GameObject CreateBaseCounterObj(GameObject parent, float x, float y, string name, Sprite sprite)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent.transform);
            obj.transform.position = new Vector3(x, y, 0);

            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            // Meja deret bawah (Y <= -1.8f) berada di depan layer karakter player (order 20 > 15/17)
            sr.sortingOrder = (y <= -1.8f) ? 20 : 1;

            BoxCollider2D bc = obj.AddComponent<BoxCollider2D>();
            bc.size = new Vector2(1.15f, 1.15f);

            return obj;
        }

        private static void AddLabel(GameObject parent, string text, Color color)
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

        private static void CreateClearCounter(GameObject parent, float x, float y, string label, Sprite sprite, string objName = null)
        {
            GameObject obj = CreateBaseCounterObj(parent, x, y, objName ?? $"Counter_{x}_{y}", sprite);
            obj.AddComponent<ClearCounter>();
            AddLabel(obj, label, new Color(0.2f, 0.15f, 0.1f));
        }

        private static void CreateDispenser(GameObject parent, float x, float y, ItemType type, string label, Color color, Sprite counterSprite, Sprite itemSprite)
        {
            GameObject obj = CreateBaseCounterObj(parent, x, y, $"Dispenser_{type}", counterSprite);

            // Meja pengambil bahan dibuat sedikit lebih gelap sebagai pembeda
            SpriteRenderer counterSr = obj.GetComponent<SpriteRenderer>();
            if (counterSr != null)
            {
                counterSr.color = new Color(0.72f, 0.70f, 0.68f, 1f);
            }

            DispenserCounter disp = obj.AddComponent<DispenserCounter>();
            disp.dispensedItem = type;
            disp.itemSprite = itemSprite;
            AddLabel(obj, label, color);

            if (itemSprite != null)
            {
                GameObject preview = new GameObject("PreviewItem");
                preview.transform.SetParent(obj.transform);
                preview.transform.localPosition = new Vector3(0, 0.20f, 0);
                SpriteRenderer sr = preview.AddComponent<SpriteRenderer>();
                sr.sprite = itemSprite;
                sr.color = Color.white;
                sr.sortingOrder = (y <= -1.8f) ? 21 : 5;
                preview.transform.localScale = ItemDatabase.GetScale(type) * 0.75f;

                if (type == ItemType.Plate)
                {
                    GameObject stackObj = new GameObject("PlateStackVisual");
                    stackObj.transform.SetParent(preview.transform);
                    stackObj.transform.localPosition = new Vector3(0, 0.08f, 0);
                    stackObj.transform.localScale = Vector3.one;
                    SpriteRenderer stackSr = stackObj.AddComponent<SpriteRenderer>();
                    stackSr.sprite = itemSprite;
                    stackSr.color = new Color(0.95f, 0.95f, 0.95f, 1f);
                    stackSr.sortingOrder = (y <= -1.8f) ? 22 : 6;
                }
            }
        }

        private static void CreateCuttingCounter(GameObject parent, float x, float y, string label, Sprite sprite)
        {
            GameObject obj = CreateBaseCounterObj(parent, x, y, "Counter_Cutting", sprite);
            obj.AddComponent<CuttingCounter>();
            AddLabel(obj, label, new Color(0.85f, 0.45f, 0.1f));
        }

        private static void CreateStove(GameObject parent, float x, float y, string label, Sprite sprite, string objName = null)
        {
            GameObject obj = CreateBaseCounterObj(parent, x, y, objName ?? $"Counter_Stove_{x}", sprite);
            obj.AddComponent<StoveCounter>();
            AddLabel(obj, label, new Color(0.95f, 0.35f, 0.15f));
        }

        private static void CreateDelivery(GameObject parent, float x, float y, string label, Sprite sprite)
        {
            GameObject obj = CreateBaseCounterObj(parent, x, y, $"Counter_Delivery_{y}", sprite);
            obj.AddComponent<DeliveryCounter>();
            AddLabel(obj, label, Color.cyan);
        }

        private static void CreateTrash(GameObject parent, float x, float y, string label, Sprite sprite)
        {
            GameObject obj = CreateBaseCounterObj(parent, x, y, "Counter_Trash", sprite);
            obj.AddComponent<TrashCounter>();
            AddLabel(obj, label, Color.red);
        }
    }
}
