using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public enum POPUP_ANI
{
    OPEN_CLOSEUP,
    OPEN_FADEIN,

    CLOSE_GETAWAY,
    CLOSE_FADEOUT,
}

/// <summary> Parent of Every Popups. </summary>
public abstract class Popup : MonoBehaviour
{
    public Canvas canvas;
    public int key = 0;
    public List<PopupLinkInfo> linked_popups { get; set; }

    public RectTransform rect;
    public RawImage image;
    public POPUP_ANI open_ani_type;
    public POPUP_ANI close_ani_type;

    private Sequence seq;

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

    public void Open()
    {
        SetAni(open_ani_type);
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
        SetAni(close_ani_type);
    }

    public abstract void OnClosed();

    public void SetAni(POPUP_ANI _type)
    {
        seq = DOTween.Sequence();
        switch (_type)
        {
            case POPUP_ANI.OPEN_CLOSEUP:
            rect.localScale = new Vector2(0f, 0f);
            seq
            .Insert(0f, rect.DOScale(new Vector2(1f, 1f), 0.25f).SetEase(Ease.OutBack));
            break;

            case POPUP_ANI.OPEN_FADEIN:
            if (image == null)
            {
                image = this.GetComponent<RawImage>();
            }
            seq
            .Insert(0f, image.DOFade(.7f, 0.25f));
            break;

            case POPUP_ANI.CLOSE_GETAWAY:
            seq
            .Insert(0f, rect.DOScale(new Vector2(0f, 0f), 0.25f).SetEase(Ease.OutQuart))
            .InsertCallback(0.25f, () => { PopupMgr.RemovePopup(this.GetType()); } );
            break;

            case POPUP_ANI.CLOSE_FADEOUT:
            if (image == null)
            {
                image = GetComponent<RawImage>();
            }
            seq
            .Insert(0f, image.DOFade(0f, 0.25f))
            .InsertCallback(0.25f, () => { PopupMgr.RemovePopup(this.GetType()); } );
            break;
        }
    }
}