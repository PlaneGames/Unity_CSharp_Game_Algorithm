using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public struct GameObjectNameTag
{
    public Color BG_col;
    public Color text_col;
}

[InitializeOnLoad]
public class HierarchyNameTagMgr : MonoBehaviour
{
    public Color BG_col   = new Color(0f, 0f, 0f, 0f);
    public Color text_col = new Color(1f, 0.729f, 0f, 1f);
    public GameObjectNameTag name_tag;

    public static Color BG_COL, TEXT_COL;

    private void Awake() 
    {
        BG_COL = BG_col;
        TEXT_COL = text_col;
    }
    
    private void Update()
    {
        BG_COL = BG_col;
        TEXT_COL = text_col;
    }
}
