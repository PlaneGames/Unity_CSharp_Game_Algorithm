using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/*
Developer : Jae Young Kwon
Version : 22.06.02
*/

public class PopupToast : Popup
{
    public Text text_title, text_lore;
    private Sequence seq_text;

    protected override void Init()
    {
        SetOrderLayer(1000);
        PopupAniCtr.SetAni(seq_text, text_title, UI_ANI.FADEIN, 0.25f, 0.3f, () => {});
        PopupAniCtr.SetAni(seq_text, text_lore, UI_ANI.FADEIN, 0.75f, 0.3f, () => {});
    }

    protected override void OnOpened()
    {
        StartCoroutine(AutoClosing());
    }

    protected override void OnClosed()
    {
    }

    private IEnumerator AutoClosing()
    {
        yield return new WaitForSeconds(3f);
        OnClose();
    }

}