using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEShopBtn : UIElement
{

    public void OnClick()
    {
        PopupMgr.GetPopup<PopupShop>();
        PopupMgr.GetPopup<PopupAlert>();
    }

}
