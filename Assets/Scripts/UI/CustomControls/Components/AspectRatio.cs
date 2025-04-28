using System;
using NUnit.Compatibility;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class AspectRatio : VisualElement
{
    private readonly float _targetAspectRatio = 16f / 9f;  // You can change this to any ratio you want

    public AspectRatio()
    {
        // Initialize the aspect ratio and child container
        style.flexGrow = 1;
        style.backgroundColor = Color.black;
        RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
        // Get the current size of the container
        float containerWidth = resolvedStyle.width;
        float containerHeight = resolvedStyle.height;

        if (containerWidth > 0 && containerHeight > 0)
        {
            // Calculate the aspect ratio of the container
            float containerAspectRatio = containerWidth / containerHeight;

            // Check if the container needs to adjust to maintain the target aspect ratio
            if (containerAspectRatio > _targetAspectRatio)
            {
                // Too wide, adjust the height (vertical black bars)
                float newHeight = containerWidth / _targetAspectRatio;
                float padding = (containerHeight - newHeight) / 2;

                style.paddingTop = 0;
                style.paddingBottom = 0;
                style.paddingLeft = Math.Abs(padding);
                style.paddingRight = Math.Abs(padding);
            }
            else
            {
                // Too tall, adjust the width (horizontal black bars)
                float newWidth = containerHeight * _targetAspectRatio;
                float padding = (containerWidth - newWidth) / 2;

                // Apply padding (black bars on the left and right)
                style.paddingTop = Math.Abs(padding);
                style.paddingBottom = Math.Abs(padding);
                style.paddingLeft = 0;
                style.paddingRight = 0;
            }
        }
    }
}
