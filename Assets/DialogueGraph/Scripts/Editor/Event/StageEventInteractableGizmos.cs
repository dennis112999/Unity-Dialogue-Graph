#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace Dennis.Tools.DialogueGraph.Sample
{
    [InitializeOnLoad]
    public static class StageEventInteractableGizmos
    {
        // Color 
        private static readonly Color SolidDiscColor = new Color(1f, 0.8f, 0.2f, 0.2f);
        private static readonly Color WireDiscColor = Color.yellow;

        // GUI or labelStyle
        private static Texture2D _labelBackgroundTex;
        private static GUIStyle _labelStyle;

        [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
        static void DrawGizmos(StageEventInteractable interactable, GizmoType gizmoType)
        {
            if (interactable == null) return;

            DrawInteractionRange(interactable);
            DrawInteractionLabel(interactable);
        }

        /// <summary>
        /// Init GUI Label Style
        /// </summary>
        private static void InitLabelStyle()
        {
            if (_labelBackgroundTex == null)
            {
                _labelBackgroundTex = MakeTex(1, 1, new Color(1f, 0.85f, 1.0f, 1.0f));
            }

            if (_labelStyle == null)
            {
                _labelStyle = new GUIStyle(GUI.skin.box)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 12,
                    fontStyle = FontStyle.Bold
                };
                _labelStyle.normal.textColor = Color.black;
                _labelStyle.normal.background = _labelBackgroundTex;
            }
        }

        /// <summary>
        /// Draw Interaction Range
        /// </summary>
        /// <param name="interactable">The target StageEventInteractable</param>
        private static void DrawInteractionRange(StageEventInteractable interactable)
        {
            var circleCollider = interactable.GetComponent<CircleCollider2D>();
            if (circleCollider == null)
            {
                Debug.LogWarning($"Missing Component: CircleCollider2D on {interactable.name}", interactable);
                return;
            }

            Vector3 center = interactable.transform.position + (Vector3)circleCollider.offset;
            float radius = circleCollider.radius;

            // Filled disc to show interaction zone
            Handles.color = SolidDiscColor;
            Handles.DrawSolidDisc(center, Vector3.back, radius);

            // Outline for visual clarity
            Handles.color = WireDiscColor;
            Handles.DrawWireDisc(center, Vector3.back, radius);
        }

        /// <summary>
        /// Draw Interaction Label
        /// </summary>
        /// <param name="interactable">The target StageEventInteractable</param>
        private static void DrawInteractionLabel(StageEventInteractable interactable)
        {
            InitLabelStyle();

            // Determine label text
            string labelText = string.IsNullOrEmpty(interactable.InteractionId) ? "Story" : interactable.InteractionId;

            // Position label above the object
            Vector3 worldLabelPosition = interactable.transform.position + Vector3.up * 2f;
            Vector2 screenLabelPosition = HandleUtility.WorldToGUIPoint(worldLabelPosition);

            // Measure label size
            Vector2 labelSize = _labelStyle.CalcSize(new GUIContent(labelText));

            // Draw label in Scene view
            Handles.BeginGUI();
            GUI.Label(
                new Rect(
                    screenLabelPosition.x - labelSize.x / 2f,
                    screenLabelPosition.y - labelSize.y / 2f,
                    labelSize.x,
                    labelSize.y
                ),
                labelText,
                _labelStyle
            );
            Handles.EndGUI();
        }

        private static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++) pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}

#endif
