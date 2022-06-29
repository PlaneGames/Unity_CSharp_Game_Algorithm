using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ChatTalkBlockMe : MonoBehaviour
{
    [SerializeField] Transform trans_balloon_cont;
    [SerializeField] ContentSizeFitter size_fitter;
    [SerializeField] TextMeshProUGUI text_time;
    [SerializeField] GameObject pref_balloon;
    [SerializeField] RectTransform rect_time;

    List<ChatTalkBalloon> talk_balloons;
    public string time { get; private set; }


    public void SetInfo(string _time, string _talk)
    {
        Debug.Log("SetInfo");
        if (talk_balloons == null)
        {
            talk_balloons = new List<ChatTalkBalloon>();
        }
        text_time.text = _time;
        time = _time;
        AddTalkBalloon(_talk);
    }

    public void AddTalkBalloon(string _talk)
    {
        var _obj = Instantiate(pref_balloon, trans_balloon_cont) as GameObject;
        var _balloon = _obj.GetComponent<ChatTalkBalloon>();
        _balloon.SetTalkMsg(_talk);
        talk_balloons.Add(_balloon);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)size_fitter.transform);
        RectTransform _rect = talk_balloons[talk_balloons.Count - 1].rect;
        rect_time.anchoredPosition = new Vector2(- _rect.sizeDelta.x + 41f, (- (_rect.sizeDelta.y + 5f) * talk_balloons.Count) + 7f);
    }
}
