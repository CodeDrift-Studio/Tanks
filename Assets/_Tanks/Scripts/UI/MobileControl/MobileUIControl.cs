using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;

namespace Tanks.Complete
{
    // Handle on screen UI Control used for mobile platform. By default, will auto-disable itself when not on mobile
    // Default execution is low to ensure it disable itself before any other script so other scripts can use its enabled
    // status as a test to know if on a mobile platform or not.
    [DefaultExecutionOrder(-90)]
    public class MobileUIControl : MonoBehaviour
    {
        public static MobileUIControl Instance { get; private set; }
        
        [Tooltip("If true (the default) the GameObject on which this is will get disabled when not on a mobile platform")]
        public bool AutoDisableOnNonMobilePlatform = true;

        [Tooltip("If true (the default), the controls start hidden and are only shown by Show() when a match begins, so they can never flash over the menu")]
        public bool HideUntilMatchStarts = true;

        public InputDevice Device => m_Control.control.device;
        
        private OnScreenControl m_Control;
        private CanvasGroup m_Group;            // Used to hide the controls during pause WITHOUT deactivating them (keeps the virtual device alive)

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            Instance = this;

            m_Group = GetComponent<CanvasGroup>();
            if (m_Group == null)
                m_Group = gameObject.AddComponent<CanvasGroup>();

            m_Control = GetComponentInChildren<OnScreenControl>();
            if (AutoDisableOnNonMobilePlatform && !Application.isMobilePlatform)
            {
                gameObject.SetActive(false);
            }
            else if (HideUntilMatchStarts)
            {
                // Hidden before the first frame; GameUIHandler.StartGame() calls Show() when the match begins.
                gameObject.SetActive(false);
            }
        }

        public void Show()
        {
            // On non mobile platform with the auto disable, we cannot show it
            if (AutoDisableOnNonMobilePlatform && !Application.isMobilePlatform)
            {
                return;
            }
            
            ApplyPauseVisibility(false);
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (AutoDisableOnNonMobilePlatform && !Application.isMobilePlatform)
            {
                return;
            }
            
            // A control that is deactivated while still pressed never receives its pointer-up, which would leave the
            // last joystick/button value stuck on the virtual device. Release them explicitly before hiding.
            ReleaseAll();

            gameObject.SetActive(false);
        }

        // Hide (true) or restore (false) the gameplay controls while the pause menu is open. Only this canvas is affected,
        // the on-screen pause button lives in the menu canvas and stays usable. The GameObject stays active on purpose:
        // deactivating it would remove the virtual gamepad the tanks are paired with. Safe to call repeatedly.
        public void SetPausedHidden(bool hidden)
        {
            // Always drop any held input at the pause/resume boundary, in both directions.
            ReleaseAll();
            ApplyPauseVisibility(hidden);
        }

        private void ApplyPauseVisibility(bool hidden)
        {
            if (m_Group == null)
                return;

            m_Group.alpha = hidden ? 0f : 1f;
            m_Group.interactable = !hidden;
            m_Group.blocksRaycasts = !hidden;
        }

        // Force every on-screen control (joystick, fire, rotation buttons) back to its released state.
        // Used on hide, pause and when the app loses focus so no input can stay stuck.
        public void ReleaseAll()
        {
            if (!gameObject.activeInHierarchy)
                return;

            var release = new PointerEventData(EventSystem.current);
            foreach (var handler in GetComponentsInChildren<IPointerUpHandler>(false))
                handler.OnPointerUp(release);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
                ReleaseAll();
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused)
                ReleaseAll();
        }
    }
}