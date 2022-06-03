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

public enum PE_NAME
{
    PopupElementCover,
}

public struct PopupElementInfo
{ 
    public string pref_address;
}

public struct PopupElementResult
{
    public GameObject obj;
    public PopupElement comp;

    public PopupElementResult(GameObject _obj, PopupElement _pe)
    {
        obj = _obj;
        comp = _pe;
    }
}

/// <summary> The Popup Element Manager. <br/> - 'PE' Means 'Popup Element'. </summary>
public class PopupElementMgr : MonoBehaviour
{

    public static Dictionary<Type, PopupElementInfo> PE_infos;
    public static Dictionary<Type, List<PopupElement>> PE_pool;

    private void Start()
    {
        Init();

        PEInit<PopupElementCover>();

        List<PopupElement> _list = new List<PopupElement>();
        PE_pool.Add(typeof(PopupElementCover), _list);
    }

    private void Init()
    {
        PE_infos = null;
        PE_infos = new Dictionary<Type, PopupElementInfo>();

        PE_pool = null;
        PE_pool = new Dictionary<Type, List<PopupElement>>();
    }

    /// <summary> Initialize The Popup. <br/>Create The GameObject In The Current Scene. </summary>
    public static void PEInit<T>()
    {
        PopupElementInfo _info;
        _info.pref_address = typeof(T).ToString();

        if (!PE_infos.ContainsKey(typeof(T)))
            PE_infos.Add(typeof(T), _info);
        else
            Debug.LogError(_info.pref_address + " 팝업이 중복으로 초기화되었습니다.");
    }

    public static void GetPE(Type _type, Popup _popup, Action<PopupElementResult> Result)
    {
        MethodInfo get_pe = typeof(PopupElementMgr).GetMethod("GetPE", new Type[] { typeof(Popup), typeof(Action<PopupElementResult>) } );
        get_pe = get_pe.MakeGenericMethod(_type);
        get_pe.Invoke(null, new object[] { _popup, Result });
    }

    /// <summary> Get Popup Element Data. <br/>- Left Pool : Active Object. <br/>- No Left Pool : Instantiate Object. </summary>
    public static void GetPE<T>(Popup _popup, Action<PopupElementResult> Result) where T : PopupElement
    {
        PopupElementResult _result;
        Type _type = typeof(T);

        void _SetInit(GameObject _obj, PopupElement _pe)
        {
            _result = new PopupElementResult(_obj, _pe);
            _obj.transform.SetParent(_popup.transform);
            PopupElementMgr.Pop(_pe);
            _obj.SetActive(true);
            Result(_result);
        }

        if (PE_pool.ContainsKey(_type))
        {
            if (PE_pool[_type].Count > 0)
            {
                _SetInit(PE_pool[_type][0].gameObject, PE_pool[_type][0]);
            }
            else
            {
                Addressables.InstantiateAsync(PE_infos[_type].pref_address, _popup.transform).Completed += (handle) =>
                {
                    _SetInit(handle.Result, handle.Result.GetComponent<T>());
                    NameTagCtr.SetUIName(handle.Result);
                };
            }
        }
        else Debug.LogError(_type + " 팝업이 초기화되지 않았습니다.");
    }

    public static void PoolingPE(Type _type, Popup _popup, Action<PopupElementResult> Result)
    {
        MethodInfo get_pe = typeof(PopupElementMgr).GetMethod("PoolingPE", new Type[] { typeof(Popup), typeof(Action<PopupElementResult>) } );
        get_pe = get_pe.MakeGenericMethod(_type);
        get_pe.Invoke(null, new object[] { _popup, Result });
    }

    /// <summary> Get Popup Element Data. <br/>- Left Pool : Active Object. <br/>- No Left Pool : Instantiate Object. </summary>
    public static void PoolingPE<T>(Popup _popup, Action<PopupElementResult> Result) where T : PopupElement
    {
        PopupElementResult _result;
        Type _type = typeof(T);

        void _SetInit(GameObject _obj, PopupElement _pe)
        {
            _result = new PopupElementResult(_obj, _pe);
            Push(_pe);
            _obj.SetActive(false);
            Result(_result);
        }

        if (PE_pool.ContainsKey(_type))
        {
            Addressables.InstantiateAsync(PE_infos[_type].pref_address, _popup.transform).Completed += (handle) =>
            {
                _SetInit(handle.Result, handle.Result.GetComponent<T>());
                NameTagCtr.SetUIName(handle.Result);
            };
        }
        else Debug.LogError(_type + " 팝업이 초기화되지 않았습니다.");
    }

    public static void Pop(PopupElement _pe)
    {
        PE_pool[_pe.GetType()].Remove(_pe);
        Debug.Log(PE_pool[_pe.GetType()].Count);
    }

    public static void Push(PopupElement _pe)
    {
        PE_pool[_pe.GetType()].Add(_pe);
        Debug.Log(PE_pool[_pe.GetType()].Count);
        _pe.transform.SetParent(SceneMgr.active_canvas_list[CANVAS_TYPE.EXPAND].trans_pool);
    }

}