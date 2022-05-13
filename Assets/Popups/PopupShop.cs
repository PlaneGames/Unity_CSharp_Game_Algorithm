using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PopupShop : MonoBehaviour, IPopup
{
    public Canvas canvas;
    public string name = "PopupShop!";
    public int key;

    public void SetKey(int _key)
    {
        key = _key;
    }

    public void SetOrderLayer(int _order)
    {
        canvas.sortingOrder = _order;
    } 

    public void PrintName()
    {
        Debug.Log(name);
    }

    public void Close()
    {
        PopupMgr.RemovePopup<PopupShop>(key);
    }
}