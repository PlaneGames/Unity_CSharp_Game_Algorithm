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
        public static readonly Color DEFAULT_COLOR_HIERARCHY_SELECTED = new Color(0.243f, 0.4901f, 0.9058f, 1f);


        static HierarchyHighlightManager()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= HierarchyHighlight_OnGUI;
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyHighlight_OnGUI;
        }


        private static void HierarchyHighlight_OnGUI(int inSelectionID, Rect inSelectionRect)
        {
            GameObject _obj = EditorUtility.InstanceIDToObject(inSelectionID) as GameObject;

            if (_obj != null)
            {
                if(_obj.name.Contains("@") && Event.current.type == EventType.Repaint)
                {
                    bool ObjectIsSelected = Selection.instanceIDs.Contains(inSelectionID);

                    Color BKCol = HierarchyNameTagMgr.BG_COL;
                    Color TextCol = HierarchyNameTagMgr.TEXT_COL;

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