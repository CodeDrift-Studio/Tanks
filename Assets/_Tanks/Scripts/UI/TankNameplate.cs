using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tanks.Complete
{
    // A small world-space "YOU" nameplate that floats above a tank. It is a separate root object (not a child of the
    // tank) so it never inherits the tank's rotation, and it simply mirrors the tank's position every frame.
    // Presentation only: it has no colliders, no raycast targets and never touches gameplay components.
    public class TankNameplate : MonoBehaviour
    {
        private const float k_ScreenHeightFraction = 0.05f;  // Plate height as a fraction of the visible screen height
        private const float k_BaseHeight = 1.9f;             // World units above the tank pivot (tank is ~1.7 tall)
        private const float k_ExtraLiftFraction = 0.05f;     // Extra lift along camera-up, as a fraction of the visible half height
        private const float k_CanvasHeight = 56f;            // Canvas size in canvas units (pixels at scale 1)
        private const float k_CanvasWidth = 150f;

        private Transform m_Target;
        private Canvas m_Canvas;

        // Creates the plate for 'target'. 'font' is reused from existing UI so the style matches the game's HUD.
        public static TankNameplate Create(Transform target, string label, TMP_FontAsset font, int layer)
        {
            var root = new GameObject("Nameplate_" + label);
            root.layer = layer;
            var plate = root.AddComponent<TankNameplate>();
            plate.m_Target = target;

            var canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.overrideSorting = true;
            canvas.sortingOrder = 10;
            plate.m_Canvas = canvas;
            var canvasRect = root.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(k_CanvasWidth, k_CanvasHeight);

            // Dark translucent plate for contrast against the bright desert
            var bgObject = new GameObject("Background", typeof(RectTransform));
            bgObject.layer = layer;
            bgObject.transform.SetParent(root.transform, false);
            var bgRect = (RectTransform)bgObject.transform;
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = bgRect.offsetMax = Vector2.zero;
            var bg = bgObject.AddComponent<Image>();
            bg.color = new Color(0.05f, 0.05f, 0.05f, 0.7f);
            bg.raycastTarget = false;

            var textObject = new GameObject("Label", typeof(RectTransform));
            textObject.layer = layer;
            textObject.transform.SetParent(root.transform, false);
            var textRect = (RectTransform)textObject.transform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = textRect.offsetMax = Vector2.zero;
            var text = textObject.AddComponent<TextMeshProUGUI>();
            if (font != null) text.font = font;
            text.text = label;
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 40f;
            text.color = Color.white;
            text.outlineColor = Color.black;
            text.outlineWidth = 0.2f;
            text.raycastTarget = false;

            plate.UpdatePlate();
            return plate;
        }

        private void LateUpdate()
        {
            UpdatePlate();
        }

        private void UpdatePlate()
        {
            // The tank was destroyed (e.g. scene teardown): nothing left to follow.
            if (m_Target == null)
            {
                Destroy(gameObject);
                return;
            }

            var cam = Camera.main;
            // Hidden while the tank is dead/deactivated, or if there is no camera to face.
            bool visible = cam != null && m_Target.gameObject.activeInHierarchy;
            if (m_Canvas.enabled != visible)
                m_Canvas.enabled = visible;
            if (!visible) return;

            var camTransform = cam.transform;

            // Visible half-height of the view at the tank, used to keep the plate a constant size on screen even
            // while CameraControl zooms.
            float halfHeight = cam.orthographic
                ? cam.orthographicSize
                : Vector3.Dot(m_Target.position - camTransform.position, camTransform.forward) * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            halfHeight = Mathf.Max(halfHeight, 0.01f);

            transform.position = m_Target.position + Vector3.up * k_BaseHeight + camTransform.up * (halfHeight * k_ExtraLiftFraction);
            transform.rotation = camTransform.rotation;   // camera-facing, independent of the tank's rotation
            transform.localScale = Vector3.one * (halfHeight * 2f * k_ScreenHeightFraction / k_CanvasHeight);
        }
    }
}
