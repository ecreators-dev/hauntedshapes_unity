#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using TMPro;
using System.Collections.Generic;

namespace Game.Edit
{
    /// <summary>
    /// Erstellt einen "Letter Container" neben dem Quellobjekt.
    /// Darin für jede Zeile ein Child ("Line n").
    /// In jeder Zeile werden vollständige Kopien des Quell-GameObjects erzeugt,
    /// jede enthält nur den jeweiligen Buchstaben.
    /// Quelle bleibt erhalten, aber die TextMeshPro-Komponente wird deaktiviert.
    /// </summary>
    public sealed class TMP_LetterContainerBuilder : EditorWindow
    {
        private TextMeshProUGUI targetText;
        private GameObject letterContainer;
        private readonly List<GameObject> clones = new();

        [MenuItem("Tools/TMP Letter Container Builder")]
        private static void Open() => GetWindow<TMP_LetterContainerBuilder>("TMP Letter Container Builder");

        private void OnGUI()
        {
            EditorGUILayout.LabelField("TMP Letter Container Builder", EditorStyles.boldLabel);
            targetText = (TextMeshProUGUI)EditorGUILayout.ObjectField(
                "Target TMP Object", targetText, typeof(TextMeshProUGUI), true);

            EditorGUILayout.Space();
            using (new EditorGUI.DisabledScope(!targetText))
            {
                if (GUILayout.Button("Erzeuge Letter Container"))
                    BuildLetterContainer();

                if (GUILayout.Button("Rückgängig machen"))
                    UndoBuild();
            }
        }

        /// <summary>
        /// Baut die Struktur: Letter Container → Line n → Buchstaben-Klone.
        /// </summary>
        private void BuildLetterContainer()
        {
            if (!targetText)
                return;

            Undo.RegisterFullObjectHierarchyUndo(targetText.gameObject, "Build Letter Container");
            UndoBuild();

            targetText.ForceMeshUpdate();
            TMP_TextInfo info = targetText.textInfo;
            string fullText = targetText.text;
            if (string.IsNullOrEmpty(fullText))
                return;

            // Container neben dem Quellobjekt anlegen
            letterContainer = new GameObject(targetText.name + "_LetterContainer");
            Undo.RegisterCreatedObjectUndo(letterContainer, "Create Letter Container");

            // gleiche Parent-Ebene wie Quelle
            letterContainer.transform.SetParent(targetText.transform.parent, false);

            GameObject sourceGO = targetText.gameObject;

            for (int lineIdx = 0; lineIdx < info.lineCount; lineIdx++)
            {
                TMP_LineInfo line = info.lineInfo[lineIdx];

                // Zeilenobjekt
                GameObject lineObj = new GameObject($"Line_{lineIdx}");
                Undo.RegisterCreatedObjectUndo(lineObj, "Create Line Object");
                lineObj.transform.SetParent(letterContainer.transform, false);

                // Fix: inkludiere den letzten Buchstaben der Zeile
                int lastChar = line.lastCharacterIndex;
                if (lastChar >= info.characterCount)
                    lastChar = info.characterCount - 1;

                for (int charIdx = line.firstCharacterIndex; charIdx <= lastChar; charIdx++)
                {
                    TMP_CharacterInfo charInfo = info.characterInfo[charIdx];
                    char c = charInfo.character;

                    if (char.IsWhiteSpace(c) || !charInfo.isVisible)
                        continue;

                    // Vollständige Kopie des GameObjects
                    GameObject clone = (GameObject)PrefabUtility.InstantiatePrefab(sourceGO);
                    if (clone == null)
                        clone = Instantiate(sourceGO);
                    Undo.RegisterCreatedObjectUndo(clone, "Create Letter Clone");
                    clone.name = $"{c}_Letter_{charIdx}";
                    clone.transform.SetParent(lineObj.transform, false);

                    // Nur den aktuellen Buchstaben behalten
                    TextMeshProUGUI tmp = clone.GetComponent<TextMeshProUGUI>();
                    if (tmp)
                    {
                        tmp.text = c.ToString();
                        tmp.ForceMeshUpdate();
                    }

                    // Relative Positionierung in der Zeile
                    RectTransform rect = clone.GetComponent<RectTransform>();
                    if (rect != null)
                    {
                        Vector3 localPos = charInfo.bottomLeft;
                        rect.anchoredPosition = localPos;
                    }

                    clones.Add(clone);
                }
            }

            // Nur die TMP-Komponente der Quelle deaktivieren
            targetText.enabled = false;

            Debug.Log($"Letter Container '{letterContainer.name}' erstellt mit {info.lineCount} Zeilen und {clones.Count} Zeichen.");
        }

        /// <summary>
        /// Entfernt Letter Container und reaktiviert die ursprüngliche TMP-Komponente.
        /// </summary>
        private void UndoBuild()
        {
            if (letterContainer)
                Undo.DestroyObjectImmediate(letterContainer);

            clones.Clear();

            if (targetText)
                targetText.enabled = true;
        }
    }
}
#endif
