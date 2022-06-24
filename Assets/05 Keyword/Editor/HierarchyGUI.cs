using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;

/*
Developer : Jae Young Kwon
Version : 22.05.30
*/

namespace Assets.Editor
{ 
    [InitializeOnLoad]
    public class HierarchyGUI
    {
        public static GUISkin testGUIskin;
        public static Texture2D texture;
        static Texture2D icon_menu;
        static int minSelectionID;
        static Rect minSelectionRect;
        static Color toggle_color;

        private const string EDITOR_WINDOW_TYPE = "UnityEditor.SceneHierarchyWindow";
        private const string EDITOR_PROJECT_TYPE = "UnityEditor.ProjectWindow";
        private const double EDITOR_WINDOWS_CACHE_TTL = 2;

        private const BindingFlags INSTANCE_PRIVATE = BindingFlags.Instance | BindingFlags.NonPublic;
        private const BindingFlags INSTANCE_PUBLIC = BindingFlags.Instance | BindingFlags.Public;

        private static readonly FieldInfo SCENE_HIERARCHY_FIELD;
        private static readonly FieldInfo TREE_VIEW_FIELD;
        private static readonly PropertyInfo TREE_VIEW_DATA_PROPERTY;
        private static readonly MethodInfo TREE_VIEW_ITEMS_METHOD;
        private static readonly PropertyInfo TREE_VIEW_OBJECT_PROPERTY;

        // Windows cache
        private static double _nextWindowsUpdate;
        private static EditorWindow[] _windowsCache;

        static HierarchyGUI()
        {
            var assembly = Assembly.GetAssembly(typeof(EditorWindow));

            var hierarchyWindowType = assembly.GetType("UnityEditor.SceneHierarchyWindow");
            SCENE_HIERARCHY_FIELD = hierarchyWindowType.GetField("m_SceneHierarchy", INSTANCE_PRIVATE);

            var sceneHierarchyType = assembly.GetType("UnityEditor.SceneHierarchy");
            TREE_VIEW_FIELD = sceneHierarchyType.GetField("m_TreeView", INSTANCE_PRIVATE);

            var treeViewType = assembly.GetType("UnityEditor.IMGUI.Controls.TreeViewController");
            TREE_VIEW_DATA_PROPERTY = treeViewType.GetProperty("data", INSTANCE_PUBLIC);

            var treeViewDataType = assembly.GetType("UnityEditor.GameObjectTreeViewDataSource");
            TREE_VIEW_ITEMS_METHOD = treeViewDataType.GetMethod("GetRows", INSTANCE_PUBLIC);

            var treeViewItem = assembly.GetType("UnityEditor.GameObjectTreeViewItem");
            TREE_VIEW_OBJECT_PROPERTY = treeViewItem.GetProperty("objectPPTR", INSTANCE_PUBLIC);

            texture = AssetDatabase.LoadAssetAtPath ("Assets/01 Arts/EditorGUI/FolderIcons/folder_normal.png", typeof(Texture2D)) as Texture2D;
            icon_menu = AssetDatabase.LoadAssetAtPath ("Assets/menu_Icon.png", typeof(Texture2D)) as Texture2D;
            EditorApplication.hierarchyWindowItemOnGUI -= HierarchyHighlight_OnGUI;
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyHighlight_OnGUI;
        }

        private static IEnumerable<TreeViewItem> GetTreeViewItems(EditorWindow window)
        {
            var sceneHierarchy = SCENE_HIERARCHY_FIELD.GetValue(window);
            var treeView = TREE_VIEW_FIELD.GetValue(sceneHierarchy);
            var treeViewData = TREE_VIEW_DATA_PROPERTY.GetValue(treeView, null);
            var treeViewItems = (IEnumerable<TreeViewItem>) TREE_VIEW_ITEMS_METHOD.Invoke(treeViewData, null);

            return treeViewItems;
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "InvertIf")]
        public static IEnumerable<EditorWindow> GetAllHierarchyWindows(bool forceUpdate = false)
        {
            if (forceUpdate || _nextWindowsUpdate < EditorApplication.timeSinceStartup)
            {
                _nextWindowsUpdate = EditorApplication.timeSinceStartup + EDITOR_WINDOWS_CACHE_TTL;
                _windowsCache = GetAllWindowsByType(EDITOR_WINDOW_TYPE).ToArray();
            }
            return _windowsCache;
        }

        public static IEnumerable<EditorWindow> GetAllWindowsByType(string type)
        {
            var objectList = Resources.FindObjectsOfTypeAll(typeof(EditorWindow));
            var windows = from obj in objectList where obj.GetType().ToString() == type select (EditorWindow) obj;
            return windows;
        }

