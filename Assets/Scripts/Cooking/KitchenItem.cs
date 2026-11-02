using System.Collections.Generic;
using UnityEngine;

namespace BurgerGame
{
    public class KitchenItem : MonoBehaviour
    {
        [Header("Item Data")]
        public ItemType itemType = ItemType.None;

        [Header("Plate Stack (if this item is a Plate)")]
        public List<ItemType> stackedIngredients = new List<ItemType>();

        private SpriteRenderer spriteRenderer;
        private List<GameObject> visualStackObjects = new List<GameObject>();

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
        }

        public void Initialize(ItemType type)
        {
            itemType = type;
            stackedIngredients.Clear();

            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            UpdateVisual();
        }

        public void SetItemType(ItemType newType)
        {
            itemType = newType;
            UpdateVisual();
        }

        public void UpdateVisual()
        {
            if (spriteRenderer == null) return;

            Sprite itemSprite = ItemDatabase.GetSprite(itemType);
            if (itemSprite != null)
            {
                spriteRenderer.sprite = itemSprite;
                spriteRenderer.color = Color.white;
            }
            else
            {
                spriteRenderer.color = ItemDatabase.GetColor(itemType);
            }

            transform.localScale = ItemDatabase.GetScale(itemType);
            spriteRenderer.sortingOrder = 10;
        }

        public bool IsPlate()
        {
            return itemType == ItemType.Plate;
        }

        public bool CanAddIngredientToPlate(ItemType ingredient)
        {
            if (!IsPlate()) return false;
            if (!ItemDatabase.IsBurgerIngredient(ingredient)) return false;

            // Aturan burger: tumpukan maksimal 8 bahan
            if (stackedIngredients.Count >= 8) return false;

            return true;
        }

        public bool TryAddIngredientToPlate(ItemType ingredient)
        {
            if (!CanAddIngredientToPlate(ingredient)) return false;

            stackedIngredients.Add(ingredient);
            RebuildPlateVisuals();
            return true;
        }

        public void SetSortingOrder(int baseOrder)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = baseOrder;
            }

            for (int i = 0; i < visualStackObjects.Count; i++)
            {
                if (visualStackObjects[i] != null)
                {
                    SpriteRenderer sr = visualStackObjects[i].GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        sr.sortingOrder = baseOrder + 1 + i;
                    }
                }
            }
        }

        private void RebuildPlateVisuals()
        {
            // Clear existing visuals
            foreach (var obj in visualStackObjects)
            {
                if (obj != null) Destroy(obj);
            }
            visualStackObjects.Clear();

            int baseOrder = (spriteRenderer != null) ? spriteRenderer.sortingOrder : 10;

            // Stack ingredients vertically with natural thickness
            float currentY = 0.04f;
            for (int i = 0; i < stackedIngredients.Count; i++)
            {
                ItemType ing = stackedIngredients[i];

                GameObject slice = new GameObject($"Layer_{i}_{ing}");
                slice.transform.SetParent(transform);
                // Negative local Z ensures slice is in front of plate in 3D/orthographic space
                slice.transform.localPosition = new Vector3(0, currentY, -0.02f * (i + 1));
                slice.transform.localScale = new Vector3(0.90f, 0.90f, 1f);

                SpriteRenderer sr = slice.AddComponent<SpriteRenderer>();
                Sprite ingSprite = ItemDatabase.GetSprite(ing);
                if (ingSprite != null)
                {
                    sr.sprite = ingSprite;
                    sr.color = Color.white;
                }
                else
                {
                    sr.sprite = spriteRenderer.sprite;
                    sr.color = ItemDatabase.GetColor(ing);
                }
                // Plate is always baseOrder, ingredients are strictly on top (baseOrder + 1 + i)
                sr.sortingOrder = baseOrder + 1 + i;

                visualStackObjects.Add(slice);

                // Ketebalan dinamis antar lapisan bahan
                if (ing == ItemType.Cheese || ing == ItemType.TomatoSliced)
                    currentY += 0.05f;
                else if (ing == ItemType.LettuceChopped)
                    currentY += 0.06f;
                else if (ing == ItemType.PattyCooked)
                    currentY += 0.08f;
                else
                    currentY += 0.07f;
            }
        }

        public static KitchenItem Spawn(ItemType type, Transform parent, Sprite baseSprite = null)
        {
            GameObject go = new GameObject($"Item_{type}");
            if (parent != null)
            {
                go.transform.SetParent(parent);
                go.transform.localPosition = Vector3.zero;
            }

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            Sprite spriteToUse = baseSprite ?? ItemDatabase.GetSprite(type);
            if (spriteToUse != null)
            {
                sr.sprite = spriteToUse;
                sr.color = Color.white;
            }
            else
            {
                sr.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4);
                sr.color = ItemDatabase.GetColor(type);
            }

            KitchenItem item = go.AddComponent<KitchenItem>();
            item.Initialize(type);
            return item;
        }
    }
}
