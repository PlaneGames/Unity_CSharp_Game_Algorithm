using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

/*
Developer : Jae Young Kwon
Version : 22.06.08
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
    public Transform trans_pool;
    public Transform trans_popup;
    public Transform trans_GUI;

    public void SetInfo(GameObject _obj, Canvas _canv, CanvasScaler _canvas_scaler, 
                        Transform _trans, Transform _trans_pool, Transform _trans_p, Transform _trans_g)
    {
        obj = _obj;
        canvas = _canv;
        canvas_scaler = _canvas_scaler;
        trans = _trans;
        trans_pool = _trans_pool;
        trans_popup = _trans_p;
        trans_GUI = _trans_g;
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
    public static int loading_commit_count, loading_pushed_count, loading_left_count;
    public static float loading_display;
    public GameObject pref_loading_ani;
    public static List<Type> pooling_list;
    
    private GameObject obj_loading_ani;
    private LoadingBar loading_bar;
    private Stopwatch watch;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        loading_commit_count = 0;
        loading_pushed_count = 0;

        pooling_list = new List<Type>();

        canvas_set_complete = false;

        canvas_pref_address = "Canvas";

        active_canvas_list = null;
        active_canvas_list = new Dictionary<CANVAS_TYPE, CanvasInfo>();

        InitSceneUI<PopupException>(98);
        
        InitSceneUI<GUIExceptionBtn>(89);
        InitSceneUI<GUIShopBtn>(77);
        InitSceneUI<PopupShop>(1);

        // 씬에 풀링 또는 초기 생성 요소들은 모두 로딩에 Commit됨.
        GenCanvas(CANVAS_TYPE.EXPAND, ( CanvasInfo Result ) =>
        {
            obj_loading_ani = Instantiate(pref_loading_ani, active_canvas_list[CANVAS_TYPE.EXPAND].trans) as GameObject;
            loading_bar = obj_loading_ani.GetComponent<LoadingBar>();
            canvas_set_complete = true;
            StartCoroutine(LoadingProgress(15));
            StartCoroutine(LoadingDisplay());
        });
    }

    public void InitSceneUI<T>(int _num)
    {
        if (_num > 0)
        {
            for (int i = 0; i < _num; i ++)
            {
                pooling_list.Add(typeof(T));
            }
        }
        else
            UnityEngine.Debug.LogError( "씬 초기화 갯수는 0개 이상이어야 합니다." );
        //UnityEngine.Debug.Log( _type.BaseType == typeof(Popup) );
    }

    public void OnOpenPopupShop()
    {
        PopupMgr.GetPopup<PopupShop>();
    } 

    public void OnOpenPopupException()
    {
        PopupMgr.GetPopup<PopupException>();
    }

    IEnumerator LoadingProgress(int _multi_tunnel_count)
    {
        if (_multi_tunnel_count > 0)
        {
            int i;
            loading_left_count = pooling_list.Count;
            int _load_id = loading_left_count;
            loading_display = 0;

            watch = new Stopwatch();
            watch.Start();

            while (true)
            {
                int _left_tunnel = _multi_tunnel_count;
                bool _commited = false;
                while (true)
                {
                    if (!_commited)
                    {
                        _commited = true;
                        for (i = 0; i < _multi_tunnel_count; i ++)
                        {
                            _load_id --;
                            if (pooling_list[_load_id].BaseType == typeof(Popup))
                            {
                                PopupMgr.PoolingPopup(pooling_list[_load_id], () => {
                                    _left_tunnel --;
                                    loading_left_count --;
                                });
                            }
                            else if (pooling_list[_load_id].BaseType == typeof(UI_GUI))
                            {
                                GUIMgr.PoolingGUI(pooling_list[_load_id], () => {
                                    _left_tunnel --;
                                    loading_left_count --;
                                });
                            }
                            
                            if (_load_id == 0)
                            {
                                break;
                            }
                        }
                    }

                    if (loading_left_count == 0)
                    {
                        break;
                    }
                    if (_left_tunnel == 0)
                    {
                        break;
                    }
                    yield return null;
                }
                if (loading_left_count == 0)
                {
                    watch.Stop();
                    break;
                }
                yield return null;
            }
            yield return null;
        }
    }

    IEnumerator LoadingDisplay()
    {   
        float _GetPer(int _left)
        {
            return (float)(pooling_list.Count - _left) / (float)pooling_list.Count;
        }

        while (true)
        {
            loading_bar.bar_guage.sizeDelta = Vector2.Lerp(loading_bar.bar_guage.sizeDelta, new Vector2(_GetPer(loading_left_count) * 160f, 12f), 0.2f);
            loading_bar.bar_text.text = ((int)(_GetPer(loading_left_count) * 100f)).ToString() + " %";
            loading_display = Mathf.Lerp(loading_display, pooling_list.Count - loading_left_count, 0.25f);
            loading_bar.bar_text_2.text = (int)loading_display + " / " + pooling_list.Count;
            if (loading_display < pooling_list.Count - 0.02f)
            {
                yield return null;
            }
            else
            {
                break;
            }
        }

        loading_bar.bar_text_2.text = "Loaded In " + watch.ElapsedMilliseconds+"ms.";
        InitUI();
        yield return null;
    }

    public static void InitUI()
    {
        GUIMgr.GetGUI<GUIExceptionBtn>();
        GUIMgr.GetGUI<GUIShopBtn>();
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
        LoadingSceneCommit();
        CanvasInfo _info = new CanvasInfo();
        Addressables.InstantiateAsync(canvas_pref_address).Completed += (handle) =>
        {
            GameObject _obj = handle.Result;
            _obj.transform.SetSiblingIndex(3);
            _info.SetInfo(_obj, _obj.GetComponent<Canvas>(), _obj.GetComponent<CanvasScaler>(), 
                          _obj.transform, _obj.transform.GetChild(0), _obj.transform.GetChild(1), _obj.transform.GetChild(2));
            SetCanvasScaleType(_type, _info);
            active_canvas_list.Add(_type, _info);
            LoadingScenePush();
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

    public static void LoadingSceneCommit()
    {
        loading_commit_count ++;
    }

    public static void LoadingScenePush()
    {
        loading_pushed_count ++;
    }

    private IEnumerator CheckLoadingComplete()
    {
        yield return new WaitForEndOfFrame();
        while (true)
        {
            if (loading_pushed_count == loading_commit_count)
            {
                break;
            }
            
            yield return null;
        }
    }

}