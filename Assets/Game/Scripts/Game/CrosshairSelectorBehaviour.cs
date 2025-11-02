using Game.Interaction;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    /// <summary>
    /// Erfasst das Objekt, welches sich im Crosshair befindet für den aktuellen Spieler
    /// </summary>
    public sealed class CrosshairSelectorBehaviour : MonoBehaviour, ICrosshairSelector, IGameLoadProgress
    {
        const int alleLayerMasks = ~0;
        private const int PLAYER_ANY = 0;
        [Min(0)]
        [SerializeField] private float selectableDistance = 1.5f;

        [SerializeField] private LayerMask mask = (LayerMask)alleLayerMasks;

        [Min(0)]
        [SerializeField] private float radiusCrosshair = 0.3f;

        [SerializeField] private Color unselectColor = Color.white;
        [SerializeField] private Color selectedColor = Color.yellow;
        [SerializeField] private Color lockedColor = Color.orangeRed;

        [Header("Optional")]
        [SerializeField] private TextMeshProUGUI textfield;

        [SerializeField] private Image crossImage;

        private static ICrosshairSelector? instance;

        public static ICrosshairSelector GetInstance() => instance ??= FindFirstObjectByType<CrosshairSelectorBehaviour>();

        private void Awake()
        {
            instance ??= this;
        }

        private void Start()
        {
            HideText();
        }

        public void HideText()
        {
            if (textfield != null)
            {
                textfield.text = "";
            }

            if (crossImage != null)
            {
                crossImage.color = unselectColor;
            }
        }

        public void ShowTarget(ICrosshairSelectableObject target, bool active)
        {
            if (textfield == null || target == null)
            {
                return;
            }

            textfield.text = target.GetText();

            if (crossImage != null)
            {
                if (target.IsLocked)
                {
                    crossImage.color = lockedColor;
                }
                else if (active)
                {
                    crossImage.color = selectedColor;
                }
                else
                {
                    HideText();
                    crossImage.color = lockedColor;
                }

            }
        }

        /// <summary>
        /// Liefert alle wählbaren Objekte, aufgelistet nach Entfernung
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ICrosshairSelectableObject? FindInSelection<T>()
            where T : Component, ICrosshairSelectableObject
        {
            ICrosshairSelectableObject? found = null;

            // findet GameObject mit korrektem Layer und Collider Hit, was nicht blockiert ist
            Vector2 center = GetCrosshairScreenCenter();
            Vector3 hitPosition = CrosshairRayCastNonBlocked(center, out GameObject surfaceCollisionObject);

            if (surfaceCollisionObject == null)
            {
                return null;
            }

            // Finde das Crosshair-erlaubte-Trefferobjekt
            ICrosshairSelectableObject selected = FilterCrosshairObjectInfos<T>(surfaceCollisionObject);

            // wenn das kein Ziel ist
            if (selected == null || !selected.CanSelect)
            {
                return found;
            }

            // wennes ein Interaktives Crosshair-Objekt-Ziel ist
            bool activatable = true;
            if (selected is IInteractiveObject obj)
            {
                // interaktive Objekte sind ziele von Crosshair-Ray, aber sie können einen Bereich haben,
                // in denen der Spieler / oder anderes erst interagieren darf
                (bool inRange, bool hasZone) = this.GetGameController().IsPlayerInRange(obj, PLAYER_ANY);
                if (hasZone && !inRange) // ist der Spiel nah?! wenn nicht dann nicht aktivierbar
                {
                    activatable = false;
                    print($"[{GetName(selected)}]: Reset because of missing inRange!");
                }

                if (!hasZone)
                {
                    print("No Trigger Zone");
                }
            }

            if (activatable)
            {
                found = selected;
            }

            return found;
        }

        private static string GetName(ICrosshairSelectableObject selected)
        {
            return ((MonoBehaviour)selected).gameObject.name;
        }

        private static T FilterCrosshairObjectInfos<T>(GameObject surfaceCollisionObject) where T : Component, ICrosshairSelectableObject
        {
            return surfaceCollisionObject.GetComponentInChildren<T>() ?? surfaceCollisionObject.GetComponentInParent<T>();
        }

        private static Vector2 GetCrosshairScreenCenter()
        {
            return new Vector2(Screen.width, Screen.height) * 0.5f;
        }

        private static void DebugDrawCollider(BoxCollider zone)
        {
            Transform t = zone.transform;

            var c = zone.center;
            var e = zone.size * 0.5f;

            Vector3[] v = new Vector3[8];
            v[0] = t.TransformPoint(c + new Vector3(-e.x, e.y, -e.z));
            v[1] = t.TransformPoint(c + new Vector3(e.x, e.y, -e.z));
            v[2] = t.TransformPoint(c + new Vector3(e.x, e.y, e.z));
            v[3] = t.TransformPoint(c + new Vector3(-e.x, e.y, e.z));

            v[4] = t.TransformPoint(c + new Vector3(-e.x, -e.y, -e.z));
            v[5] = t.TransformPoint(c + new Vector3(e.x, -e.y, -e.z));
            v[6] = t.TransformPoint(c + new Vector3(e.x, -e.y, e.z));
            v[7] = t.TransformPoint(c + new Vector3(-e.x, -e.y, e.z));

            Color col = Color.green;

            // top
            Debug.DrawLine(v[0], v[1], col);
            Debug.DrawLine(v[1], v[2], col);
            Debug.DrawLine(v[2], v[3], col);
            Debug.DrawLine(v[3], v[0], col);

            // bottom
            Debug.DrawLine(v[4], v[5], col);
            Debug.DrawLine(v[5], v[6], col);
            Debug.DrawLine(v[6], v[7], col);
            Debug.DrawLine(v[7], v[4], col);

            // verticals
            Debug.DrawLine(v[0], v[4], col);
            Debug.DrawLine(v[1], v[5], col);
            Debug.DrawLine(v[2], v[6], col);
            Debug.DrawLine(v[3], v[7], col);
        }

        // Es bedeutet, dass wenn ein untergeordneter Collider in einem Objekt eine Component bestimmen soll
        // die aber nur in einem Parent GameObject ist, dass manuell in der Hierarchie nach oben gesucht werden muss
        /// <summary>
        /// Findet alle GameObjects, die im Radius vom Crosshair sind. Sortiert nach Entfernung (aufsteigend).
        /// Es werden nur GameObjects gefunden, deren Raycast auf einen Collider getroffen ist
        /// </summary>
        /// <param name="screenPosition"></param>
        /// <param name="found"></param>
        /// <returns></returns>
        private Vector3 CrosshairRayCastNonBlocked(Vector2 screenPosition, out GameObject found)
        {
            // finde alle Objekte am Crosshair innerhalb eines radius um die Screenposition des Crosshair
            Camera cam = Camera.main;
            found = null;
            if (cam == null)
            {
                return Vector3.zero;
            }

            Ray ray = cam.ScreenPointToRay(screenPosition);

            // Alle Treffer im Radius und in Reichweite
            bool hit = Physics.SphereCast(ray, radiusCrosshair, out RaycastHit hits, selectableDistance, layerMask: mask, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
            if (!hit)
            {
                return Vector3.zero;
            }

            GameObject target = GetObject(hits);

            // finetuning: Seitlich von einer Tür stehen und durch die Wand eine Tür anvisieren, soll kein Treffer sein:
            // nur exakt auf der sichtbaren Tür klicken bedeutet treffer!
            bool hitOther = Physics.Raycast(ray, out RaycastHit hitsOther, selectableDistance, layerMask: alleLayerMasks, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
            if (hitOther && GetObject(hitsOther) != target)
            {
                return Vector3.zero;
            }

            Debug.DrawLine(ray.GetPoint(0), ray.GetPoint(hitsOther.distance), Color.red);

            found = target;

            // Rückgabe der Aufschlagposition des nächsten Objekts
            return hits.point;
        }

        private static GameObject GetObject(RaycastHit input)
        {
            return input.collider.gameObject;
        }
    }
}
