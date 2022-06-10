using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIShopBtn : UI_GUI
{

    public void OnClick()
    {
        PopupMgr.GetPopup<PopupShop>();
        PopupMgr.GetPopup<PopupAlert>();
    }

}
