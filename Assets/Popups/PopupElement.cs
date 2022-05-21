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

/// <summary> Parent of Every Popup Elements. </summary>
public abstract class PopupElement : MonoBehaviour
{
    public Canvas canvas;
    public RectTransform rect;
    [HideInInspector] public Sequence seq;
    [HideInInspector] public Image image;
    public POPUP_ANI open_ani_type;
    public POPUP_ANI close_ani_type;

    public void SetOrder(int _order)
    {
        canvas.sortingOrder = _order;
    }

    public void OnOpened()
    {
        PopupElementMgr.Pop(this);
        gameObject.SetActive(true);
        PopupAniCtr.SetAni(this, open_ani_type, () => {});
    }

    public void OnClosed()
    {
        PopupAniCtr.SetAni(this, close_ani_type, () => {
            PopupElementMgr.Push(this);
            gameObject.SetActive(false);
        });
    }
}