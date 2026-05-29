using UnityEngine;
using UnityEngine.EventSystems;

namespace PacknCraft.UI
{
    [ExecuteAlways, RequireComponent(typeof(RectTransform))]
    public class FitToParentScaler : UIBehaviour
    {
        // PRIVATE FIELDS
        private DrivenRectTransformTracker tracker;
        private bool isDirty;
        private RectTransform target;
        private RectTransform container;

        private Rect lastSafeArea = new(0, 0, 0, 0);
        private Vector2Int lastScreenSize = new(0, 0);
        private bool waitForLayout;

        // UNITY METHODS
        protected override void OnEnable()
        {
            base.OnEnable();

            target = transform as RectTransform;
            container = target.parent as RectTransform;

            SetDirty();
        }

        protected override void OnDisable()
        {
            tracker.Clear();
            base.OnDisable();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            SetDirty();
        }

        private void Update()
        {
            if (Screen.width != lastScreenSize.x ||
                Screen.height != lastScreenSize.y ||
                Screen.safeArea != lastSafeArea)
            {
                lastScreenSize = new Vector2Int(Screen.width, Screen.height);
                lastSafeArea = Screen.safeArea;

                waitForLayout = true;
            }
        }

        private void LateUpdate()
        {
            if (waitForLayout)
            {
                waitForLayout = false;
                SetDirty();
                return;
            }

            if (!isDirty) return;

            isDirty = false;
            ApplyLayout();
        }

        // PRIVATE METHODS
        private void SetDirty()
        {
            isDirty = true;
        }

        private void ApplyLayout()
        {
            if (container == null || target == null) return;

            tracker.Clear();

            tracker.Add(this, target,
                DrivenTransformProperties.Anchors |
                DrivenTransformProperties.Pivot |
                DrivenTransformProperties.AnchoredPosition |
                DrivenTransformProperties.Scale
            );

            // Enforce center anchor & pivot
            target.anchorMin = new Vector2(0.5f, 0.5f);
            target.anchorMax = new Vector2(0.5f, 0.5f);
            target.pivot = new Vector2(0.5f, 0.5f);

            Vector2 containerSize = container.rect.size;
            Vector2 targetSize = target.rect.size;

            if (targetSize.x <= 0 || targetSize.y <= 0) return;

            float scale = Mathf.Min(
                1f,
                Mathf.Min(
                    containerSize.x / targetSize.x,
                    containerSize.y / targetSize.y
                )
            );

            target.localScale = new Vector3(scale, scale, 1f);
            target.anchoredPosition = Vector2.zero;
        }
    }
}