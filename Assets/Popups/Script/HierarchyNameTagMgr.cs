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
    public GUISkin gui_skin;
    public GameObjectNameTag[] name_tag;
    
    public static GUISkin GUI_SKIN;
    public static List<GameObjectNameTag> Name_Tag;


    private void Awake() 
    {
        GUI_SKIN = gui_skin;
        SetNameTag();
    }
    
    private void Update()
    {
        GUI_SKIN = gui_skin;
        SetNameTag();
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
