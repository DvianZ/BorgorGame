using UnityEngine;

namespace BurgerGame
{
    public class DeliveryCounter : BaseCounter
    {
        public override void Interact(PlayerController player)
        {
            if (player.HasItem())
            {
                KitchenItem item = player.GetItem();
                if (item.IsPlate())
                {
                    // Check burger against active orders
                    bool success = OrderManager.Instance != null && 
                                   OrderManager.Instance.TryDeliverBurger(item.stackedIngredients);

                    if (success)
                    {
                        // Successfully served!
                        Destroy(player.TakeItem().gameObject);

                        // Visual flash feedback
                        StartCoroutine(FlashColor(Color.green));
                    }
                    else
                    {
                        // Wrong recipe or incomplete burger
                        StartCoroutine(FlashColor(Color.red));
                    }
                }
            }
        }

        private System.Collections.IEnumerator FlashColor(Color color)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
                yield return new WaitForSeconds(0.25f);
                spriteRenderer.color = originalColor;
            }
        }
    }
}
