using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;


public struct TroubleShooter
{
    public bool success;
    public ErrorCode error;

    public void Successed ()
    {
        success = true;
    }

    public void HasError (ErrorCode _error)
    {
        success = false;
        error = _error;
        UI_PopupMgr.GetPopup<UIP_Exception>();
    }
}

public enum ErrorCode
{
    None = 0,
    ChatRoomNotInitialized,
    ChatRoomPWIsWrong,
}