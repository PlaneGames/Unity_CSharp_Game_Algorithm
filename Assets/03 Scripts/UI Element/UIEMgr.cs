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
Version : 22.06.08
*/

public struct UIEInfo
{ 
    public string pref_address;
} 

public struct UIEResult
{
    public GameObject obj;
    public UIElement comp;

    public UIEResult(GameObject _obj, UIElement _comp)
    {
        this.obj = _obj;
        this.comp = _comp;
    }
}

public class UIEMgr : MonoBehaviour
{
    public static Dictionary<Type, UIEInfo> UIE_infos;
    public static Dictionary<Type, List<UIElement>> UIE_pool;
    public static Dictionary<Type, int> UIE_count_in_scene;

    public static int UIE_order;


    private void Start()
    {
        Init();

        UIEInit<UIEShopBtn>();
        UIEInit<UIEExceptionBtn>();
        UIEInit<UIEChatBtn>();
    }

    private void Init()
    {
        UIE_order = 0;

        UIE_infos = null;
        UIE_infos = new Dictionary<Type, UIEInfo>();

        UIE_pool = null;
        UIE_pool = new Dictionary<Type, List<UIElement>>();

        UIE_count_in_scene = null;
        UIE_count_in_scene = new Dictionary<Type, int>();
    }

    public static void UIEInit<T>()
    {
        UIEInfo _info;
        _info.pref_address = typeof(T).ToString();

        if (!UIE_infos.ContainsKey(typeof(T)))
        {
            UIE_infos.Add(typeof(T), _info);
            List<UIElement> _list = new List<UIElement>();
            UIE_pool.Add(typeof(T), _list);
            UIE_count_in_scene.Add(typeof(T), 0);
            
        } else
            Debug.LogError(_info.pref_address + "UI Element가 중복으로 초기화되었습니다.");
    }

    public static void GetUIE<T>() where T : UIElement
    {
        GetUIE<T>( (Result) => {} );
    }

    public static void GetUIE<T>(Action<UIEResult> Result) where T : UIElement
    {
        UIEResult _result;
        Type _type = typeof(T);
        
        void _SetInit(GameObject _obj, UIElement _UIE)
        {
            _result = new UIEResult(_obj, _UIE);
            _obj.SetActive(true);
            _UIE.OnOpen();
            _UIE.transform.SetParent(SceneMgr.active_canvas_list[CANVAS_TYPE.EXPAND].trans_UIE);
            Result(_result);
        }

        if (UIE_infos.ContainsKey(_type))
        {
            if (UIE_pool[_type].Count > 0)
            {
                _SetInit(UIE_pool[_type][0].gameObject, UIE_pool[_type][0]);
            }
            else
            {
                Addressables.InstantiateAsync(UIE_infos[_type].pref_address, SceneMgr.active_canvas_list[CANVAS_TYPE.EXPAND].trans_UIE).Completed += (handle) =>
                {
                    _SetInit(handle.Result, handle.Result.GetComponent<T>());
                    NameTagCtr.SetUIName(handle.Result);
                    UIE_count_in_scene[_type] ++;
                };
            }
        }
        else Debug.LogError(_type + "UI Element가 초기화되지 않았습니다.");
    }

    public static void PoolingUIE<T>(Action Result) where T : UIElement
    {
        Type _type = typeof(T);
        
        void _SetInit(GameObject _obj, UIElement _UIE)
        {
            Push(_UIE);
            _obj.SetActive(false);
            Result();
        }

        if (UIE_infos.ContainsKey(_type))
        {
            Addressables.InstantiateAsync(UIE_infos[_type].pref_address, SceneMgr.active_canvas_list[CANVAS_TYPE.EXPAND].trans_UIE).Completed += (handle) =>
            {
                _SetInit(handle.Result, handle.Result.GetComponent<T>());
                NameTagCtr.SetUIName(handle.Result);
                UIE_count_in_scene[_type] ++;
            };
        }
        else Debug.LogError(_type + "UI Element가 초기화되지 않았습니다.");
    }

    public static void PoolingUIE(Type _type, Action Result)
    {
        MethodInfo get_pe = typeof(UIEMgr).GetMethod("PoolingUIE", new Type[] { typeof(Action) } );
        get_pe = get_pe.MakeGenericMethod(_type);
        get_pe.Invoke(null, new object[] { Result });
    }

    public static void Pop(UIElement _UIE)
    {
        UIE_pool[_UIE.GetType()].Remove(_UIE);
    }

    public static void Push(UIElement _UIE)
    {
        UIE_pool[_UIE.GetType()].Add(_UIE);
        _UIE.transform.SetParent(SceneMgr.active_canvas_list[CANVAS_TYPE.EXPAND].trans_pool);
    }

}
