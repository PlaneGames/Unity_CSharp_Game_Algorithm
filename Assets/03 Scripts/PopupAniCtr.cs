using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/*
Developer : Jae Young Kwon
Version : 22.06.02
*/

public enum POPUP_ANI
{
    OPEN_CLOSEUP,
    OPEN_FADEIN,
    OPEN_TOP_SLIDING,

    CLOSE_GETAWAY,
    CLOSE_FADEOUT,
    CLOSE_TOP_SLIDING
}

public enum UI_ANI
{
    FADEIN,
    FADEOUT,
}

/// <summary> Popup Animation Controller. </summary>
public class PopupAniCtr : MonoBehaviour
{

    public static void SetAni(UI_Popup _popup, POPUP_ANI _type, Action _callback)
    {
        if (_popup.image == null)
        {
            _popup.image = _popup.GetComponent<Image>();
        }
        SetAni(_popup.seq, _popup.rect, _popup.image, _type, _callback);
    }

    public static void SetAni(UI_Element _popup_element, POPUP_ANI _type, Action _callback)
    {
        if (_popup_element.image == null)
        {
            _popup_element.image = _popup_element.GetComponent<Image>();
        }
        SetAni(_popup_element.seq, _popup_element.rect, _popup_element.image, _type, _callback);
    }

    public static void SetAni(Sequence _seq, RectTransform _rect, Image _image, POPUP_ANI _type, Action _callback)
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
            .Insert(0f, _rect.DOScale(new Vector2(1f, 1f), 0.25f).SetEase(Ease.OutBack))
            .InsertCallback(0.25f, () => { _callback(); } );
            break;

            case POPUP_ANI.OPEN_FADEIN:
            _seq
            .Insert(0f, _image.DOFade(.4f, 0.25f))
            .InsertCallback(0.25f, () => { _callback(); } );
            break;

            case POPUP_ANI.OPEN_TOP_SLIDING:
            _rect.localScale = new Vector2(1f, 1f);
            _rect.anchoredPosition = new Vector2(0f, 140f);
            _seq
            .Insert(0f, _rect.DOAnchorPosY(0f, 0.5f).SetEase(Ease.OutBack))
            .InsertCallback(0.5f, () => { _callback(); } );
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

            case POPUP_ANI.CLOSE_TOP_SLIDING:
            _rect.anchoredPosition = new Vector2(0f, 0f);
            _seq
            .Insert(0f, _rect.DOAnchorPosY(140f, 0.25f).SetEase(Ease.InQuad))
            .InsertCallback(0.25f, () => { _callback(); } );
            break;
        }
    }

    public static void SetAni(Sequence _seq, Text _text, UI_ANI _type, float _start_delay, float _duration, Action _callback)
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
            case UI_ANI.FADEIN:
            _text.color -= Color.black;
            _seq
            .Insert(_start_delay, _text.DOFade(1f, _duration))
            .InsertCallback(_start_delay + _duration, () => { _callback(); } );
            break;
        }
    }

}