using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEExceptionBtn : UIElement
{

    public void OnClick()
    {
        PopupMgr.GetPopup<PopupException>(( Result ) => 
        {
            PopupException _shop = (PopupException)Result.comp;
            _shop.SetDisplay();
        });
        PopupMgr.GetPopup<PopupToast>();
    }

}
