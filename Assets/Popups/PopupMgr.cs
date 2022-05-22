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
Version : 22.05.21
*/

public struct PopupInfo
{ 
    public string pref_address;  
    public bool overlapping_able;
} 

public struct PopupResult
{ 
    public GameObject obj;
    public Popup comp;
}


/// <summary> The Popup Manager. </summary>
public class PopupMgr : MonoBehaviour
{

    public static Dictionary<Type, PopupInfo> popup_infos;
    public static Dictionary<Type, List<Popup>> popup_pool;

    public static int last_popup_order;
    public static int gap_between_order = 10;
    public static int last_key;
    
    public static bool popup_is_opening;


    private void Start()
    {
        Init();

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

        popup_infos = null;
        popup_infos = new Dictionary<Type, PopupInfo>();

        popup_pool = null;
        popup_pool = new Dictionary<Type, List<Popup>>();
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
        _info.pref_address = typeof(T).ToString();
        _info.overlapping_able = _overlapping_able;

        if (!popup_infos.ContainsKey(typeof(T)))
            popup_infos.Add(typeof(T), _info);
        else
            Debug.LogError(_info.pref_address + " 팝업이 중복으로 초기화되었습니다.");
    }

    /// <summary> Get Popup Data. <br/>- Left Pool : Active Object. <br/>- No Left Pool : Instantiate Object. </summary>
    public static void GetPopup<T>() where T : Popup
    {
        GetPopup<T>( ( PopupResult Result ) => {} );
    }

    /// <summary> Get Popup Data. <br/>- Left Pool : Active Object. <br/>- No Left Pool : Instantiate Object. </summary>
    public static void GetPopup<T>(Action<PopupResult> Result) where T : Popup
    {
        PopupResult _result;
        Type _type = typeof(T);

        if (popup_is_opening)
            return;

        popup_is_opening = true;
    
        if (popup_pool.ContainsKey(_type))
        {
            if (popup_pool[_type].Count > 0)
            {
                _result.obj = popup_pool[_type][0].gameObject;
                _result.comp = popup_pool[_type][0];
                PopupOrderInit(_result.comp);
                popup_pool[_type][0].SetElements( () => {
                    popup_pool[_type][0].Open();
                    popup_is_opening = false;
                    Result(_result);
                });
            }
            else
            {
                Addressables.InstantiateAsync(popup_infos[_type].pref_address, SceneMgr.active_canvas_list[CANVAS_TYPE.EXPAND].trans).Completed += (handle) =>
                {
                    _result.obj = handle.Result;
                    _result.comp = _result.obj.GetComponent<T>();
                    PopupOrderInit(_result.comp);
                    _result.comp.SetElements( () => {
                        popup_pool[_type][0].Open();
                        popup_is_opening = false;
                        Result(_result);
                    });
                };
            }
        }

    }

    private static void PopupOrderInit(Popup _comp)
    {
        last_key ++;
        last_popup_order = popup_pool.Count * gap_between_order;
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

    public static void Pop(Popup _popup)
    {
        popup_pool[_popup.GetType()].Remove(_popup);
    }

    public static void Push(Popup _popup)
    {
        popup_pool[_popup.GetType()].Add(_popup);
    }

}