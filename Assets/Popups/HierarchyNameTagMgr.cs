using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public struct GameObjectNameTag
{
    public string name;
    public Color BG_col;
    public Color text_col;
}

[InitializeOnLoad]
public class HierarchyNameTagMgr : MonoBehaviour
{
    [Header ("< Name Tag Setting >")]
    public Color BG_col   = new Color(0f, 0f, 0f, 0f);
    public Color text_col = new Color(1f, 0.729f, 0f, 1f);
    public GameObjectNameTag[] name_tag;

    public static List<GameObjectNameTag> Name_Tag;
    public static Color BG_COL, TEXT_COL;

    private void Awake() 
    {
        SetNameTag();
        BG_COL = BG_col;
        TEXT_COL = text_col;
    }
    
    private void Update()
    {
        SetNameTag();
        BG_COL = BG_col;
        TEXT_COL = text_col;
    }

    private void SetNameTag()
    {
        if (Name_Tag == null)
        {
            Name_Tag = new List<GameObjectNameTag>();
            for (int i = 0; i < name_tag.Length; i ++)
            {
                if (!Name_Tag.Contains(name_tag[i]))
                {
                    Name_Tag.Add(name_tag[i]);
                }
            }
        }
    }
}
