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
Version : 22.05.19
*/

public struct PopupInfo
{ 
    public string address;  
    public bool overlapping_able;
} 

public struct PopupResult
{ 
    public GameObject obj;
    public Popup comp;
}

public struct PopupLinkInfo
{
    public Popup popup;
    public bool remove_together;
}


/// <summary> The Popup Manager. </summary>
public class PopupMgr : MonoBehaviour
{

    public static Dictionary<Type, PopupInfo> popup_pref_address_list;

    public static Dictionary<Type, Popup> pool_popup_list;
    public static Dictionary<Type, Popup> active_popup_list;

    public static Dictionary<int, Popup> active_popup_order_list;

    public static int last_popup_order;
    public static int gap_between_order = 10;
    public static int last_key;
    
    public static bool popup_is_opening;


    private void Start()
    {
        Init();

        PopupInit<PopupCover>(true);
        PopupInit<PopupShop>();
        PopupInit<PopupException>();   
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            GetPopup<PopupShop>();
        }
        if (Input.GetKey(KeyCode.A))
        {
            GetPopup<PopupException>();
        }
    }
    
    private void Init()
    {
        last_popup_order = 0;
        last_key = 0;

        popup_pref_address_list = null;
        popup_pref_address_list = new Dictionary<Type, PopupInfo>();

        pool_popup_list = null;
        pool_popup_list = new Dictionary<Type, Popup>();

        active_popup_list = null;
        active_popup_list = new Dictionary<Type, Popup>();
    }

    /// <summary> Initialize The Popup. <br/>Create The GameObject In The Current Scene. </summary>
    public static void PopupInit<T>()
    {
        PopupInit<T>(false);
    }

    /// <summary> Initialize The Popup. <br/>Create The GameObject In The Current Scene. </summary>
    public static void PopupInit<T>(bool _overlapping_able)
    {
        PopupInfo _info;
        _info.address = typeof(T).ToString();
        _info.overlapping_able = _overlapping_able;

        if (!popup_pref_address_list.ContainsKey(typeof(T)))
            popup_pref_address_list.Add(typeof(T), _info);
        else
            Debug.LogError(_info.address + " 팝업이 중복으로 초기화되었습니다.");
    }

    public static void GetPopup<T>() where T : Popup
    {
        GetPopup<T>( ( PopupResult Result ) => {} );
    }

    public static void GetPopup<T>(Action<PopupResult> Result) where T : Popup
    {
        var _type = typeof(T);
        var _over_able = popup_pref_address_list[_type].overlapping_able;

        if (!_over_able)
        {
            if (popup_is_opening)
                return;
    
            popup_is_opening = true;
        }

        PopupResult _info;
        
        string _pref = popup_pref_address_list[_type].address;

        if (pool_popup_list.ContainsKey(_type))
        {
            if (!active_popup_list.ContainsKey(_type))
            {
                var _popup = pool_popup_list[_type];
                _info.obj = _popup.gameObject;
                _info.comp = _popup;
                _popup.CheckLinking(() => 
                {
                    PopupSetActive(_type, _info, false);
                    popup_is_opening = false;
                    Result(_info);
                });
            }
            else
            {
                if (!_over_able)
                    popup_is_opening = false;
                else
                {
                    var _popup = pool_popup_list[_type];
                    _info.obj = _popup.gameObject;
                    _info.comp = _popup;
                    _popup.CheckLinking(() => 
                    {
                        PopupSetActive(_type, _info, true);
                        popup_is_opening = false;
                        Result(_info);
                    });
                }
            }
        }
        else
        {
            Addressables.InstantiateAsync(_pref, SceneMgr.active_canvas_list[CANVAS_TYPE.EXPAND].trans).Completed += (handle) =>
            {
                GameObject _obj = handle.Result;
                _info.obj = _obj;
                _info.comp = _obj.GetComponent<T>();
                pool_popup_list.Add(_type, _info.comp);
                _info.obj.SetActive(false);
                _info.comp.CheckLinking(() => 
                {
                    PopupSetActive(_type, _info, false);
                    popup_is_opening = false;
                    Result(_info);
                });
            };
        }
    }

    private static void PopupSetActive(Type _type, PopupResult _info, bool _over_able)
    {
        if (!_over_able)
            active_popup_list.Add(_type, _info.comp);
        active_popup_list[_type].gameObject.SetActive(true);
        PopupOrderInit(active_popup_list[_type]);
        active_popup_list[_type].Open();
    }

    private static void PopupOrderInit(Popup _comp)
    {
        last_key ++;
        last_popup_order = active_popup_list.Count * gap_between_order;
        _comp.SetKey(last_key);
        _comp.SetOrderLayer(last_popup_order);
    }

    public static void RemovePopup(Type _type)
    {
        active_popup_list[_type].OnClosed();
        active_popup_list[_type].gameObject.SetActive(false);
        active_popup_list.Remove(_type);
        last_key --;
        last_popup_order -= gap_between_order;
    }

}