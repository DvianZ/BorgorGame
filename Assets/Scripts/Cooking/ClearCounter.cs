using UnityEngine;

namespace BurgerGame
{
    public class ClearCounter : BaseCounter
    {
        public override void Interact(PlayerController player)
        {
            if (!HasItem())
            {
                // Counter is empty
                if (player.HasItem())
                {
                    // Player drops item onto counter
                    SetItem(player.TakeItem());
                }
            }
            else
            {
                // Counter has an item
                if (!player.HasItem())
                {
                    // Player picks up item from counter
                    player.SetItem(TakeItem());
                }
                else
                {
                    // Both have items: Check Plate interaction!
                    if (player.GetItem().IsPlate())
                    {
                        // Player holds Plate, Counter has ingredient
                        if (player.GetItem().TryAddIngredientToPlate(currentItem.itemType))
                        {
                            Destroy(TakeItem().gameObject);
                        }
                    }
                    else if (currentItem.IsPlate())
                    {
                        // Counter has Plate, Player holds ingredient
                        if (currentItem.TryAddIngredientToPlate(player.GetItem().itemType))
                        {
                            Destroy(player.TakeItem().gameObject);
                        }
                    }
                }
            }
        }
    }
}
