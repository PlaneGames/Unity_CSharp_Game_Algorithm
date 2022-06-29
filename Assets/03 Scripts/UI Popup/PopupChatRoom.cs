using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

/*
Developer : Jae Young Kwon
Version : 22.06.29
*/

public class PopupChatRoom : Popup
{
    [SerializeField] Transform trans_talk_block_cont;
    [SerializeField] ScrollRect scroll_rect;
    [SerializeField] ContentSizeFitter size_fitter;
    [SerializeField] TMP_InputField input_field;
    [SerializeField] GameObject pref_talk_block_me;
    [SerializeField] GameObject pref_talk_block_other;

    ChatTalkBlockMe last_talk_block_me;
    ChatTalkBlockOther last_talk_block_other;

    Sequence seq_scroll;
    string last_time;
    int last_uuid;
    

    protected override void Init()
    {
    }

    protected override void OnOpened()
    {
    }

    protected override void OnClosed()
    {
    }

    public void OnInputTalk(TextMeshProUGUI _text)
    {
        Client.ReqChatSendMsg("길동이", input_field.text);
        input_field.Select();
        input_field.text = "";
    }

    public void CreateMyTalkBalloon(int UUID, string _time, string _talk)
    {
        if (CheckTalkBoxAddAble(UUID, _time))
        {
            last_talk_block_me.AddTalkBalloon(_talk);
        }
        else
        {
            var _obj = Instantiate(pref_talk_block_me, trans_talk_block_cont) as GameObject;
            last_talk_block_me = _obj.GetComponent<ChatTalkBlockMe>();
            last_talk_block_me.SetInfo(_time, _talk);
            last_uuid = UUID;
            last_time = _time;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)size_fitter.transform);
        UpdateScrollSmoothly();
    }

    public void CreateOtherTalkBalloon(int UUID, string _name, string _time, string _talk)
    {
        if (CheckTalkBoxAddAble(UUID, _time))
        {
            last_talk_block_other.AddTalkBalloon(_talk);
        }
        else
        {
            var _obj = Instantiate(pref_talk_block_other, trans_talk_block_cont) as GameObject;
            last_talk_block_other = _obj.GetComponent<ChatTalkBlockOther>();
            last_talk_block_other.SetInfo(_name, _time, _talk);
            last_uuid = UUID;
            last_time = _time;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)size_fitter.transform);
        UpdateScrollSmoothly();
    }

    void UpdateScrollSmoothly()
    {
        seq_scroll.Kill();
        seq_scroll = DOTween.Sequence();
        seq_scroll
        .Insert(0f, scroll_rect.DONormalizedPos(Vector2.zero, 0.5f).SetEase(Ease.OutQuad));
    }
    
    bool CheckTalkBoxAddAble(int UUID, string _time)
    {
        if (last_time != _time || last_uuid != UUID)
        {
            return false;
        }
        return true;
    }
}