using System.Collections.Generic;
using UnityEngine;

namespace BurgerGame
{
    public enum ItemType
    {
        None = 0,
        Plate,
        BunBottom,
        PattyRaw,
        PattyCooked,
        PattyBurnt,
        Lettuce,
        LettuceChopped,
        Tomato,
        TomatoSliced,
        Cheese,
        BunTop
    }

    public static class ItemDatabase
    {
        public static Color GetColor(ItemType type)
        {
            switch (type)
            {
                case ItemType.Plate:
                    return new Color(0.96f, 0.96f, 0.98f, 1f); // White/Silver plate
                case ItemType.BunBottom:
                    return new Color(0.82f, 0.57f, 0.33f, 1f); // Light brown bun
                case ItemType.PattyRaw:
                    return new Color(0.85f, 0.25f, 0.20f, 1f); // Raw red beef
                case ItemType.PattyCooked:
                    return new Color(0.36f, 0.22f, 0.15f, 1f); // Deep cooked brown
                case ItemType.PattyBurnt:
                    return new Color(0.15f, 0.15f, 0.15f, 1f); // Charcoal black
                case ItemType.Lettuce:
                    return new Color(0.18f, 0.75f, 0.32f, 1f); // Leaf green
                case ItemType.LettuceChopped:
                    return new Color(0.48f, 0.88f, 0.38f, 1f); // Light green cut
                case ItemType.Tomato:
                    return new Color(0.92f, 0.20f, 0.18f, 1f); // Bright red
                case ItemType.TomatoSliced:
                    return new Color(0.96f, 0.40f, 0.38f, 1f); // Sliced red
                case ItemType.Cheese:
                    return new Color(1.0f, 0.82f, 0.15f, 1f);  // Golden cheddar
                case ItemType.BunTop:
                    return new Color(0.90f, 0.62f, 0.35f, 1f); // Golden bun top
                default:
                    return Color.white;
            }
        }

        public static string GetName(ItemType type)
        {
            switch (type)
            {
                case ItemType.Plate: return "Piring";
                case ItemType.BunBottom: return "Roti Bawah";
                case ItemType.PattyRaw: return "Daging Mentah";
                case ItemType.PattyCooked: return "Daging Matang";
                case ItemType.PattyBurnt: return "Daging GOSONG";
                case ItemType.Lettuce: return "Selada Segar";
                case ItemType.LettuceChopped: return "Selada Potong";
                case ItemType.Tomato: return "Tomat Segar";
                case ItemType.TomatoSliced: return "Tomat Iris";
                case ItemType.Cheese: return "Keju Cheddar";
                case ItemType.BunTop: return "Roti Atas";
                default: return "Kosong";
            }
        }

        public static Vector3 GetScale(ItemType type)
        {
            switch (type)
            {
                case ItemType.Plate:
                    return new Vector3(0.95f, 0.95f, 1f);
                case ItemType.Tomato:
                    return new Vector3(0.70f, 0.70f, 1f);
                default:
                    // Semua bahan burger proporsional di skala 0.85
                    return new Vector3(0.85f, 0.85f, 1f);
            }
        }

        private static Dictionary<ItemType, Sprite> spriteCache = new Dictionary<ItemType, Sprite>();
        private static Dictionary<ItemType, Texture2D> textureCache = new Dictionary<ItemType, Texture2D>();

        public static string GetSpriteName(ItemType type)
        {
            switch (type)
            {
                case ItemType.Plate: return "plate";
                case ItemType.BunBottom: return "bun_bottom";
                case ItemType.PattyRaw: return "patty_raw";
                case ItemType.PattyCooked: return "patty_cooked";
                case ItemType.PattyBurnt: return "patty_burnt";
                case ItemType.Lettuce: return "lettuce";
                case ItemType.LettuceChopped: return "lettuce_chopped";
                case ItemType.Tomato: return "tomato";
                case ItemType.TomatoSliced: return "tomato_sliced";
                case ItemType.Cheese: return "cheese";
                case ItemType.BunTop: return "bun_top";
                default: return null;
            }
        }

        public static Sprite GetSprite(ItemType type)
        {
            if (spriteCache.TryGetValue(type, out Sprite sp) && sp != null)
            {
                return sp;
            }

            string name = GetSpriteName(type);
            if (string.IsNullOrEmpty(name)) return null;

            sp = Resources.Load<Sprite>("Sprites/" + name);
#if UNITY_EDITOR
            if (sp == null)
            {
                sp = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Sprites/{name}.png");
            }
#endif
            if (sp != null)
            {
                spriteCache[type] = sp;
            }
            return sp;
        }

        public static Texture2D GetTexture(ItemType type)
        {
            if (textureCache.TryGetValue(type, out Texture2D tex) && tex != null)
            {
                return tex;
            }

            string name = GetSpriteName(type);
            if (string.IsNullOrEmpty(name)) return null;

            tex = Resources.Load<Texture2D>("Sprites/" + name);
            if (tex == null)
            {
                Sprite sp = GetSprite(type);
                if (sp != null && sp.texture != null)
                {
                    tex = sp.texture;
                }
            }
            if (tex != null)
            {
                textureCache[type] = tex;
            }
            return tex;
        }

        public static bool IsBurgerIngredient(ItemType type)
        {
            return type == ItemType.BunBottom ||
                   type == ItemType.PattyCooked ||
                   type == ItemType.Cheese ||
                   type == ItemType.LettuceChopped ||
                   type == ItemType.TomatoSliced ||
                   type == ItemType.BunTop;
        }

        public static int GetItemValue(ItemType type)
        {
            switch (type)
            {
                case ItemType.BunBottom:
                case ItemType.BunTop:
                    return 3;
                case ItemType.Lettuce:
                case ItemType.Tomato:
                    return 4;
                case ItemType.LettuceChopped:
                case ItemType.TomatoSliced:
                    return 6;
                case ItemType.Cheese:
                    return 5;
                case ItemType.PattyRaw:
                    return 6;
                case ItemType.PattyCooked:
                    return 10;
                case ItemType.PattyBurnt:
                    return 4;
                case ItemType.Plate:
                    return 5;
                default:
                    return 2;
            }
        }

        public static int CalculateWasteValue(KitchenItem item)
        {
            if (item == null) return 0;

            int total = GetItemValue(item.itemType);

            // Jika membawa piring dengan tumpukan bahan burger, hitung seluruh nilai bahan di dalamnya
            if (item.stackedIngredients != null && item.stackedIngredients.Count > 0)
            {
                foreach (ItemType ing in item.stackedIngredients)
                {
                    total += GetItemValue(ing);
                }
            }

            return total;
        }

        public static string GetItemDescription(KitchenItem item)
        {
            if (item == null) return "Bahan Makanan";

            if (item.IsPlate())
            {
                if (item.stackedIngredients == null || item.stackedIngredients.Count == 0)
                {
                    return "Piring Kosong";
                }
                else
                {
                    return $"Burger ({item.stackedIngredients.Count} Bahan)";
                }
            }

            return GetName(item.itemType);
        }
    }
}
