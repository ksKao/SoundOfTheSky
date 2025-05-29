using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class AspectRatio : VisualElement
{
    public AspectRatio()
    {
        // Initialize the aspect ratio and child container
        style.flexGrow = 1;
        style.backgroundColor = Color.black;

        RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
        if (parent is null)
            return;

        // Get the current size of the container
        float containerWidth = parent.resolvedStyle.width;
        float containerHeight = parent.resolvedStyle.height;

        if (containerWidth > 0 && containerHeight > 0)
        {
            float targetWidth = containerWidth;
            float targetHeight = targetWidth / 16f * 9f;

            if (targetHeight > containerHeight)
            {
                // Height is the limiting factor
                targetHeight = containerHeight;
                targetWidth = targetHeight / 9f * 16f;
            }

            style.width = targetWidth;
            style.maxWidth = targetWidth;
            style.maxHeight = targetHeight;
            style.height = targetHeight;
        }

        parent.style.display = DisplayStyle.Flex;
        parent.style.flexDirection = FlexDirection.Row;
        parent.style.backgroundColor = Color.black;
        parent.style.justifyContent = Justify.Center;
        parent.style.alignItems = Align.Center;
    }
}
