using UnityEngine;

namespace BurgerGame
{
    public class TrashCounter : BaseCounter
    {
        public override void Interact(PlayerController player)
        {
            if (player.HasItem())
            {
                KitchenItem item = player.TakeItem();
                if (item != null)
                {
                    // Kurangi skor berdasarkan total nilai barang/bahan yang dibuang
                    if (OrderManager.Instance != null)
                    {
                        OrderManager.Instance.PenalizeWaste(item);
                    }

                    Destroy(item.gameObject);

                    // Efek visual tempat sampah berkedip merah terang saat membuang
                    StartCoroutine(FlashTrash());
                }
            }
        }

        private System.Collections.IEnumerator FlashTrash()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(0.95f, 0.25f, 0.25f, 1f);
                yield return new WaitForSeconds(0.18f);
                spriteRenderer.color = originalColor;
            }
        }
    }
}
