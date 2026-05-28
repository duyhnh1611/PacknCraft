using UnityEngine;

namespace PacknCraft.Utils
{
    [ExecuteAlways]
    public class SafeArea : MonoBehaviour
    {
        // PRIVATE FIELDS
        private RectTransform rectTransform;
        private Rect lastSafeArea = new(0, 0, 0, 0);
        private Vector2Int lastScreenSize = new(0, 0);

        // UNITY EVENTS
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            ApplySafeArea();
        }

        private void Update()
        {
            if (Screen.width != lastScreenSize.x ||
                Screen.height != lastScreenSize.y ||
                Screen.safeArea != lastSafeArea)
            {
                ApplySafeArea();
            }
        }

        // PRIVATE METHODS
        private void ApplySafeArea()
        {
            Rect safeArea = Screen.safeArea;

            lastSafeArea = safeArea;
            lastScreenSize = new(Screen.width, Screen.height);

            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }
    }
}