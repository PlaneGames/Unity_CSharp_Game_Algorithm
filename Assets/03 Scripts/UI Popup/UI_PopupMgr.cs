using System;
using System.Reflection;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/*
Developer : Jae Young Kwon
Version : 22.07.01
*/


public struct UI_PopupInfo
{
    public string pref_address;
    public bool overlapping_able;
    public bool ordering_able;
}

public struct UI_PopupResult
{
    public GameObject obj;
    public UI_Popup comp;

    public UI_PopupResult(GameObject _obj, UI_Popup _popup)
    {
        obj = _obj;
        comp = _popup;
    }
}

// Managers

/// <summary>  
/// The UI_Popup Manager. For Create, delete, And Manage. 
/// <br/> - PopupInit : Only once, it should be used at the beginning of the scene.
/// <br/> - GetPopup  : Can "pop up!" the scene.
/// </summary>
public class UI_PopupMgr : MonoBehaviour
{
    public static Dictionary<Type, UI_PopupInfo> infos;
    public static Dictionary<Type, List<UI_Popup>> pool;
    public static Dictionary<Type, int> popup_count_in_scene;

    public static int last_popup_order;
    public static int gap_between_order = 20;
    public static int last_key;
    
    public static bool popup_is_opening;


    private void Start()
    {
        Init();

        PopupInit<UIP_Shop>();
        PopupInit<UIP_Alert>();
        PopupInit<UIP_CheatAlert>();
        PopupInit<UIP_ChatRoom>();
        PopupInit<UIP_Exception>(true, true);
        PopupInit<UIP_Toast>(false);
    }

    private void Init()
    {
        last_popup_order = 0;
        last_key = 0;

        infos = null;
        infos = new Dictionary<Type, UI_PopupInfo>();

        pool = null;
        pool = new Dictionary<Type, List<UI_Popup>>();

        popup_count_in_scene = null;
        popup_count_in_scene = new Dictionary<Type, int>();
    }

    /// <summary> Initialize The UI_Popup. <br/>Create The GameObject In The Current Scene. </summary>
    public static void PopupInit<T>()
    {
        PopupInit<T>(false, true);
    }

    /// <summary> Initialize The UI_Popup. <br/>Create The GameObject In The Current Scene. </summary>
    public static void PopupInit<T>(bool _order_able)
    {
        PopupInit<T>(false, _order_able);
    }

    /// <summary> Initialize The UI_Popup. <br/>Create The GameObject In The Current Scene. </summary>
    public static void PopupInit<T>(bool _overlapping_able, bool _order_able)
    {
        UI_PopupInfo _info;
        _info.pref_address = typeof(T).ToString();
        _info.overlapping_able = _overlapping_able;
        _info.ordering_able = _order_able;

        if (!infos.ContainsKey(typeof(T)))
        {
            infos.Add(typeof(T), _info);
            List<UI_Popup> _list = new List<UI_Popup>();
            pool.Add(typeof(T), _list);
            popup_count_in_scene.Add(typeof(T), 0);
            
        } else
            Debug.LogError(_info.pref_address + "팝업이 중복으로 초기화되었습니다.");
    }

    /// <summary> Get UI_Popup Data. <br/>- Left Pool : Active Object. <br/>- No Left Pool : Instantiate Object. </summary>
    public static void GetPopup<T>() where T : UI_Popup
    {
        GetPopup<T>( ( UI_PopupResult Result ) => {} );
    }

