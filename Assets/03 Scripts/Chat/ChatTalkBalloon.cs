using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ChatTalkBalloon : MonoBehaviour
{
    [SerializeField] public RectTransform rect;
    [SerializeField] TextMeshProUGUI text_talk;
    [SerializeField] ContentSizeFitter size_fitter;

    public void SetTalkMsg(string _talk)
    {
        text_talk.text = _talk;
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)size_fitter.transform);
    }
}
