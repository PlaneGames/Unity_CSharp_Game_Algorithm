using System;
using System.Reflection;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using DG.Tweening;

/*
Developer : Jae Young Kwon
Version : 22.07.01
*/

public class UI_Element : MonoBehaviour
{
    public Canvas canvas;
    public RectTransform rect;
    [HideInInspector] public Sequence seq;
    [HideInInspector] public Image image;
    public POPUP_ANI open_ani_type;
    public POPUP_ANI close_ani_type;


    private void Awake()
    {
        //rect.localScale = new Vector2(0f, 0f);
    }

    public void SetOrder(int _order)
    {
        canvas.sortingOrder = _order;
    }

    public void OnOpened()
    {
        PopupAniCtr.SetAni(this, open_ani_type, () => {});
    }

    public void OnClosed()
    {
        PopupAniCtr.SetAni(this, close_ani_type, () => {
            UI_ElementMgr.Push(this);
            gameObject.SetActive(false);
        });
    }
}