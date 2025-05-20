using TMPro;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class ReplaceAllTMPFonts : MonoBehaviour
    {
        [MenuItem("Tools/Replace All TMP Fonts")]
        public static void ReplaceFonts()
        {
            TMP_FontAsset newFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
                AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:TMP_FontAsset")[0]));

            if (newFont == null)
            {
                Debug.LogError("No TMP_FontAsset found in the project!");
                return;
            }

            TextMeshProUGUI[] allTexts = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();

            int replacedCount = 0;

            foreach (TextMeshProUGUI text in allTexts)
            {
                Undo.RecordObject(text, "Change TMP Font");
                text.font = newFont;
                replacedCount++;
            }

            Debug.Log($"Replaced fonts on {replacedCount} TextMeshPro objects with {newFont.name}");
        }
    }
}