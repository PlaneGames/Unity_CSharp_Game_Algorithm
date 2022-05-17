using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


/// <summary> Parent of Every Popups. </summary>
public abstract class Popup : MonoBehaviour
{
    public Canvas canvas;
    public int key = 0;
    public List<PopupLinkInfo> linked_popups { get; set; }


    public void SetKey(int _key)
    {
        key = _key;
    }

    public void SetOrderLayer(int _order)
    {
        canvas.sortingOrder = _order;
    } 

    public void SetToLink(Popup _popup, bool _remove_together)
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

    }

    protected void CloseComplete()
    {
        PopupMgr.RemovePopup(this.GetType());
    }

    public abstract void OnClosed();
}