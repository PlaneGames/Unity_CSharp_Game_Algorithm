using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/*
Developer : Jae Young Kwon
Version : 22.05.19
*/

public enum POPUP_ANI
{
    OPEN_CLOSEUP,
    OPEN_FADEIN,

    CLOSE_GETAWAY,
    CLOSE_FADEOUT,
}

public struct LinkToPEInfo
{
    public PopupElement PE;
    public bool remove_together;
}

/// <summary> Parent of Every Popups. </summary>
public abstract class Popup : MonoBehaviour
{
    public Canvas canvas;
    public int key = 0;
    public List<PopupElement> linked_PEs { get; set; }

    public RectTransform rect;
    [HideInInspector] public Image image;
    public POPUP_ANI open_ani_type;
    public POPUP_ANI close_ani_type;

    [HideInInspector] public Sequence seq;
    private bool is_closing;

    public void SetElements(Action Result)
    {
        IEnumerator _push_PEs()
        {
            List<int> _un_pushed_list = new List<int>();
            _un_pushed_list.Add(0);

            PopupElementMgr.GetPE<PopupElementCover>(this, ( Result ) => {
                linked_PEs.Add(Result.comp);
                _un_pushed_list.Remove(0);
            });

            while (true)
            {
                if (_un_pushed_list.Count == 0)
                {
                    _un_pushed_list.Clear();
                    _un_pushed_list = null;
                    break;
                }
                yield return null;
            }

            Result();
        }

        if (linked_PEs == null)
        {
            linked_PEs = new List<PopupElement>();
        }
        StartCoroutine(_push_PEs());
    }

    public void SetKey(int _key)
    {
        key = _key;
    }

    public void SetOrderLayer(int _order)
    {
        canvas.sortingOrder = _order;
    }

    public void Open()
    {
        is_closing = false;
        PopupAniCtr.SetAni(this, open_ani_type, () => {});
    }

    public void Close()
    {
        if (is_closing)
            return;

        CloseElements();
        is_closing = true;
        PopupAniCtr.SetAni(this, close_ani_type, () => {
            PopupMgr.RemovePopup(GetType());
        });
    }

    private void CloseElements()
    {
        if (linked_PEs != null)
        {
            int i;
            for (i = 0; i < linked_PEs.Count; i ++)
            {
                linked_PEs[i].OnClosed();
            }
            linked_PEs.Clear();
        }
    }

    public abstract void OnOpened();

    public abstract void OnClosed();
}