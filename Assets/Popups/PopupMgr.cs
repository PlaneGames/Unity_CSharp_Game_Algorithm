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
Version : 22.05.13
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
    public GameObject gameObject { get ; } 

    public void SetKey(int _key);

    public void SetOrderLayer(int _order);

    public void PrintName();
}

/// <summary> The Popup Manager. </summary>
public class PopupMgr : MonoBehaviour
{

    public static Transform canvas_trans;

    public static Dictionary<Type, PopupInfo> popup_pref_address_list;
    public static Dictionary<Type, IPopup> active_popup_list;

    public static int last_popup_order;
    public static int gap_between_order = 10;
    public static int last_key;

    private void Start()
    {
        Init();
        PopupInit<PopupShop>();

        GetPopup<PopupShop>(( PopupResult Result ) => 
        { 
            Result.comp.PrintName();
        });
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            GetPopup<PopupShop>(( PopupResult Result ) => 
            { 
                Result.comp.PrintName();
            });
        }   
    }

    private void Init()
    {
        last_popup_order = 0;
        last_key = 0;
        
        popup_pref_address_list = null;
        popup_pref_address_list = new Dictionary<Type, PopupInfo>();

        active_popup_list = null;
        active_popup_list = new Dictionary<Type, IPopup>();

        canvas_trans = GameObject.Find("Canvas").transform;
    }

    public static void PopupInit<T>()
    {
        PopupInit<T>(false);
    }
 
    public static void PopupInit<T>(bool _overlapping_able)
    {
        PopupInfo _info;
        _info.address = typeof(T).ToString();
        _info.overlapping_able = _overlapping_able;

        if (!popup_pref_address_list.ContainsKey(typeof(T)))
            popup_pref_address_list.Add(typeof(T), _info);
        else
            Debug.LogError(_info.address + " 팝업 Init가 중복 실행되었습니다.");
    }

    public static void GetPopup<T>(Action<PopupResult> Result) where T : IPopup
    {
        PopupResult _info;
        var _type = typeof(T);
        string _pref = popup_pref_address_list[_type].address;

        if (!active_popup_list.ContainsKey(_type))
        {
            Addressables.InstantiateAsync(_pref, canvas_trans).Completed += (handle) =>
            {
                GameObject _obj = handle.Result;
                _info.obj = _obj;
                _info.comp = _obj.GetComponent<T>();

                active_popup_list.Add(_type, _info.comp);

                _info.comp.SetKey(last_key);
                last_key ++;
                last_popup_order += gap_between_order;
                _info.comp.SetOrderLayer(last_popup_order);

                _obj = null;
                Result(_info);
            };
        }
        else
        {
            var _popup = active_popup_list[_type];
            _popup.gameObject.SetActive(true);
            _popup.SetKey(last_key);
            last_key ++;
            last_popup_order += gap_between_order;
            _popup.SetOrderLayer(last_popup_order);
        }
    }

    /*
    private static void AddPopupToList<T>(int _key, IPopup _comp)
    {
        var _type = typeof(T);
        if (!active_popup_list.ContainsKey(_type))
        {
            var _info = new Dictionary<int, IPopup>();
            _info.Add(_key, _comp);
            active_popup_list.Add(_type, _info);
        }
        else
        {
            active_popup_list[_type][_key].gameObject.SetActive(true);
        }
        _comp.SetKey(last_key);
        last_key ++;
        last_popup_order += gap_between_order;
        _comp.SetOrderLayer(last_popup_order);
    }
    */

    public static void RemovePopup<T>(int _key)
    {
        var _type = typeof(T);
        active_popup_list[_type].gameObject.SetActive(false);
        //active_popup_list.Remove(_type);
        last_key --;
        last_popup_order -= gap_between_order;
    }   

}