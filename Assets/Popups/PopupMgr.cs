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
Version : 22.05.17
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
    public static Dictionary<Type, Popup> active_popup_list;

    public static int last_popup_order;
    public static int gap_between_order = 10;
    public static int last_key;

    public static bool popup_is_opening;


    private void Start()
    {
        Init();

        PopupInit<PopupShop>();
        PopupInit<PopupCover>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            GetPopup<PopupCover>( ( PopupResult cover ) => 
            {
                GetPopup<PopupShop>( ( PopupResult Result ) => 
                {
                    Result.comp.SetToLink(cover.comp, true);
                });
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
        active_popup_list = new Dictionary<Type, Popup>();
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

    public static void GetPopup<T>() where T : Popup
    {
        GetPopup<T>( ( PopupResult Result ) => {} );
    }

    public static void GetPopup<T>(bool _active_able) where T : Popup
    {
        GetPopup<T>( ( PopupResult Result ) => { Result.obj.SetActive(_active_able); } );
    }

    public static void GetPopup<T>(Action<PopupResult> Result) where T : Popup
    {
        if (!popup_is_opening)
        {
            popup_is_opening = true;
            PopupResult _info;
            var _type = typeof(T);
            string _pref = popup_pref_address_list[_type].address;

            if (!active_popup_list.ContainsKey(_type))
            {
                Addressables.InstantiateAsync(_pref, SceneMgr.active_canvas_list[CANVAS_TYPE.EXPAND].trans).Completed += (handle) =>
                {
                    GameObject _obj = handle.Result;
                    _info.obj = _obj;
                    _info.comp = _obj.GetComponent<T>();
                    active_popup_list.Add(_type, _info.comp);
                    PopupOrderInit(_info.comp);
                    _info.comp.Open();
                    _obj = null;
                    popup_is_opening = false;
                    Result(_info);
                };
            }
            else
            {
                var _popup = active_popup_list[_type];
                _popup.gameObject.SetActive(true);
                PopupOrderInit(_popup);
                _info.obj = _popup.gameObject;
                _info.comp = _popup;
                _popup.Open();
                popup_is_opening = false;
                Result(_info);
            }
        }
    }

    private static void PopupOrderInit(Popup _comp)
    {
        last_key ++;
        last_popup_order += gap_between_order;
        _comp.SetKey(last_key);
        _comp.SetOrderLayer(last_popup_order);
    }

    public static void RemovePopup(Type _type)
    {
        active_popup_list[_type].OnClosed();
        active_popup_list[_type].gameObject.SetActive(false);
        last_key --;
        last_popup_order -= gap_between_order;
    }

}