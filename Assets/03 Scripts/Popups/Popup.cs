using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/*
Developer : Jae Young Kwon
Version : 22.05.23
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
    [Header ("< Popup Basic Infos >")]
    public Canvas canvas;
    public RectTransform rect;
    public POPUP_ANI open_ani_type;
    public POPUP_ANI close_ani_type;
    public PE_NAME[] popup_elements_to_link;

    [HideInInspector] public Image image;
    [HideInInspector] public Sequence seq;
    private bool is_closing;
    private bool is_loaded;

    private List<PopupElement> linked_PEs { get; set; }

    public void PoolingPEs(Action Result)
    {
        SetElements(() => {
            PushPEs();
            Debug.Log(linked_PEs.Count);
            is_loaded = true;
            Result();
        });
    }

    public void GetPEs(Action Result)
    {
        SetElements(() => {
            OpenPEs();
            Debug.Log(linked_PEs.Count);
            is_loaded = true;
            Result();
        });
    }

    private void SetElements(Action Result)
    {
        int i;

        IEnumerator _push_PEs()
        {
            List<int> _un_pushed_list = new List<int>();

            for (i = 0; i < popup_elements_to_link.Length; i ++)
            {
                _un_pushed_list.Add(0);
                Type _type = Type.GetType(popup_elements_to_link[i].ToString());
                if (_type != null)
                {
                    PopupElementMgr.GetPE(_type, this, ( Result ) => {
                        linked_PEs.Add(Result.comp);
                        Result.comp.SetOrder(canvas.sortingOrder - 1);
                        _un_pushed_list.Remove(0);
                    });
                }
            }
            
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
        if (!is_loaded)
            StartCoroutine(_push_PEs());
        else
            Result();

    }

    private void PushPEs()
    {
        if (linked_PEs != null && linked_PEs.Count > 0)
        {
            int i;
            for (i = 0; i < linked_PEs.Count; i ++)
            {
                PopupElementMgr.Push(linked_PEs[i]);
                linked_PEs[i].gameObject.SetActive(false);
            }
        }
    }

    private void OpenPEs()
    {
        if (linked_PEs != null && linked_PEs.Count > 0)
        {
            int i;
            for (i = 0; i < linked_PEs.Count; i ++)
            {
                linked_PEs[i].OnOpened();
            }
        }
    }

    public void SetOrderLayer(int _order)
    {
        canvas.sortingOrder = _order;
    }

    public void OnOpen()
    {
        Init();
        is_closing = false;
        PopupMgr.Pop(this);
        PopupAniCtr.SetAni(this, open_ani_type, () => 
        {
            OnOpened(); 
        });
    }

    public void OnClose()
    {
        if (is_closing)
            return;

        CloseElements();
        is_closing = true;
        PopupAniCtr.SetAni(this, close_ani_type, () => 
        {
            PopupMgr.Push(this);
            OnClosed();
            gameObject.SetActive(false);
            is_loaded = false;
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

    protected virtual void Init()
    {

    }

    protected abstract void OnOpened();

    protected abstract void OnClosed();

}