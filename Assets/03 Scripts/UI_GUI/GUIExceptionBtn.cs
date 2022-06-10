using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIExceptionBtn : UI_GUI
{

    public void OnClick()
    {
        PopupMgr.GetPopup<PopupException>();
        PopupMgr.GetPopup<PopupToast>();
    }

}
