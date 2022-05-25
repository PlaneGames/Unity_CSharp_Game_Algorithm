using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

/*
Developer : Jae Young Kwon
Version : 22.05.19
*/

public enum CANVAS_TYPE
{
    MATCH_WIDTH,
    MATCH_HEIGHT,
    MATCH_CENTER,
    MATCH_CUSTOM,
    EXPAND,
    SHRINK,
}

public struct CanvasInfo
{
    public GameObject obj;
    public Canvas canvas;
    public CanvasScaler canvas_scaler;
    public Transform trans;
    public Transform trans_popup;
    public Transform trans_popup_pool;

    public void SetInfo(GameObject _obj, Canvas _canv, CanvasScaler _canvas_scaler, 
                        Transform _trans, Transform _trans_p, Transform _trans_ppool)
    {
        obj = _obj;
        canvas = _canv;
        canvas_scaler = _canvas_scaler;
        trans = _trans;
        trans_popup = _trans_p;
        trans_popup_pool = _trans_ppool;
    }

    public void SetName(string _name)
    {
        obj.name = _name;
    }
}


/// <summary> The Scene Manager. </summary>
public class SceneMgr : MonoBehaviour
{
    private static string canvas_pref_address;
    public static Dictionary<CANVAS_TYPE, CanvasInfo> active_canvas_list;
    public static bool canvas_set_complete;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        canvas_set_complete = false;

        canvas_pref_address = "Canvas";

        active_canvas_list = null;
        active_canvas_list = new Dictionary<CANVAS_TYPE, CanvasInfo>();

        GenCanvas(CANVAS_TYPE.EXPAND, ( CanvasInfo Result ) => 
        {
            canvas_set_complete = true;
        });
    }

    public static void GetCanvas(CANVAS_TYPE _type, Action<CanvasInfo> Result)
    {
        if (active_canvas_list.ContainsKey(_type))
        {
            Result(active_canvas_list[_type]);
        }
        else
        {
            GenCanvas(_type, Result);
        }
    }

    public static void GenCanvas(CANVAS_TYPE _type, Action<CanvasInfo> Result)
    {
        CanvasInfo _info = new CanvasInfo();
        Addressables.InstantiateAsync(canvas_pref_address).Completed += (handle) =>
        {
            GameObject _obj = handle.Result;
            _obj.transform.SetSiblingIndex(3);
            _info.SetInfo(_obj, _obj.GetComponent<Canvas>(), _obj.GetComponent<CanvasScaler>(), 
                          _obj.transform, _obj.transform.GetChild(0), _obj.transform.GetChild(0).GetChild(0));
            SetCanvasScaleType(_type, _info);
            active_canvas_list.Add(_type, _info);
            Result(_info);
        };
    }

    public static void SetCanvasScaleType(CANVAS_TYPE _type, CanvasInfo _canv_info)
    {
        var _canv_s = _canv_info.canvas_scaler;
        switch (_type)
        {
            case CANVAS_TYPE.MATCH_WIDTH:
            _canv_s.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            _canv_s.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            _canv_s.matchWidthOrHeight = 0f;
            _canv_info.SetName("@ Canvas_Width");
            break;

            case CANVAS_TYPE.MATCH_HEIGHT:
            _canv_s.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            _canv_s.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            _canv_s.matchWidthOrHeight = 1f;
            _canv_info.SetName("@ Canvas_Height");
            break;

            case CANVAS_TYPE.MATCH_CENTER:
            _canv_s.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            _canv_s.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            _canv_s.matchWidthOrHeight = 0.5f;
            _canv_info.SetName("@ Canvas_Center");
            break;

            case CANVAS_TYPE.MATCH_CUSTOM:
            _canv_s.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            _canv_s.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            _canv_s.matchWidthOrHeight = 0.5f;
            _canv_info.SetName("@ Canvas_Custom");
            break;

            case CANVAS_TYPE.EXPAND:
            _canv_s.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            _canv_s.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            _canv_info.SetName("@ Canvas_Expand");
            break;

            case CANVAS_TYPE.SHRINK:
            _canv_s.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            _canv_s.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
            _canv_info.SetName("@ Canvas_Shrink");
            break;
        }
    }

    public static void SetCustomCanvasScale(CanvasScaler _canv_s, float _match_value)
    {
        _canv_s.matchWidthOrHeight = _match_value;
    }

}