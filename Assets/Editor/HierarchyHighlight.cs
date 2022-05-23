using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace Assets.Editor
{
    [InitializeOnLoad]
    public class HierarchyHighlightManager
    {
        static Texture2D texture;
        static Texture2D texture2;

        static HierarchyHighlightManager()
        {
            texture = AssetDatabase.LoadAssetAtPath ("Assets/folder.png", typeof(Texture2D)) as Texture2D;
            texture2 = AssetDatabase.LoadAssetAtPath ("Assets/free_icon.png", typeof(Texture2D)) as Texture2D;
            EditorApplication.hierarchyWindowItemOnGUI -= HierarchyHighlight_OnGUI;
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyHighlight_OnGUI;
        }


        private static void HierarchyHighlight_OnGUI(int inSelectionID, Rect inSelectionRect)
        {
            GameObject _obj = EditorUtility.InstanceIDToObject(inSelectionID) as GameObject;

            if (_obj != null && HierarchyNameTagMgr.Name_Tag != null)
            {
                Color BKCol = new Color(1f, 1f, 1f, 0.7f);
                Color TextCol = new Color(1f, 1f, 1f, 0.7f);

                for (int i = 0; i < HierarchyNameTagMgr.Name_Tag.Count; i ++)
                {
                    if (_obj.name.Contains(HierarchyNameTagMgr.Name_Tag[i].name))
                    {
                        BKCol = HierarchyNameTagMgr.Name_Tag[i].BG_col;
                        TextCol = HierarchyNameTagMgr.Name_Tag[i].text_col;
                        break;
                    }
                }

                if (_obj.name.Contains("@") && Event.current.type == EventType.Repaint)
                {
                    bool ObjectIsSelected = Selection.instanceIDs.Contains(inSelectionID);

                    Rect _selection_rect = new Rect (inSelectionRect);
                    _selection_rect.x = 0;
                    _selection_rect.y = inSelectionRect.position.y;
                    _selection_rect.width = GUILayoutUtility.GetLastRect().width;

                    if (_selection_rect.Contains(Event.current.mousePosition))
                    {
                        Debug.Log(GUI.contentColor);
                    }
                    else
                    {

                    }

                    Rect _panel = new Rect (inSelectionRect);
                    _panel.x = 60;
                    _panel.y = inSelectionRect.position.y;
                    _panel.width = 16;
                    _panel.height = 16;
                    
                    Color _prev_c = GUI.color;
                    Color _cur_c = Color.white - _prev_c + new Color(0f, 0f, 0f, 1f);
                    EditorGUI.DrawRect(_panel, _cur_c);
                    
                    if (ObjectIsSelected)
                        _cur_c = GUI.skin.settings.selectionColor;
                    else
                        _cur_c = Color.white - _prev_c + new Color(0f, 0f, 0f, 1f);

                    EditorGUI.DrawRect(_panel, _cur_c);


                    Rect r = new Rect (inSelectionRect); 
                    r.x = 58;
                    r.y = inSelectionRect.position.y - 2.5f;
                    r.width = 20;
                    r.height = 20;

                    GUI.color = _prev_c;
                    GUI.Label (r, texture); 
                    
                    
                    if (!_obj.activeSelf)
                    {
                        TextCol = new Color(1f, 1f, 1f, 0.7f);
                    }
                    
                    FontStyle TextStyle = FontStyle.Normal;

                    Rect Offset = new Rect(inSelectionRect.position + new Vector2(18f, 0f), new Vector2(14f, 0f));

                    if (BKCol.a > 0f)
                    {
                        Rect BackgroundOffset = new Rect(inSelectionRect.position + new Vector2(18f, 0f), inSelectionRect.size);
                        EditorGUI.DrawRect(BackgroundOffset, new Color(0.76f, 0.76f, 0.76f, 1f));

                        if (ObjectIsSelected)
                            EditorGUI.DrawRect(BackgroundOffset, Color.Lerp(GUI.skin.settings.selectionColor, BKCol, 0.3f));
                        else
                            EditorGUI.DrawRect(BackgroundOffset, BKCol);
                    }

                    EditorGUI.LabelField(Offset, "@", new GUIStyle()
                    {
                        normal = new GUIStyleState() { textColor = TextCol },
                        fontStyle = TextStyle
                    });

                    EditorApplication.RepaintHierarchyWindow();
                }
            }
        }
    }
}