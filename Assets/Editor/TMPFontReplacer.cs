using UnityEngine;
using UnityEditor;
using TMPro;

public class TMPFontReplacer : EditorWindow
{
    private TMP_FontAsset newFont;

    [MenuItem("Tools/Replace TMP Fonts")]
    static void Init()
    {
        TMPFontReplacer window = (TMPFontReplacer)EditorWindow.GetWindow(typeof(TMPFontReplacer));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Replace TMP Fonts", EditorStyles.boldLabel);
        newFont = (TMP_FontAsset)EditorGUILayout.ObjectField("New Font Asset", newFont, typeof(TMP_FontAsset), false);

        if (GUILayout.Button("Replace Fonts In Scene"))
        {
            ReplaceFontsInScene();
        }
    }

    void ReplaceFontsInScene()
    {
        if (newFont == null)
        {
            Debug.LogWarning("No font selected!");
            return;
        }

        var texts = FindObjectsOfType<TextMeshProUGUI>(true); // for UI texts
        foreach (var tmp in texts)
        {
            Undo.RecordObject(tmp, "Replace TMP Font");
            tmp.font = newFont;
            EditorUtility.SetDirty(tmp);
        }

        var worldTexts = FindObjectsOfType<TextMeshPro>(true); // for 3D world texts
        foreach (var tmp in worldTexts)
        {
            Undo.RecordObject(tmp, "Replace TMP Font");
            tmp.font = newFont;
            EditorUtility.SetDirty(tmp);
        }

        Debug.Log("All TMP fonts replaced.");
    }
}