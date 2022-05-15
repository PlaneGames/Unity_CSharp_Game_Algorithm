using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PopupCover : MonoBehaviour, IPopup
{
    public Canvas canvas;
    public string name = "PopupCover!";
    public int key;
    public List<PopupLinkInfo> linked_popups { get; set; }

    public void SetKey(int _key)
    {
        key = _key;
    }

    public void SetOrderLayer(int _order)
    {
        canvas.sortingOrder = _order;
    }

    public void SetToLink(IPopup _popup, bool _remove_together)
    {
        if (linked_popups == null)
        {
            linked_popups = new List<PopupLinkInfo>();
        }
        PopupLinkInfo _info;
        _info.popup = _popup;
        _info.remove_together = _remove_together;
        linked_popups.Add(_info);
    }

    public void PrintName()
    {
        Debug.Log(name);
    }

    public void Close()
    {
        PopupMgr.RemovePopup<PopupCover>(key);
    }
}