using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/*
Developer : Jae Young Kwon
Version : 22.06.20
*/

public class UIP_CheatAlert : UI_Popup
{ 

    protected override void Init()
    {
    }

    protected override void OnOpened()
    {
    }

    protected override void OnClosed()
    {
    }

    public void OnUpBtnClick()
    {
        Client.ReqChatRoomOpen("문을여시오.", 1234);
    }    
    
    public void OnDownBtnClick()
    {
        Client.ReqChatRoomJoin("문을여시오.", 1234);
    }

}