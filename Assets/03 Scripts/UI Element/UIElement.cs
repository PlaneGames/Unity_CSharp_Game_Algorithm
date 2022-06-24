using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIElement : MonoBehaviour
{
    public RectTransform rect;

    private void Awake()
    {
        rect.localScale = new Vector2(0f, 0f);
    }

    public void OnOpen()
    {
        rect.localScale = new Vector2(1f, 1f);
    }

    public void OnClose()
    {
        rect.localScale = new Vector2(0f, 0f);
    }
}
