using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/*
Developer : Jae Young Kwon
Version : 22.05.20
*/

/// <summary> Popup Animation Controller. </summary>
public class PopupAniCtr : MonoBehaviour
{

    public static void SetAni(Popup _popup, POPUP_ANI _type, Action _callback)
    {
        if (_popup.image == null)
        {
            _popup.image = _popup.GetComponent<Image>();
        }
        SetAni(_popup.GetType(), _popup.seq, _popup.rect, _popup.image, _type, _callback);
    }

    public static void SetAni(PopupElement _popup_element, POPUP_ANI _type, Action _callback)
    {
        if (_popup_element.image == null)
        {
            _popup_element.image = _popup_element.GetComponent<Image>();
        }
        SetAni(_popup_element.GetType(), _popup_element.seq, _popup_element.rect, _popup_element.image, _type, _callback);
    }

    public static void SetAni(Type _popup_type, Sequence _seq, RectTransform _rect, Image _image, POPUP_ANI _type, Action _callback)
    {
        if (_seq != null)
        {
            _seq.Rewind(false);
            _seq.Kill(true);
        }
        else
        {
            _seq = DOTween.Sequence();
        }
        switch (_type)
        {
            case POPUP_ANI.OPEN_CLOSEUP:
            _rect.localScale = new Vector2(0.3f, 0.3f);
            _seq
            .Insert(0f, _rect.DOScale(new Vector2(1f, 1f), 0.25f).SetEase(Ease.OutBack));
            break;

            case POPUP_ANI.OPEN_FADEIN:
            _seq
            .Insert(0f, _image.DOFade(.7f, 0.25f));
            break;

            case POPUP_ANI.CLOSE_GETAWAY:
            _seq
            .Insert(0f, _rect.DOScale(new Vector2(0.1f, 0.1f), 0.25f).SetEase(Ease.InBack))
            .InsertCallback(0.25f, () => { _callback(); } );
            break;

            case POPUP_ANI.CLOSE_FADEOUT:
            _seq
            .Insert(0f, _image.DOFade(0f, 0.25f))
            .InsertCallback(0.25f, () => { _callback(); } );
            break;
        }
    }

}