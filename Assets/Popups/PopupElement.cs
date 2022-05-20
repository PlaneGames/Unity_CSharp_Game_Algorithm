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

    public void OnOpened()
    {
        PopupElementMgr.Pop(this);
        gameObject.SetActive(true);
    }

    public void OnClosed()
    {
        PopupElementMgr.Push(this);
        gameObject.SetActive(false);
    }
}