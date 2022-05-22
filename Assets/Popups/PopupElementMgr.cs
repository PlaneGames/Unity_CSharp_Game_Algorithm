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

public struct PopupElementInfo
{ 
    public string pref_address;
}

public struct PopupElementResult
{
    public GameObject obj;
    public PopupElement comp;
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

    /// <summary> Get Popup Element Data. <br/>- Left Pool : Active Object. <br/>- No Left Pool : Instantiate Object. </summary>
    public static void GetPE<T>(Popup _popup, Action<PopupElementResult> Result) where T : PopupElement
    {
        PopupElementResult _result;
        Type _type = typeof(T);

        if (PE_pool.ContainsKey(_type))
        {
            if (PE_pool[_type].Count > 0)
            {
                _result.obj = PE_pool[_type][0].gameObject;
                _result.comp = PE_pool[_type][0];
                PE_pool[_type][0].transform.SetParent(_popup.transform);
                PE_pool[_type][0].OnOpened();
                Result(_result);
            }
            else
            {
                Addressables.InstantiateAsync(PE_infos[_type].pref_address, _popup.transform).Completed += (handle) =>
                {
                    _result.obj = handle.Result;
                    _result.comp = _result.obj.GetComponent<T>();
                    _result.comp.OnOpened();
                    Result(_result);
                };
            }
        }
    }

    public static void Pop(PopupElement _pe)
    {
        PE_pool[_pe.GetType()].Remove(_pe);
    }

    public static void Push(PopupElement _pe)
    {
        PE_pool[_pe.GetType()].Add(_pe);
    }

}