using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;

/*
Developer : Jae Young Kwon
Version : 22.05.02
*/

public struct PopupInfo
{
    public string address;
    public bool overlapping_able;
}

public struct PopupResult
{
    public GameObject obj;
    public IPopup comp;
}

/// <summary> Interface of Every Popups. </summary>
public interface IPopup 
{
    public void SetOrderLayer(int _order);

    public void PrintName();
}

/// <summary> The Popup Manager. </summary>
public class PopupMgr : MonoBehaviour
{

    public Dictionary<Type, PopupInfo> popup_pref_address_list;
    public Dictionary<Type, List<IPopup>> active_popup_list;

    public int last_popup_order;
    public int gap_between_order = 10;


    private void Start()
    {
        Init();
        PopupInit<PopupShop>();

        GetPopup<PopupShop>(( PopupResult Result ) => 
        { 
            Result.comp.PrintName();
        });
    }

    private void Init()
    {
        last_popup_order = 0;
        
        popup_pref_address_list = null;
        popup_pref_address_list = new Dictionary<Type, PopupInfo>();

        active_popup_list = null;
        active_popup_list = new Dictionary<Type, List<IPopup>>();
    }

    public void PopupInit<T>()
    {
        PopupInit<T>(false);
    }

    public void PopupInit<T>(bool _overlapping_able)
    {
        PopupInfo _info;
        _info.address = typeof(T).ToString();
        _info.overlapping_able = _overlapping_able;

        if (!popup_pref_address_list.ContainsKey(typeof(T)))
            popup_pref_address_list.Add(typeof(T), _info);
        else
            Debug.LogError(_info.address + " 팝업 Init가 중복 실행되었습니다.");
    }

    public void GetPopup<T>(Action<PopupResult> Result) where T : IPopup
    {
        PopupResult _info;
        string _pref = popup_pref_address_list[typeof(T)].address;

        Addressables.InstantiateAsync(_pref).Completed += (handle) =>
        {
            GameObject _obj = handle.Result;
            _info.obj = _obj;
            _info.comp = _obj.GetComponent<T>();
            AddPopupToList<T>(_info.comp);

            _obj = null;
            Result(_info);
        };
    }

    private void AddPopupToList<T>(IPopup _comp)
    {
        Type _type = typeof(T);
        if (!active_popup_list.ContainsKey(_type))
        {
            var _popup_list = new List<IPopup>();
            _popup_list.Add(_comp);
            active_popup_list.Add(_type, _popup_list);
        }
        else
        {
            active_popup_list[_type].Add(_comp);
        }

        last_popup_order += gap_between_order;
        _comp.SetOrderLayer(last_popup_order);
    }

}