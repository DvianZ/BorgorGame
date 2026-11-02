using System.IO;
using UnityEditor;
using UnityEngine;

namespace BurgerGame.Editor
{
    [InitializeOnLoad]
    public static class UITextureOptimizer
    {
        static UITextureOptimizer()
        {
            EditorApplication.delayCall += () =>
            {
                OptimizeAllUITextures();
                OptimizeCharacterSprites();
            };
        }

        [MenuItem("Burger Game/Optimize All Textures (UI & Character)")]
        public static void OptimizeAll()
        {
            OptimizeAllUITextures();
            OptimizeCharacterSprites();
        }

        [MenuItem("Burger Game/Optimize UI Textures (Lossless High-Res)")]
        public static void OptimizeAllUITextures()
        {
            string[] searchFolders = new string[]
            {
                "Assets/Sprites/UI",
                "Assets/Resources/Sprites/UI"
            };

            int optimizedCount = 0;

            AssetDatabase.StartAssetEditing();
            try
            {
                string[] guids = AssetDatabase.FindAssets("t:Texture2D", searchFolders);
                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    if (string.IsNullOrEmpty(path) || (!path.EndsWith(".png") && !path.EndsWith(".jpg") && !path.EndsWith(".jpeg"))) continue;

                    TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
                    if (ti == null) continue;

                    bool changed = false;

                    if (ti.textureType != TextureImporterType.Sprite)
                    {
                        ti.textureType = TextureImporterType.Sprite;
                        changed = true;
                    }

                    if (ti.spriteImportMode != SpriteImportMode.Single)
                    {
                        ti.spriteImportMode = SpriteImportMode.Single;
                        changed = true;
                    }

                    if (ti.mipmapEnabled)
                    {
                        ti.mipmapEnabled = false;
                        changed = true;
                    }

                    if (ti.filterMode != FilterMode.Bilinear)
                    {
                        ti.filterMode = FilterMode.Bilinear;
                        changed = true;
                    }

                    if (ti.maxTextureSize < 4096)
                    {
                        ti.maxTextureSize = 4096;
                        changed = true;
                    }

                    if (ti.textureCompression != TextureImporterCompression.Uncompressed)
                    {
                        ti.textureCompression = TextureImporterCompression.Uncompressed;
                        changed = true;
                    }

                    if (!ti.alphaIsTransparency)
                    {
                        ti.alphaIsTransparency = true;
                        changed = true;
                    }

                    if (!ti.isReadable)
                    {
                        ti.isReadable = true;
                        changed = true;
                    }

                    // Default platform settings
                    TextureImporterPlatformSettings def = ti.GetDefaultPlatformTextureSettings();
                    if (def.maxTextureSize < 4096 || def.textureCompression != TextureImporterCompression.Uncompressed || def.format != TextureImporterFormat.RGBA32)
                    {
                        def.maxTextureSize = 4096;
                        def.textureCompression = TextureImporterCompression.Uncompressed;
                        def.format = TextureImporterFormat.RGBA32;
                        ti.SetPlatformTextureSettings(def);
                        changed = true;
                    }

                    // Standalone platform settings
                    TextureImporterPlatformSettings standalone = ti.GetPlatformTextureSettings("Standalone");
                    if (!standalone.overridden || standalone.maxTextureSize < 4096 || standalone.textureCompression != TextureImporterCompression.Uncompressed || standalone.format != TextureImporterFormat.RGBA32)
                    {
                        standalone.overridden = true;
                        standalone.maxTextureSize = 4096;
                        standalone.textureCompression = TextureImporterCompression.Uncompressed;
                        standalone.format = TextureImporterFormat.RGBA32;
                        ti.SetPlatformTextureSettings(standalone);
                        changed = true;
                    }

                    if (changed)
                    {
                        EditorUtility.SetDirty(ti);
                        ti.SaveAndReimport();
                        optimizedCount++;
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.Refresh();
            Debug.Log($"<color=#2ECC71><b>[UITextureOptimizer]</b> Berhasil mengoptimasi {optimizedCount} aset UI ke RGBA32 Uncompressed 4096px (Lossless, tajam & tidak pecah)!</color>");
        }

        [MenuItem("Burger Game/Optimize Character Sprites (Crisp Pixel-Perfect)")]
        public static void OptimizeCharacterSprites()
        {
            string[] searchFolders = new string[]
            {
                "Assets/Sprites/Character",
                "Assets/Resources/Sprites/Character"
            };

            int count = 0;
            AssetDatabase.StartAssetEditing();
            try
            {
                string[] guids = AssetDatabase.FindAssets("t:Texture2D", searchFolders);
                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    if (string.IsNullOrEmpty(path) || !path.EndsWith(".png")) continue;

                    TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
                    if (ti == null) continue;

                    bool changed = false;

                    if (ti.textureType != TextureImporterType.Sprite)
                    {
                        ti.textureType = TextureImporterType.Sprite;
                        changed = true;
                    }

                    if (ti.spriteImportMode != SpriteImportMode.Single)
                    {
                        ti.spriteImportMode = SpriteImportMode.Single;
                        changed = true;
                    }

                    // 200 PPU for 256x384 canvas -> 1.28 x 1.92 units in world
                    if (Mathf.Abs(ti.spritePixelsPerUnit - 200f) > 0.01f)
                    {
                        ti.spritePixelsPerUnit = 200f;
                        changed = true;
                    }

                    // Point filter keeps pixel art lines ultra-crisp without blur or smear
                    if (ti.filterMode != FilterMode.Point)
                    {
                        ti.filterMode = FilterMode.Point;
                        changed = true;
                    }

                    if (ti.mipmapEnabled)
                    {
                        ti.mipmapEnabled = false;
                        changed = true;
                    }

                    if (ti.textureCompression != TextureImporterCompression.Uncompressed)
                    {
                        ti.textureCompression = TextureImporterCompression.Uncompressed;
                        changed = true;
                    }

                    if (!ti.alphaIsTransparency)
                    {
                        ti.alphaIsTransparency = true;
                        changed = true;
                    }

                    if (!ti.isReadable)
                    {
                        ti.isReadable = true;
                        changed = true;
                    }

                    // Standalone format
                    TextureImporterPlatformSettings standalone = ti.GetPlatformTextureSettings("Standalone");
                    if (!standalone.overridden || standalone.textureCompression != TextureImporterCompression.Uncompressed || standalone.format != TextureImporterFormat.RGBA32)
                    {
                        standalone.overridden = true;
                        standalone.maxTextureSize = 2048;
                        standalone.textureCompression = TextureImporterCompression.Uncompressed;
                        standalone.format = TextureImporterFormat.RGBA32;
                        ti.SetPlatformTextureSettings(standalone);
                        changed = true;
                    }

                    if (changed)
                    {
                        EditorUtility.SetDirty(ti);
                        ti.SaveAndReimport();
                        count++;
                    }
                }

                // Also optimize box_player.png
                string[] playerPaths = new string[]
                {
                    "Assets/Sprites/box_player.png",
                    "Assets/Resources/Sprites/box_player.png"
                };
                foreach (string p in playerPaths)
                {
                    TextureImporter ti = AssetImporter.GetAtPath(p) as TextureImporter;
                    if (ti != null)
                    {
                        ti.textureType = TextureImporterType.Sprite;
                        ti.spriteImportMode = SpriteImportMode.Single;
                        ti.spritePixelsPerUnit = 200f;
                        ti.filterMode = FilterMode.Point;
                        ti.mipmapEnabled = false;
                        ti.textureCompression = TextureImporterCompression.Uncompressed;
                        ti.alphaIsTransparency = true;
                        EditorUtility.SetDirty(ti);
                        ti.SaveAndReimport();
                        count++;
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.Refresh();
            Debug.Log($"<color=#3498DB><b>[UITextureOptimizer]</b> Berhasil mengoptimasi {count} aset karakter ke RGBA32 Uncompressed, 200 PPU & FilterMode.Point (Crisp & tidak pecah)!</color>");
        }
    }
}