        private static string GetAnchorText(Vector2 _min, Vector2 _max)
        {
            var _str_w = "";
            var _str_h = "";
            var _str = "";
            
            if (_min.x == 0f)
            {
                if (_max.x == 0f)
                    _str_w = "Left";
                else if (_max.x == 1f)
                    _str_w = "Stretch";
                else
                    _str_w = "Custom";
            }
            else if (_min.x == 0.5f)
            {
                _str_w = "Center";
            }
            else if (_min.x == 1f)
            {
                _str_w = "Right";
            }
            else
            {
                _str_w = "Custom";
            }
            
            if (_min.y == 0f)
            {
                if (_max.y == 0f)
                    _str_h = "Bottom";
                else if (_max.y == 1f)
                    _str_h = "Stretch";
                else
                    _str_h = "Custom";
            }
            else if (_min.y == 0.5f)
            {
                _str_h = "Middle";
            }
            else if (_min.y == 1f)
            {
                _str_h = "Top";
            }
            else
            {
                _str_w = "Custom";
            }

            if (_str_w.Equals(_str_h))
                return _str_w;

            _str = _str_w + " " + _str_h;

            return _str;
        }

        private static void ChangeIcon(GameObject _obj, Texture2D _texture)
        {
            var hierarchyWindows = GetAllHierarchyWindows(true);
            foreach (var window in hierarchyWindows)
            {
                var treeViewItems = GetTreeViewItems(window);
                foreach (var item in treeViewItems)
                {
                    if (_obj == TREE_VIEW_OBJECT_PROPERTY.GetValue(item, null) as GameObject)
                    {
                        item.icon = _texture;
                    }
                }
            }
            EditorApplication.RepaintHierarchyWindow();
        }

        private static void SetChangeIcon(GameObject _obj)
        {
            if (_obj.name.Contains("--"))
            {
                ChangeIcon(_obj, texture);
            }
        }

        private static void SetNameTag(GameObject _obj, Rect _rect_selection)
        {
            if (HierarchyNameTagMgr.Name_Tag != null)
            {
                bool _able = false;

                Color BKCol = new Color(1f, 1f, 1f, 0.7f);
                Color TextCol = new Color(1f, 1f, 1f, 0.7f);

                for (int i = 0; i < HierarchyNameTagMgr.Name_Tag.Count; i ++)
                {
                    if (_obj.name.Contains(HierarchyNameTagMgr.Name_Tag[i].name))
                    {
                        BKCol = HierarchyNameTagMgr.Name_Tag[i].BG_col;
                        TextCol = HierarchyNameTagMgr.Name_Tag[i].text_col;
                        _able = true;
                        break;
                    }
                }

                if (_able && _obj.name.Contains("@") && Event.current.type == EventType.Repaint)
                {
                    if (!_obj.activeSelf)
                    {
                        TextCol = new Color(1f, 1f, 1f, 0.7f);
                    }

                    FontStyle TextStyle = FontStyle.Normal;
                    Rect Offset = new Rect(_rect_selection.position + new Vector2(18f, 0f), new Vector2(14f, 0f));

                    EditorGUI.LabelField(Offset, "@", new GUIStyle()
                    {
                        normal = new GUIStyleState() { textColor = TextCol },
                        fontStyle = TextStyle
                    });
                    
                    EditorApplication.RepaintHierarchyWindow();
                }
            }
        }

        private static void SetObjectMenu(GameObject _obj, int _id_selection, Rect _rect_selection)
        {
            Rect _rect_menu = new Rect(0f, _rect_selection.y, GUILayoutUtility.GetLastRect().width, 16f);
            Rect _rect_menu_icon = new Rect(32f, _rect_selection.y - 1f, 18f, 18f);

            bool _selected =
                _rect_menu.Contains(Event.current.mousePosition);

            bool _hover_toggle = _rect_menu_icon.Contains(Event.current.mousePosition);

            if (_selected) 
            {
                bool _is_actived = _obj.activeSelf;
                Color _prev_col = GUI.color;
                if (_hover_toggle)
                    toggle_color = Color.Lerp(toggle_color, new Color(1f, 1f, 1f, 1f), 0.1f);
                else
                    toggle_color = Color.Lerp(toggle_color, new Color(1f, 1f, 1f, 0.5f), 0.1f);
                GUI.color = toggle_color;
                bool _btn = GUI.Toggle(_rect_menu_icon, _is_actived, "");
                GUI.color = _prev_col;

                _obj.SetActive(_btn);
            }
        }

        private static void SetAnchorTag(GameObject _obj, Rect _rect)
        {
            if (_obj.name.Contains("@ Popup") || _obj.name.Contains("@ UIE"))
            {
                GUIStyle _style = new GUIStyle();
                _style.alignment = TextAnchor.MiddleRight;
                _style.normal.textColor = new Color(1f, 1f, 1f, 0.4f);
                Rect rect = _rect;
                rect.width = _rect.width;

                RectTransform _rt = _obj.GetComponent<RectTransform>();

                GUI.Label(rect, GetAnchorText(_rt.anchorMin, _rt.anchorMax), _style);
            }
        }

        static void AddMenuItem(GenericMenu menu, string menuPath, GenericMenu.MenuFunction func)
        {
            menu.AddItem(new GUIContent(menuPath), false, func);
        }

        private static void HierarchyHighlight_OnGUI(int _id_selection, Rect _rect_selection)
        {
            GameObject _obj = EditorUtility.InstanceIDToObject(_id_selection) as GameObject;

            if (_obj != null)
            {
                SetObjectMenu(_obj, _id_selection, _rect_selection);
                SetChangeIcon(_obj);
                SetNameTag(_obj, _rect_selection);
                SetAnchorTag(_obj, _rect_selection);
            }
        }
    }
}