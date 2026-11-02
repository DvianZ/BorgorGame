using UnityEngine;

namespace BurgerGame
{
    public class BaseCounter : MonoBehaviour
    {
        [Header("Counter Setup")]
        public Transform topPoint;
        public KitchenItem currentItem;

        [Header("Visual Feedback")]
        protected SpriteRenderer spriteRenderer;
        protected Color originalColor = Color.white;
        protected bool isSelected = false;

        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                originalColor = spriteRenderer.color;

                // Meja deret bawah (Y <= -1.8f) layernya diatur di depan karakter player (order 20 > 15/17)
                if (transform.position.y <= -1.8f)
                {
                    spriteRenderer.sortingOrder = 20;
                }
            }

            if (topPoint == null)
            {
                // Create child top point if not assigned
                GameObject tp = new GameObject("TopPoint");
                tp.transform.SetParent(transform);
                tp.transform.localPosition = new Vector3(0, 0.25f, 0);
                topPoint = tp.transform;
            }
        }

        protected virtual void Start()
        {
            // Pastikan jika meja berada di deret bawah, sorting order meja dan labelnya berada di depan player
            if (transform.position.y <= -1.8f)
            {
                if (spriteRenderer != null)
                {
                    spriteRenderer.sortingOrder = 20;
                }

                MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
                if (mr != null)
                {
                    mr.sortingOrder = 22;
                }
            }
        }

        public virtual void Interact(PlayerController player)
        {
            // Override in derived classes
        }

        public virtual void InteractAlternate(PlayerController player)
        {
            // Override in derived classes (e.g. CuttingCounter)
        }

        public virtual bool HasItem()
        {
            return currentItem != null;
        }

        public virtual void SetItem(KitchenItem item)
        {
            currentItem = item;
            if (item != null)
            {
                item.transform.SetParent(topPoint);
                item.transform.localPosition = Vector3.zero;
                int baseOrder = (spriteRenderer != null) ? (spriteRenderer.sortingOrder + 2) : 10;
                item.SetSortingOrder(baseOrder);
            }
        }

        public virtual KitchenItem TakeItem()
        {
            KitchenItem item = currentItem;
            currentItem = null;
            return item;
        }

        public virtual void SetSelected(bool selected)
        {
            isSelected = selected;
            if (spriteRenderer != null)
            {
                if (selected)
                {
                    // Subtle brighten highlight
                    spriteRenderer.color = Color.Lerp(originalColor, Color.white, 0.45f);
                }
                else
                {
                    spriteRenderer.color = originalColor;
                }
            }
        }

        public virtual void ResetCounter()
        {
            if (currentItem != null)
            {
                Destroy(currentItem.gameObject);
                currentItem = null;
            }
            SetSelected(false);
        }
    }
}
