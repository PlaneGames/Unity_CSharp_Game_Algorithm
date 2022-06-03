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

public struct GUIInfo
{ 
    public string pref_address;
} 

public struct GUIResult
{
    public GameObject obj;
    public UI_GUI comp;

    public GUIResult(GameObject _obj, UI_GUI _comp)
    {
        this.obj = _obj;
        this.comp = _comp;
    }
}

public class GUIMgr : MonoBehaviour
{

    public static Dictionary<Type, GUIInfo> GUI_infos;
    public static Dictionary<Type, List<UI_GUI>> GUI_pool;
    public static Dictionary<Type, int> GUI_count_in_scene;

    public static int gui_order;


    private void Start()
    {
        Init();
    }

    private void Init()
    {
        gui_order = 0;

        GUI_infos = null;
        GUI_infos = new Dictionary<Type, GUIInfo>();

        GUI_pool = null;
        GUI_pool = new Dictionary<Type, List<UI_GUI>>();

        GUI_count_in_scene = null;
        GUI_count_in_scene = new Dictionary<Type, int>();
    }

    public static void GUIInit<T>()
    {
        GUIInfo _info;
        _info.pref_address = typeof(T).ToString();

        if (!GUI_infos.ContainsKey(typeof(T)))
        {
            GUI_infos.Add(typeof(T), _info);
            List<UI_GUI> _list = new List<UI_GUI>();
            GUI_pool.Add(typeof(T), _list);
            GUI_count_in_scene.Add(typeof(T), 0);
            
        } else
            Debug.LogError(_info.pref_address + "GUI가 중복으로 초기화되었습니다.");
    }

    public static void GetGUI<T>(Action<GUIResult> Result) where T : UI_GUI
    {
        GUIResult _result;
        Type _type = typeof(T);
        
        void _SetInit(GameObject _obj, UI_GUI _GUI)
        {
            _result = new GUIResult(_obj, _GUI);
            Result(_result);
        }

        if (GUI_infos.ContainsKey(_type))
        {
            Addressables.InstantiateAsync(GUI_infos[_type].pref_address, SceneMgr.active_canvas_list[CANVAS_TYPE.EXPAND].trans_popup).Completed += (handle) =>
            {
                _SetInit(handle.Result, handle.Result.GetComponent<T>());
                NameTagCtr.SetUIName(handle.Result);
                GUI_count_in_scene[_type] ++;
            };
        }
        else Debug.LogError(_type + " GUI가 초기화되지 않았습니다.");
    }

}