    /// <summary> Get UI_Popup Data. <br/>- Left Pool : Active Object. <br/>- No Left Pool : Instantiate Object. </summary>
    public static void GetPopup<T>(Action<UI_PopupResult> Result) where T : UI_Popup
    {
        UI_PopupResult _result;
        Type _type = typeof(T);

        if (popup_is_opening)
            return;

        popup_is_opening = true;
        
        void _SetInit(GameObject _obj, UI_Popup _popup)
        {
            _result = new UI_PopupResult(_obj, _popup);
            _popup.transform.SetParent(SceneMgr.active_canvas_list[CANVAS_TYPE.EXPAND].trans_popup);
            if (infos[_type].ordering_able)
            {
                PopupOrderInit(_result.comp);
            }
            UI_PopupMgr.Pop(_result.comp);
            _obj.SetActive(true);
            _popup.rect.localScale = new Vector2(0f, 0f);
            _popup.GetPEs( () => {
                _popup.OnOpen();
                popup_is_opening = false;
                Result(_result);
            });
        }

        if (pool.ContainsKey(_type))
        {
            if (pool[_type].Count > 0)
            {
                _SetInit(pool[_type][0].gameObject, pool[_type][0]);
            }
            else if ( (!infos[_type].overlapping_able && popup_count_in_scene[_type] == 0) || (infos[_type].overlapping_able) )
            {
                Addressables.InstantiateAsync(infos[_type].pref_address, SceneMgr.active_canvas_list[CANVAS_TYPE.EXPAND].trans_popup).Completed += (handle) =>
                {
                    _SetInit(handle.Result, handle.Result.GetComponent<T>());
                    NameTagCtr.SetUIName(handle.Result);
                    popup_count_in_scene[_type] ++;
                };
            }
            else
            {
                popup_is_opening = false;
            }
        }
        else Debug.LogError(_type + " 팝업이 초기화되지 않았습니다.");
    }

    /// <summary> Pooling UI_Popup. <br/>- Instantiate Object And Move To Canvas Pool </summary>
    public static void PoolingPopup<T>(Action Result) where T : UI_Popup
    {
        UI_PopupResult _result;
        Type _type = typeof(T);
        
        void _SetInit(GameObject _obj, UI_Popup _popup)
        {
            _result = new UI_PopupResult(_obj, _popup);
            _obj.SetActive(true);
            _popup.PoolingPEs( () => {
                Push(_popup);
                _obj.SetActive(false);
                SceneMgr.LoadingScenePush();
                Result();
            });
        }

        if (pool.ContainsKey(_type))
        {
            if ( (!infos[_type].overlapping_able && popup_count_in_scene[_type] == 0) || (infos[_type].overlapping_able) )
            {
                SceneMgr.LoadingSceneCommit();
                Addressables.InstantiateAsync(infos[_type].pref_address, SceneMgr.active_canvas_list[CANVAS_TYPE.EXPAND].trans_popup).Completed += (handle) =>
                {
                    _SetInit(handle.Result, handle.Result.GetComponent<T>());
                    NameTagCtr.SetUIName(handle.Result);
                    popup_count_in_scene[_type] ++;
                };
            }
            else
            {
                Debug.LogError(_type + " 팝업이 이미 생성 최대치에 도달하였습니다. (중복 생성 불가.)");
            }
        }
        else Debug.LogError(_type + " 팝업이 초기화되지 않았습니다.");
    }

    public static void PoolingPopup(Type _type, Action Result)
    {
        MethodInfo get_pe = typeof(UI_PopupMgr).GetMethod("PoolingPopup", new Type[] { typeof(Action) } );
        get_pe = get_pe.MakeGenericMethod(_type);
        get_pe.Invoke(null, new object[] { Result });
    }

    private static void PopupOrderInit(UI_Popup _comp)
    {
        last_popup_order ++;
        _comp.SetOrderLayer(last_popup_order * gap_between_order);
    }

    public static void PopupOrderPop(UI_Popup _comp)
    {
        if (infos[_comp.GetType()].ordering_able)
            last_popup_order --;
    }

    public static void Pop(UI_Popup _popup)
    {
        pool[_popup.GetType()].Remove(_popup);
    }

    public static void Push(UI_Popup _popup)
    {
        pool[_popup.GetType()].Add(_popup);
        _popup.transform.SetParent(SceneMgr.active_canvas_list[CANVAS_TYPE.EXPAND].trans_pool);
    }

}