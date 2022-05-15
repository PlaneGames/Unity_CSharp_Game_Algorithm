using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PopupShop : MonoBehaviour, IPopup
{
    public Canvas canvas;
    public string name = "PopupShop!";
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
        if (linked_popups != null)
        {
            int i;
            for (i = 0; i < linked_popups.Count; i ++)
            {
                if (linked_popups[i].remove_together)
                {
                    linked_popups[i].popup.Close();
                }
            }
            linked_popups.Clear();
            linked_popups = null;
        }

        PopupMgr.RemovePopup<PopupShop>(key);
    }
}