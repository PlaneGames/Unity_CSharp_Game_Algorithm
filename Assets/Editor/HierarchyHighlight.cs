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

        private void OnGUI() {
            EditorGUIUtility.SetIconSize(new Vector2(8, 8));
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
                    if (!_obj.activeSelf)
                    {
                        TextCol = new Color(1f, 1f, 1f, 0.7f);
                    }
                    
                    FontStyle TextStyle = FontStyle.Normal;
                    Rect Offset = new Rect(inSelectionRect.position + new Vector2(18f, 0f), new Vector2(14f, 0f));

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