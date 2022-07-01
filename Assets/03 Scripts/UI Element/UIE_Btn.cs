using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIE_Btn : UI_Element
{

    [SerializeField] Button btn;


    public void SetBtnOnClick(UnityEngine.Events.UnityAction Func)
    {
        btn.onClick.AddListener(() => { Func(); });
    }

}
