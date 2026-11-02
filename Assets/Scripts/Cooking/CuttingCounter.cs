using UnityEngine;

namespace BurgerGame
{
    public class CuttingCounter : BaseCounter
    {
        [Header("Cutting Logic")]
        public int cutProgress = 0;
        public int maxCuts = 3;

        private GameObject progressBarObject;
        private Transform progressFill;

        protected override void Awake()
        {
            base.Awake();
            CreateProgressBar();
        }

        private static Sprite cutSpriteLeft;
        private static Sprite cutSpriteCenter;

        private static Sprite GetCutSpriteLeft()
        {
            if (cutSpriteLeft == null)
            {
                Texture2D tex = Texture2D.whiteTexture;
                cutSpriteLeft = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0f, 0.5f), tex.width);
            }
            return cutSpriteLeft;
        }

        private static Sprite GetCutSpriteCenter()
        {
            if (cutSpriteCenter == null)
            {
                Texture2D tex = Texture2D.whiteTexture;
                cutSpriteCenter = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), tex.width);
            }
            return cutSpriteCenter;
        }

        private void CreateProgressBar()
        {
            if (progressBarObject != null) return;

            progressBarObject = new GameObject("CutProgressBar");
            progressBarObject.transform.SetParent(transform);
            progressBarObject.transform.localPosition = new Vector3(0, 0.80f, 0);

            // Background Frame (Soft Pastel Cream)
            GameObject bg = new GameObject("BG");
            bg.transform.SetParent(progressBarObject.transform);
            bg.transform.localPosition = Vector3.zero;
            bg.transform.localScale = new Vector3(0.9f, 0.18f, 1f);
            SpriteRenderer srBg = bg.AddComponent<SpriteRenderer>();
            srBg.sprite = GetCutSpriteCenter();
            srBg.color = new Color(0.88f, 0.85f, 0.82f, 0.95f);
            srBg.sortingOrder = 20;

            // Fill (Anchor left - Soft Pastel Sky Mint)
            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(progressBarObject.transform);
            fill.transform.localPosition = new Vector3(-0.42f, 0, 0); // anchor left
            fill.transform.localScale = new Vector3(0f, 0.12f, 1f);
            SpriteRenderer srFill = fill.AddComponent<SpriteRenderer>();
            srFill.sprite = GetCutSpriteLeft();
            srFill.color = new Color(0.60f, 0.85f, 0.92f, 1f);
            srFill.sortingOrder = 21;
            progressFill = fill.transform;

            progressBarObject.SetActive(false);
        }

        private void UpdateProgressBar()
        {
            if (progressBarObject == null || progressFill == null) return;

            if (cutProgress > 0 && cutProgress < maxCuts)
            {
                progressBarObject.SetActive(true);
                float ratio = (float)cutProgress / maxCuts;
                progressFill.localScale = new Vector3(0.84f * ratio, 0.12f, 1f);
            }
            else
            {
                progressBarObject.SetActive(false);
            }
        }

        public bool CanCut(ItemType type)
        {
            return type == ItemType.Lettuce || type == ItemType.Tomato;
        }

        public ItemType GetCutOutput(ItemType input)
        {
            if (input == ItemType.Lettuce) return ItemType.LettuceChopped;
            if (input == ItemType.Tomato) return ItemType.TomatoSliced;
            return input;
        }

        public override void Interact(PlayerController player)
        {
            if (!HasItem())
            {
                if (player.HasItem() && CanCut(player.GetItem().itemType))
                {
                    SetItem(player.TakeItem());
                    cutProgress = 0;
                    UpdateProgressBar();
                }
            }
            else
            {
                if (!player.HasItem())
                {
                    player.SetItem(TakeItem());
                    cutProgress = 0;
                    UpdateProgressBar();
                }
                else if (player.GetItem().IsPlate())
                {
                    if (player.GetItem().TryAddIngredientToPlate(currentItem.itemType))
                    {
                        Destroy(TakeItem().gameObject);
                        cutProgress = 0;
                        UpdateProgressBar();
                    }
                }
            }
        }

        public override void InteractAlternate(PlayerController player)
        {
            if (HasItem() && CanCut(currentItem.itemType))
            {
                cutProgress++;
                UpdateProgressBar();

                // Play cutting / chopping SFX
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayChopSFX();
                }

                // Quick visual bounce on item
                if (currentItem != null)
                {
                    currentItem.transform.localScale *= 1.15f;
                    Invoke(nameof(ResetItemScale), 0.08f);
                }

                if (cutProgress >= maxCuts)
                {
                    ItemType output = GetCutOutput(currentItem.itemType);
                    currentItem.SetItemType(output);
                    cutProgress = 0;
                    UpdateProgressBar();
                }
            }
        }

        private void ResetItemScale()
        {
            if (currentItem != null)
            {
                currentItem.UpdateVisual();
            }
        }

        public override void ResetCounter()
        {
            base.ResetCounter();
            cutProgress = 0;
            if (progressBarObject != null) progressBarObject.SetActive(false);
        }
    }
}
