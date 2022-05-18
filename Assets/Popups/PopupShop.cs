using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class PopupShop : Popup
{

    public override void OnClosed()
    {
        Debug.Log(this);
    }

}