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

    /// <summary> 이건.. 말도 안된는 거야! </summary>
    public const int _1234 = 1234;

    private List<PopupElement> linked_PEs { get; set; }

    private void Awake()
    {
        rect.localScale = new Vector2(0f, 0f);
    }

    public void PoolingPEs(Action Result)
    {
        SetElements(true, () => {
            Result();
        });
    }

    public void GetPEs(Action Result)
    {
        SetElements(false, () => {
            Result();
        });
    }

    private void SetElements(bool _is_pooling, Action Result)
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
                    if (_is_pooling)
                    {
                        PopupElementMgr.PoolingPE(_type, this, ( Result ) => 
                        {
                            _un_pushed_list.Remove(0);
                        });
                    }
                    else
                    {
                        PopupElementMgr.GetPE(_type, this, ( Result ) => 
                        {
                            _un_pushed_list.Remove(0);
                            linked_PEs.Add(Result.comp);
                            Result.comp.SetOrder(canvas.sortingOrder - 1);
                            Result.comp.OnOpened();
                        });
                    }
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
        StartCoroutine(_push_PEs());

    }

    public void SetOrderLayer(int _order)
    {
        canvas.sortingOrder = _order;
    }

    public void OnOpen()
    {
        Init();
        is_closing = false;
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
        PopupMgr.PopupOrderPop(this);
        is_closing = true;
        PopupAniCtr.SetAni(this, close_ani_type, () => 
        {
            PopupMgr.Push(this);
            OnClosed();
            gameObject.SetActive(false);
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