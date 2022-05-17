using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class PopupCover : Popup
{

    public override void OnClosed()
    {
        Debug.Log(this);
        CloseComplete();
    }

}