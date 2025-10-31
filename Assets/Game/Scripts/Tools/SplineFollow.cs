using Game;

using Unity.Mathematics;

using UnityEngine;
using UnityEngine.Splines;

[ExecuteAlways]
public class SplineFollow : MonoBehaviour, IGameLoadProgress
{
    [Header("Bewegt und orientiert dieses Objekt entlang einer Spline")]
    [SerializeField] private SplineContainer follow;
    [SerializeField] private int splineSelect;
    [SerializeField, Range(0f, 1f)] private float progressOnSpline;
    [SerializeField] private float speed = 3f;
    [Tooltip("Wenn aktiv, wird nach Ende der Spline wieder von vorn begonnen")]
    [SerializeField] private bool continueMode = true;

    private Transform selfTransform;

    private void Awake()
    {
        selfTransform = transform;
    }

    private void Update()
    {
        if (follow == null || follow.Splines.Count == 0)
            return;

        // Wähle Spline anhand Index (mit Modulo bei continueMode)
        int index = Mathf.Clamp(splineSelect, 0, follow.Splines.Count - 1);
        if (continueMode)
            index = splineSelect % follow.Splines.Count;

        Spline selected = follow.Splines[index];

        // Fortschritt entlang der Spline basierend auf Geschwindigkeit erhöhen
        float splineLength = selected.GetLength();
        
        if (progressOnSpline > 1f)
        {
            if (continueMode)
                progressOnSpline %= 1f;
            else
                progressOnSpline = 1f;
        }

        // Spline auswerten
        if (selected.Evaluate(progressOnSpline, out float3 pos, out float3 tangent, out float3 up))
        {
            Vector3 position = pos;
            Vector3 forward = Vector3.Normalize(tangent);
            Vector3 upVec = up;

            // Position & Rotation setzen
            selfTransform.localPosition = position;
            selfTransform.localRotation = Quaternion.LookRotation(forward, upVec);
        }
    }
}