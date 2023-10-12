using UnityEngine;
using UnityEngine.UI;

namespace UFB.UI
{
    public class ScreenspaceMeshTriangle : Image
    {
        private RectTransform _anchor;
        private Transform _targetTransform;

        public void SetAnchor(RectTransform anchor)
        {
            _anchor = anchor;
            Debug.Log($"Set popupAnchor to {anchor.name}");
        }

        public void SetTarget(Transform t)
        {
            _targetTransform = t;
            Debug.Log($"Set targetTransform to {t.name}");
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                return;
            }
#endif

            if (_targetTransform == null || _anchor == null)
            {
                return;
            }

            vh.Clear();

            // Convert world position of targetObject to canvas position
            Vector2 screenPos = UnityEngine.Camera.main.WorldToScreenPoint(
                _targetTransform.position
            );
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                screenPos,
                null,
                out localPos
            );

            // Use localPos as one vertex and two corners of the popupAnchor as the other two vertices
            Vector3[] vertices = new Vector3[3];
            vertices[0] = localPos;
            // vertices[1] = min; // bottom-left corner of the popup
            // vertices[2] = max; // top-right corner of the popup
            vertices[1] = new Vector3(_anchor.rect.xMin, _anchor.rect.yMin, 0);
            vertices[2] = new Vector3(_anchor.rect.xMax, _anchor.rect.yMax * -1, 0);

            // Add vertices to VertexHelper
            for (int i = 0; i < 3; i++)
            {
                vh.AddVert(vertices[i], color, Vector2.zero); // color is the color of the Image component
            }

            // Add triangle
            vh.AddTriangle(0, 1, 2);
        }

        private void Update()
        {
            SetVerticesDirty(); // Call this to redraw the triangle if needed (like if the targetObject or popupAnchor moves)
        }
    }
}
