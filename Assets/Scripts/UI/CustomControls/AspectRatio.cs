using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class AspectRatio : VisualElement
{
    private float _widthRatio = 16;
    private float _heightRatio = 9;

    public bool modifyParentStyle = true;

    public float WidthRatio
    {
        get => _widthRatio;
        set
        {
            _widthRatio = value;
            OnGeometryChanged(new GeometryChangedEvent());
        }
    }
    public float HeightRatio
    {
        get => _heightRatio;
        set
        {
            _heightRatio = value;
            OnGeometryChanged(new GeometryChangedEvent());
        }
    }

    public AspectRatio()
    {
        style.flexGrow = 1;
        style.backgroundColor = Color.black;

        RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
        if (parent is null)
            return;

        // Get the current size of the container
        float containerWidth =
            parent.resolvedStyle.width
            - parent.resolvedStyle.paddingLeft
            - parent.resolvedStyle.paddingRight;
        float containerHeight =
            parent.resolvedStyle.height
            - parent.resolvedStyle.paddingTop
            - parent.resolvedStyle.paddingBottom;

        if (containerWidth > 0 && containerHeight > 0)
        {
            float targetWidth = containerWidth;
            float targetHeight = targetWidth / _widthRatio * _heightRatio;

            if (targetHeight > containerHeight)
            {
                // Height is the limiting factor
                targetHeight = containerHeight;
                targetWidth = targetHeight / _heightRatio * _widthRatio;
            }

            style.width = targetWidth;
            style.maxWidth = targetWidth;
            style.height = targetHeight;
            style.maxHeight = targetHeight;
        }

        if (modifyParentStyle)
        {
            parent.style.display = DisplayStyle.Flex;
            parent.style.flexDirection = FlexDirection.Row;
            parent.style.backgroundColor = Color.black;
            parent.style.justifyContent = Justify.Center;
            parent.style.alignItems = Align.Center;
        }
    }
}
