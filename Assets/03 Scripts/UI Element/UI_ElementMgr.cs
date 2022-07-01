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


public struct UI_ElementInfo
{
    public string pref_address;
}

public struct UI_ElementResult
{
    public GameObject obj;
    public UI_Element comp;

    public UI_ElementResult(GameObject _obj, UI_Element _pe)
    {
        obj = _obj;
        comp = _pe;
    }
}

// Managers

public class UI_ElementMgr : SingleTonMonobehaviour<UI_ElementMgr>
{
    public static Dictionary<Type, UI_ElementInfo> infos;
    public static Dictionary<Type, List<UI_Element>> pool;


    private void Start()
    {
        Init();
        InitUIE<UIE_Panel>();
        InitUIE<UIE_ChatBtn>();
    }

    private void Init()
    {
        infos = null;
        infos = new Dictionary<Type, UI_ElementInfo>();

        pool = null;
        pool = new Dictionary<Type, List<UI_Element>>();
    }

    /// <summary> Initialize The Popup. <br/>Create The GameObject In The Current Scene. </summary>
    public static void InitUIE<T>()
    {
        UI_ElementInfo _info;
        _info.pref_address = typeof(T).ToString();

        if (!infos.ContainsKey(typeof(T)))
        {
            infos.Add(typeof(T), _info);
            List<UI_Element> _list = new List<UI_Element>();
            pool.Add(typeof(T), _list);
        } else
            Debug.LogError(_info.pref_address + " 팝업이 중복으로 초기화되었습니다.");
    }

    /// <summary> Get Popup Element Data. <br/>- Left Pool : Active Object. <br/>- No Left Pool : Instantiate Object. </summary>
    public static void GetUIE(Type _type, Action<UI_ElementResult> Result)
    {
        UI_ElementResult _result;

        void _SetInit(GameObject _obj, UI_Element _pe)
        {
            _result = new UI_ElementResult(_obj, _pe);
            _obj.transform.SetParent(SceneMgr.active_canvas_list[CANVAS_TYPE.EXPAND].trans_UIE);
            UI_ElementMgr.Pop(_pe);
            _obj.SetActive(true);
            Result(_result);
        }

        if (pool.ContainsKey(_type))
        {
            if (pool[_type].Count > 0)
            {
                _SetInit(pool[_type][0].gameObject, pool[_type][0]);
            }
            else
            {
                Addressables.InstantiateAsync(infos[_type].pref_address, SceneMgr.active_canvas_list[CANVAS_TYPE.EXPAND].trans_UIE).Completed += (handle) =>
                {
                    _SetInit(handle.Result, (UI_Element)handle.Result.GetComponent(_type.ToString()));
                    NameTagCtr.SetUIName(handle.Result);
                };
            }
        }
        else Debug.LogError(_type + " 팝업이 초기화되지 않았습니다.");
    }

    /// <summary> Get Popup Element Data. <br/>- Left Pool : Active Object. <br/>- No Left Pool : Instantiate Object. </summary>
    public static void PoolingUIE(Type _type, Action<UI_ElementResult> Result)
    {
        UI_ElementResult _result;

        void _SetInit(GameObject _obj, UI_Element _pe)
        {
            _result = new UI_ElementResult(_obj, _pe);
            Push(_pe);
            _obj.SetActive(false);
            Result(_result);
        }

        if (pool.ContainsKey(_type))
        {
            Addressables.InstantiateAsync(infos[_type].pref_address, SceneMgr.active_canvas_list[CANVAS_TYPE.EXPAND].trans_UIE).Completed += (handle) =>
            {
                _SetInit(handle.Result, (UI_Element)handle.Result.GetComponent(_type));
                NameTagCtr.SetUIName(handle.Result);
            };
        }
        else Debug.LogError(_type + " 팝업이 초기화되지 않았습니다.");
    }

    public static void Pop(UI_Element _pe)
    {
        pool[_pe.GetType()].Remove(_pe);
    }

    public static void Push(UI_Element _pe)
    {
        pool[_pe.GetType()].Add(_pe);
        _pe.transform.SetParent(SceneMgr.active_canvas_list[CANVAS_TYPE.EXPAND].trans_pool);
    }
}