using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BurgerGame.Editor
{
    [InitializeOnLoad]
    public static class BakeSceneOnLoad
    {
        static BakeSceneOnLoad()
        {
            EditorApplication.delayCall += ExecuteBake;
        }

        private static void ExecuteBake()
        {
            if (Application.isPlaying) return;

            AssetDatabase.ImportAsset("Assets/Resources/Audio", ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            KitchenSceneBuilder.BakeAllScenes();
            Debug.Log("<color=#2ECC71><b>[BakeSceneOnLoad]</b> Seluruh scene berhasil di-bake dengan posisi meja baru (Y -0.9f) dan customer di bawah!</color>");
        }
    }
}

