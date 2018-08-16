﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unity sprite utility class for helping utilizing sprites. 
/// </summary>
public static class SpriteUtility {

    /// <summary>
    /// Resizes the given sprite renderer's local scale to the given dimensions. 
    /// </summary>
    public static void ResizeSpriteRendererToDimensions(Vector2 dimensions, SpriteRenderer s)
    {
        Vector3 spriteBounds = s.bounds.size;
        if (spriteBounds != Vector3.zero)
        {
            s.transform.localScale = new Vector3(s.transform.localScale.x * dimensions.x / spriteBounds.x,
                                                 s.transform.localScale.y * dimensions.y / spriteBounds.y, 1);
        }
    }

    /// <summary>
    /// Resizes the given sprite renderer's local scale to the given rect transform's size delta dimensions. 
    /// </summary>
    public static void ResizeSpriteRendererToRect(RectTransform rect, SpriteRenderer s)
    {
        Vector2 dimensions = rect.sizeDelta;
        ResizeSpriteRendererToDimensions(dimensions, s);
    }

    /// <summary>
    /// Resizes the given sprite renderer's local scale to the given gameobject's rect transform's size delta dimensions. 
    /// </summary>
    public static void ResizeSpriteRendererToGObj(GameObject g, SpriteRenderer s)
    {
        RectTransform rect = g.GetComponent<RectTransform>();
        ResizeSpriteRendererToRect(rect, s);
    }
}
