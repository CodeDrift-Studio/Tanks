using UnityEngine;

namespace Tanks.Complete
{
    // Pushes on-screen controls away from screen edges that are covered by notches, rounded corners or the gesture bar.
    // Controls keep their authored anchored position (their "base" position); the safe-area inset is added on top, so
    // on a screen without cut-outs nothing moves. Controls must be anchored to a screen corner (bottom-left / bottom-right).
    public class MobileControlSafeArea : MonoBehaviour
    {
        [Tooltip("Controls anchored to the bottom-left corner: pushed right by the left inset and up by the bottom inset")]
        [SerializeField] private RectTransform[] m_LeftControls;

        [Tooltip("Controls anchored to the bottom-right corner: pushed left by the right inset and up by the bottom inset")]
        [SerializeField] private RectTransform[] m_RightControls;

        private Canvas m_Canvas;
        private Vector2[] m_LeftBase;
        private Vector2[] m_RightBase;
        private Rect m_AppliedSafeArea;
        private Vector2Int m_AppliedResolution;
        private float m_AppliedScale;

        private void Awake()
        {
            m_Canvas = GetComponent<Canvas>();
            m_LeftBase = CaptureBase(m_LeftControls);
            m_RightBase = CaptureBase(m_RightControls);
        }

        private void OnEnable()
        {
            Apply();
        }

        private void Update()
        {
            // Safe area changes on rotation / split-screen; cheap comparison, no allocations.
            if (Screen.safeArea != m_AppliedSafeArea || Screen.width != m_AppliedResolution.x || Screen.height != m_AppliedResolution.y
                || (m_Canvas != null && m_Canvas.scaleFactor != m_AppliedScale))
                Apply();
        }

        private static Vector2[] CaptureBase(RectTransform[] controls)
        {
            var result = new Vector2[controls != null ? controls.Length : 0];
            for (int i = 0; i < result.Length; ++i)
                result[i] = controls[i] != null ? controls[i].anchoredPosition : Vector2.zero;
            return result;
        }

        private void Apply()
        {
            if (m_Canvas == null || m_Canvas.scaleFactor <= 0f)
                return;

            Rect safe = Screen.safeArea;
            m_AppliedSafeArea = safe;
            m_AppliedResolution = new Vector2Int(Screen.width, Screen.height);

            // Screen pixels -> canvas units
            float scale = m_Canvas.scaleFactor;
            m_AppliedScale = scale;
            float left = safe.xMin / scale;
            float right = (Screen.width - safe.xMax) / scale;
            float bottom = safe.yMin / scale;

            for (int i = 0; i < m_LeftBase.Length; ++i)
                if (m_LeftControls[i] != null)
                    m_LeftControls[i].anchoredPosition = m_LeftBase[i] + new Vector2(left, bottom);

            for (int i = 0; i < m_RightBase.Length; ++i)
                if (m_RightControls[i] != null)
                    m_RightControls[i].anchoredPosition = m_RightBase[i] + new Vector2(-right, bottom);
        }
    }
}
