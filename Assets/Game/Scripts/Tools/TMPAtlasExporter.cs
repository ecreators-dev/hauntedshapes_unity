#if UNITY_EDITOR
using UnityEditor;

using UnityEngine;

public static class TMPAtlasExporter
{
    [MenuItem("Tools/Export TMP Atlas")]
    public static void ExportTMPAtlas()
    {
        var selected = Selection.activeObject as TMPro.TMP_FontAsset;
        if (!selected || !selected.atlasTexture)
        {
            Debug.LogError("Kein TMP Font Asset oder Atlas gefunden!");
            return;
        }

        string path = EditorUtility.SaveFilePanel("Speichern als PNG", "Assets", selected.name, "png");
        if (string.IsNullOrEmpty(path))
            return;

        var tex = selected.atlasTexture;
        var bytes = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.Refresh();
        Debug.Log("Atlas exportiert nach: " + path);
    }
}
#endif