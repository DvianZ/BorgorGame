using UnityEngine;

namespace BurgerGame
{
    public enum CharacterFacing
    {
        Down,
        Up,
        Left,
        Right
    }

    [RequireComponent(typeof(CircleCollider2D))]
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }

        [Header("Movement")]
        public float moveSpeed = 5.2f;
        private Vector2 movementInput;
        private Vector2 lastFacingDirection = Vector2.down;

        [Header("Mobile Controller Input")]
        public Vector2 mobileMovementInput = Vector2.zero;

        [Header("Interaction")]
        public float interactionDistance = 1.25f;
        public LayerMask counterLayer;
        public Transform holdPoint;
        public KitchenItem heldItem;

        [Header("Character Sprites")]
        public Sprite idleDown;
        public Sprite idleUp;
        public Sprite idleLeft;
        public Sprite idleRight;

        public Sprite[] walkDownSprites;
        public Sprite[] walkUpSprites;
        public Sprite[] walkLeftSprites;
        public Sprite[] walkRightSprites;

        [Header("Animation Settings")]
        public float animationFps = 8f;
        public CharacterFacing currentFacing = CharacterFacing.Down;

        private BaseCounter selectedCounter;
        private SpriteRenderer spriteRenderer;
        private Rigidbody2D rb2d;
        private float walkTimer = 0f;
        private Vector3 baseScale = Vector3.one;
        private int defaultSortingOrder = 15;

        private void Awake()
        {
            Instance = this;
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
            if (spriteRenderer != null && spriteRenderer.sortingOrder > 0)
            {
                defaultSortingOrder = spriteRenderer.sortingOrder;
            }

            rb2d = GetComponent<Rigidbody2D>();

            // Normalize base scale to positive values (prevent inverted sprites)
            baseScale = new Vector3(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
            if (baseScale.x < 0.1f) baseScale = Vector3.one * 0.85f;
            transform.localScale = baseScale;

            // Setup CircleCollider2D around character feet for natural top-down depth
            CircleCollider2D col = GetComponent<CircleCollider2D>();
            if (col != null)
            {
                col.radius = 0.38f;
                col.offset = new Vector2(0f, -0.32f);
            }

            LoadCharacterSprites();

            if (holdPoint == null)
            {
                GameObject hp = new GameObject("HoldPoint");
                hp.transform.SetParent(transform);
                hp.transform.localPosition = new Vector3(0, 0.15f, 0);
                holdPoint = hp.transform;
            }

            // Initial appearance
            UpdateAnimation();
        }

        public void LoadCharacterSprites()
        {
            // Load Idles
            if (idleDown == null) idleDown = Resources.Load<Sprite>("Sprites/Character/chef_idle_down") ?? LoadAssetSprite("chef_idle_down");
            if (idleUp == null) idleUp = Resources.Load<Sprite>("Sprites/Character/chef_idle_up") ?? LoadAssetSprite("chef_idle_up");
            if (idleLeft == null) idleLeft = Resources.Load<Sprite>("Sprites/Character/chef_idle_left") ?? LoadAssetSprite("chef_idle_left");
            if (idleRight == null) idleRight = Resources.Load<Sprite>("Sprites/Character/chef_idle_right") ?? LoadAssetSprite("chef_idle_right");

            // Load Walk Down (4 frames)
            if (walkDownSprites == null || walkDownSprites.Length < 4 || walkDownSprites[0] == null)
            {
                walkDownSprites = new Sprite[4];
                for (int i = 0; i < 4; i++)
                {
                    walkDownSprites[i] = Resources.Load<Sprite>($"Sprites/Character/chef_walk_down_{i}") ?? LoadAssetSprite($"chef_walk_down_{i}");
                }
            }

            // Load Walk Up (4 frames)
            if (walkUpSprites == null || walkUpSprites.Length < 4 || walkUpSprites[0] == null)
            {
                walkUpSprites = new Sprite[4];
                for (int i = 0; i < 4; i++)
                {
                    walkUpSprites[i] = Resources.Load<Sprite>($"Sprites/Character/chef_walk_up_{i}") ?? LoadAssetSprite($"chef_walk_up_{i}");
                }
            }

            // Load Walk Left (4 frames)
            if (walkLeftSprites == null || walkLeftSprites.Length < 4 || walkLeftSprites[0] == null)
            {
                walkLeftSprites = new Sprite[4];
                for (int i = 0; i < 4; i++)
                {
                    walkLeftSprites[i] = Resources.Load<Sprite>($"Sprites/Character/chef_walk_left_{i}") ?? LoadAssetSprite($"chef_walk_left_{i}");
                }
            }

            // Load Walk Right (4 frames)
            if (walkRightSprites == null || walkRightSprites.Length < 4 || walkRightSprites[0] == null)
            {
                walkRightSprites = new Sprite[4];
                for (int i = 0; i < 4; i++)
                {
                    walkRightSprites[i] = Resources.Load<Sprite>($"Sprites/Character/chef_walk_right_{i}") ?? LoadAssetSprite($"chef_walk_right_{i}");
                }
            }
        }

        private Sprite LoadAssetSprite(string name)
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Sprites/Character/{name}.png")
                ?? UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Resources/Sprites/Character/{name}.png");
#else
            return null;
#endif
        }

        private void Update()
        {
            HandleMovementInput();
            UpdateAnimation();
            HandleInteractionDetection();
            HandleActionInput();
        }

        private void FixedUpdate()
        {
            if (movementInput.sqrMagnitude > 0.001f)
            {
                Vector3 moveDelta = new Vector3(movementInput.x, movementInput.y, 0f) * (moveSpeed * Time.fixedDeltaTime);
                if (rb2d != null)
                {
                    rb2d.MovePosition(rb2d.position + new Vector2(moveDelta.x, moveDelta.y));
                }
                else
                {
                    transform.position += moveDelta;
                }
            }
        }

        private void HandleMovementInput()
        {
            if (OrderManager.Instance != null && (OrderManager.Instance.currentState != GameState.Playing || OrderManager.Instance.isOpeningTransition))
            {
                movementInput = Vector2.zero;
                return;
            }

            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            Vector2 combined = new Vector2(h, v) + mobileMovementInput;
            movementInput = Vector2.ClampMagnitude(combined, 1.0f);
        }

        private void UpdateAnimation()
        {
            bool isMoving = movementInput.sqrMagnitude > 0.02f;

            if (isMoving)
            {
                // Prioritize dominant movement direction
                if (Mathf.Abs(movementInput.x) > Mathf.Abs(movementInput.y) * 0.85f)
                {
                    if (movementInput.x > 0.05f)
                    {
                        currentFacing = CharacterFacing.Right;
                        lastFacingDirection = Vector2.right;
                    }
                    else if (movementInput.x < -0.05f)
                    {
                        currentFacing = CharacterFacing.Left;
                        lastFacingDirection = Vector2.left;
                    }
                }
                else
                {
                    if (movementInput.y > 0.05f)
                    {
                        currentFacing = CharacterFacing.Up;
                        lastFacingDirection = Vector2.up;
                    }
                    else if (movementInput.y < -0.05f)
                    {
                        currentFacing = CharacterFacing.Down;
                        lastFacingDirection = Vector2.down;
                    }
                }

                // Advance walking cycle
                walkTimer += Time.deltaTime * animationFps;
                int frameIndex = Mathf.FloorToInt(walkTimer) % 4;

                Sprite activeSprite = null;
                switch (currentFacing)
                {
                    case CharacterFacing.Down:
                        if (walkDownSprites != null && walkDownSprites.Length > frameIndex)
                            activeSprite = walkDownSprites[frameIndex];
                        break;
                    case CharacterFacing.Up:
                        if (walkUpSprites != null && walkUpSprites.Length > frameIndex)
                            activeSprite = walkUpSprites[frameIndex];
                        break;
                    case CharacterFacing.Left:
                        if (walkLeftSprites != null && walkLeftSprites.Length > frameIndex)
                            activeSprite = walkLeftSprites[frameIndex];
                        break;
                    case CharacterFacing.Right:
                        if (walkRightSprites != null && walkRightSprites.Length > frameIndex)
                            activeSprite = walkRightSprites[frameIndex];
                        break;
                }

                if (activeSprite != null && spriteRenderer != null)
                {
                    spriteRenderer.sprite = activeSprite;
                }

                // Subtle step bobbing for responsive game juice
                float stepBounce = Mathf.Sin(walkTimer * Mathf.PI * 2f) * 0.025f;
                transform.localScale = new Vector3(baseScale.x, baseScale.y + stepBounce, baseScale.z);
            }
            else
            {
                walkTimer = 0f;

                Sprite idleSp = null;
                switch (currentFacing)
                {
                    case CharacterFacing.Down:
                        idleSp = idleDown ?? (walkDownSprites != null && walkDownSprites.Length > 0 ? walkDownSprites[0] : null);
                        break;
                    case CharacterFacing.Up:
                        idleSp = idleUp ?? (walkUpSprites != null && walkUpSprites.Length > 0 ? walkUpSprites[0] : null);
                        break;
                    case CharacterFacing.Left:
                        idleSp = idleLeft ?? (walkLeftSprites != null && walkLeftSprites.Length > 0 ? walkLeftSprites[0] : null);
                        break;
                    case CharacterFacing.Right:
                        idleSp = idleRight ?? (walkRightSprites != null && walkRightSprites.Length > 0 ? walkRightSprites[0] : null);
                        break;
                }

                if (idleSp != null && spriteRenderer != null)
                {
                    spriteRenderer.sprite = idleSp;
                }

                // Gentle idle breathing animation
                float breathe = Mathf.Sin(Time.time * 2.8f) * 0.015f;
                transform.localScale = new Vector3(baseScale.x, baseScale.y + breathe, baseScale.z);
            }

            UpdateHoldPoint();
        }

        private void UpdateHoldPoint()
        {
            if (holdPoint == null) return;

            bool isFacingUp = (currentFacing == CharacterFacing.Up);

            // Position held item naturally based on current chef facing direction
            switch (currentFacing)
            {
                case CharacterFacing.Down:
                    holdPoint.localPosition = new Vector3(0f, 0.05f, 0f);
                    break;
                case CharacterFacing.Up:
                    holdPoint.localPosition = new Vector3(0f, 0.18f, 0f);
                    break;
                case CharacterFacing.Left:
                    holdPoint.localPosition = new Vector3(-0.35f, 0.15f, 0f);
                    break;
                case CharacterFacing.Right:
                    holdPoint.localPosition = new Vector3(0.35f, 0.15f, 0f);
                    break;
            }

            // Atur order in layer:
            // Ketika karakter menghadap atas, order in layer karakter menjadi paling depan agar menutupi apa yang dibawa.
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = isFacingUp ? (defaultSortingOrder + 2) : defaultSortingOrder;
            }

            if (heldItem != null)
            {
                int playerOrder = (spriteRenderer != null) ? spriteRenderer.sortingOrder : defaultSortingOrder;
                // Jika menghadap atas: item ditaruh di layer belakang pemain (playerOrder - 10)
                // Jika menghadap arah lain: item ditaruh di layer depan pemain (playerOrder + 2)
                int baseOrder = isFacingUp ? (playerOrder - 10) : (playerOrder + 2);
                heldItem.SetSortingOrder(baseOrder);
            }
        }

        public Vector2 GetFacingDirection()
        {
            switch (currentFacing)
            {
                case CharacterFacing.Up: return Vector2.up;
                case CharacterFacing.Down: return Vector2.down;
                case CharacterFacing.Left: return Vector2.left;
                case CharacterFacing.Right: return Vector2.right;
                default: return Vector2.down;
            }
        }

        private void HandleInteractionDetection()
        {
            Vector2 center = (Vector2)transform.position;
            Vector2 facingDir = GetFacingDirection();
            Vector2 checkPos = center + facingDir * 0.95f;

            // Search for all counters in the interaction radius
            Collider2D[] allHits = Physics2D.OverlapCircleAll(center, 1.65f);
            BaseCounter closestCounter = null;
            float bestScore = float.MaxValue;

            foreach (var hit in allHits)
            {
                BaseCounter counter = hit.GetComponent<BaseCounter>();
                if (counter == null) continue;

                Vector2 counterPos = (Vector2)counter.transform.position;
                Vector2 toCounter = counterPos - center;
                float distToCenter = toCounter.magnitude;
                if (distToCenter > 1.65f) continue;

                // Check directional alignment relative to character facing direction
                float dot = Vector2.Dot(facingDir, toCounter.normalized);

                // Strictly filter out counters that are not in the direction the character is facing.
                // dot < 0.35f rejects perpendicular (90 degrees, dot ~ 0) and behind (dot < 0) counters.
                if (dot < 0.35f) continue;

                // Projection along facing vector
                float forwardDist = Vector2.Dot(toCounter, facingDir);
                if (forwardDist < 0.15f) continue; // Must be in front of the player

                // Side offset perpendicular to facing vector
                float sideDist = Mathf.Abs(Vector2.Dot(toCounter, new Vector2(-facingDir.y, facingDir.x)));
                if (sideDist > 1.05f) continue; // Must not be excessively offset sideways

                // Distance to the forward check point
                float distToCheckPos = Vector2.Distance(checkPos, counterPos);

                // Weighted score: heavily favor forward alignment and closeness to interaction point
                float score = distToCheckPos / Mathf.Max(0.2f, dot);
                if (score < bestScore)
                {
                    bestScore = score;
                    closestCounter = counter;
                }
            }

            // Update counter highlight
            if (selectedCounter != closestCounter)
            {
                if (selectedCounter != null) selectedCounter.SetSelected(false);
                selectedCounter = closestCounter;
                if (selectedCounter != null) selectedCounter.SetSelected(true);
            }
        }

        public void TriggerPrimaryAction()
        {
            if (OrderManager.Instance != null && (OrderManager.Instance.currentState != GameState.Playing || OrderManager.Instance.isOpeningTransition)) return;
            if (selectedCounter != null)
            {
                selectedCounter.Interact(this);
            }
        }

        public void TriggerAlternateAction()
        {
            if (OrderManager.Instance != null && (OrderManager.Instance.currentState != GameState.Playing || OrderManager.Instance.isOpeningTransition)) return;
            if (selectedCounter != null)
            {
                selectedCounter.InteractAlternate(this);
            }
        }

        public void TriggerDropAction()
        {
            if (OrderManager.Instance != null && (OrderManager.Instance.currentState != GameState.Playing || OrderManager.Instance.isOpeningTransition)) return;
            // Pemain harus berada di depan tempat sampah untuk membuang bahan
            if (selectedCounter != null && selectedCounter is TrashCounter)
            {
                selectedCounter.Interact(this);
            }
        }

        private void HandleActionInput()
        {
            if (OrderManager.Instance != null && (OrderManager.Instance.currentState != GameState.Playing || OrderManager.Instance.isOpeningTransition))
            {
                return;
            }

            // Primary Interact: Space or E
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
            {
                TriggerPrimaryAction();
            }

            // Alternate Interact (Chop/Cut): F or C
            if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.C))
            {
                TriggerAlternateAction();
            }

            // Drop/Trash shortcut: Q
            if (Input.GetKeyDown(KeyCode.Q))
            {
                TriggerDropAction();
            }
        }

        public void ResetPlayer()
        {
            if (heldItem != null)
            {
                Destroy(heldItem.gameObject);
                heldItem = null;
            }
            transform.position = Vector3.zero;
            if (rb2d != null)
            {
                rb2d.position = Vector2.zero;
                rb2d.linearVelocity = Vector2.zero;
            }
            mobileMovementInput = Vector2.zero;
            movementInput = Vector2.zero;
            currentFacing = CharacterFacing.Down;
            lastFacingDirection = Vector2.down;
            walkTimer = 0f;
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = idleDown ?? (walkDownSprites != null && walkDownSprites.Length > 0 ? walkDownSprites[0] : null);
            }
            if (selectedCounter != null)
            {
                selectedCounter.SetSelected(false);
                selectedCounter = null;
            }
        }

        public bool HasItem()
        {
            return heldItem != null;
        }

        public KitchenItem GetItem()
        {
            return heldItem;
        }

        public void SetItem(KitchenItem item)
        {
            heldItem = item;
            if (item != null)
            {
                item.transform.SetParent(holdPoint);
                item.transform.localPosition = Vector3.zero;
                UpdateHoldPoint();
            }
        }

        public KitchenItem TakeItem()
        {
            KitchenItem item = heldItem;
            heldItem = null;
            if (item != null)
            {
                item.SetSortingOrder(10);
            }
            return item;
        }
    }
}
