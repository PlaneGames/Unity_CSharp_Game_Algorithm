using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEChatBtn : UIElement
{

    public void OnClick()
    {
        PopupMgr.GetPopup<PopupCheatAlert>();
    }

}
