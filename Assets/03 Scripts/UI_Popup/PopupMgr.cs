using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/*
Developer : Jae Young Kwon
Version : 22.06.03
*/

public struct PopupInfo
{ 
    public string pref_address;  
    public bool overlapping_able;
    public bool ordering_able;
} 

public struct PopupResult
{ 
    public GameObject obj;
    public Popup comp;

    public PopupResult(GameObject _obj, Popup _comp)
    {
        this.obj = _obj;
        this.comp = _comp;
    }
}


/// <summary>  
/// The Popup Manager. For Create, delete, And Manage. 
/// <br/> - PopupInit : Only once, it should be used at the beginning of the scene.
/// <br/> - GetPopup  : Can "pop up!" the scene.
/// </summary>
public class PopupMgr : MonoBehaviour
{
    public static Dictionary<Type, PopupInfo> popup_infos;
    public static Dictionary<Type, List<Popup>> popup_pool;
    public static Dictionary<Type, int> popup_count_in_scene;

    public static int last_popup_order;
    public static int gap_between_order = 20;
    public static int last_key;
    
    public static bool popup_is_opening;


    private void Start()
    {
        Init();

        PopupInit<PopupShop>();
        PopupInit<PopupException>(true, true);
        PopupInit<PopupToast>(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GetPopup<PopupShop>();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            GetPopup<PopupException>();
        }        
        if (Input.GetKeyDown(KeyCode.E))
        {
            GetPopup<PopupToast>();
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

        popup_count_in_scene = null;
        popup_count_in_scene = new Dictionary<Type, int>();
    }

    /// <summary> Initialize The Popup. <br/>Create The GameObject In The Current Scene. </summary>
    public static void PopupInit<T>()
    {
        PopupInit<T>(false, true);
    }

    /// <summary> Initialize The Popup. <br/>Create The GameObject In The Current Scene. </summary>
    public static void PopupInit<T>(bool _order_able)
    {
        PopupInit<T>(false, _order_able);
    }

    /// <summary> Initialize The Popup. <br/>Create The GameObject In The Current Scene. </summary>
    public static void PopupInit<T>(bool _overlapping_able, bool _order_able)
    {
        PopupInfo _info;
        _info.pref_address = typeof(T).ToString();
        _info.overlapping_able = _overlapping_able;
        _info.ordering_able = _order_able;

        if (!popup_infos.ContainsKey(typeof(T)))
        {
            popup_infos.Add(typeof(T), _info);
            List<Popup> _list = new List<Popup>();
            popup_pool.Add(typeof(T), _list);
            popup_count_in_scene.Add(typeof(T), 0);
            
        } else
            Debug.LogError(_info.pref_address + "팝업이 중복으로 초기화되었습니다.");
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
        
        void _SetInit(GameObject _obj, Popup _popup)
        {
            _result = new PopupResult(_obj, _popup);
            _popup.transform.SetParent(SceneMgr.active_canvas_list[CANVAS_TYPE.EXPAND].trans_popup);
            if (popup_infos[_type].ordering_able)
            {
                PopupOrderInit(_result.comp);
            }
            PopupMgr.Pop(_result.comp);
            _obj.SetActive(true);
            _popup.rect.localScale = new Vector2(0f, 0f);
            _popup.GetPEs( () => {
                _popup.OnOpen();
                popup_is_opening = false;
                Result(_result);
            });
        }

        if (popup_pool.ContainsKey(_type))
        {
            if (popup_pool[_type].Count > 0)
            {
                _SetInit(popup_pool[_type][0].gameObject, popup_pool[_type][0]);
            }
            else if ( (!popup_infos[_type].overlapping_able && popup_count_in_scene[_type] == 0) || (popup_infos[_type].overlapping_able) )
            {
                Addressables.InstantiateAsync(popup_infos[_type].pref_address, SceneMgr.active_canvas_list[CANVAS_TYPE.EXPAND].trans_popup).Completed += (handle) =>
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

    /// <summary> Get Popup Data. <br/>- Left Pool : Active Object. <br/>- No Left Pool : Instantiate Object. </summary>
    public static void PoolingPopup<T>(Action Result) where T : Popup
    {
        PopupResult _result;
        Type _type = typeof(T);
        
        void _SetInit(GameObject _obj, Popup _popup)
        {
            _result = new PopupResult(_obj, _popup);
            _obj.SetActive(true);
            _popup.PoolingPEs( () => {
                Push(_popup);
                _obj.SetActive(false);
                SceneMgr.LoadingScenePush();
                Result();
            });
        }

        if (popup_pool.ContainsKey(_type))
        {
            if ( (!popup_infos[_type].overlapping_able && popup_count_in_scene[_type] == 0) || (popup_infos[_type].overlapping_able) )
            {
                SceneMgr.LoadingSceneCommit();
                Addressables.InstantiateAsync(popup_infos[_type].pref_address, SceneMgr.active_canvas_list[CANVAS_TYPE.EXPAND].trans_popup).Completed += (handle) =>
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
        MethodInfo get_pe = typeof(PopupMgr).GetMethod("PoolingPopup", new Type[] { typeof(Action) } );
        get_pe = get_pe.MakeGenericMethod(_type);
        get_pe.Invoke(null, new object[] { Result });
    }

    private static void PopupOrderInit(Popup _comp)
    {
        last_popup_order ++;
        _comp.SetOrderLayer(last_popup_order * gap_between_order);
    }

    public static void PopupOrderPop(Popup _comp)
    {
        if (popup_infos[_comp.GetType()].ordering_able)
            last_popup_order --;
    }

    public static void Pop(Popup _popup)
    {
        popup_pool[_popup.GetType()].Remove(_popup);
    }

    public static void Push(Popup _popup)
    {
        popup_pool[_popup.GetType()].Add(_popup);
        _popup.transform.SetParent(SceneMgr.active_canvas_list[CANVAS_TYPE.EXPAND].trans_pool);
    }

}