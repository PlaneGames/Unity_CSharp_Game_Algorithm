using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTonMonobehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    static T _instance = null;
    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                //_instance가 널인데 혹시 어디 있을지도 모르니 한번 찾아본다.
                _instance = (T)FindObjectOfType(typeof(T));
                if(_instance == null)
                {
                    var _newGameObject = new GameObject(typeof(T).ToString());
                    _instance = _newGameObject.AddComponent<T>();
                }
                
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if(_instance == null)
        {
            _instance = this as T;
        }
        DontDestroyOnLoad(gameObject);
        
    }
}