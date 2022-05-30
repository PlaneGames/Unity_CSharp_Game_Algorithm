using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.IO;

/*
Developer : Jae Young Kwon
Version : 22.05.30
*/

[InitializeOnLoad]
public static class ProjectBrowserGUI
{

	struct FolderStyle
	{
		public Color color;
		public Texture2D tag_icon;
		public Texture2D tag_icon_L;
	}


	private const BindingFlags INSTANCE_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
	private const BindingFlags STATIC_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

	public static Texture2D texture_folder_normal;
	public static Texture2D texture_folder_normal_L;
	public static Texture2D texture_folder_opened;

	static Dictionary<string, FolderStyle> folders_styles;
	static List<string> data_type_filter;

	static ProjectBrowserGUI()
	{
		var assembly = Assembly.GetAssembly(typeof(EditorWindow));

		texture_folder_normal = AssetDatabase.LoadAssetAtPath ("Assets/01 Arts/EditorGUI/FolderIcons/folder_normal.png", typeof(Texture2D)) as Texture2D;
		texture_folder_normal_L = AssetDatabase.LoadAssetAtPath ("Assets/01 Arts/EditorGUI/FolderIcons/folder_normal_L.png", typeof(Texture2D)) as Texture2D;
		texture_folder_opened = AssetDatabase.LoadAssetAtPath ("Assets/01 Arts/EditorGUI/FolderIcons/folder_opened.png", typeof(Texture2D)) as Texture2D;

		EditorApplication.projectWindowItemOnGUI -= ChangeIcon;
        EditorApplication.projectWindowItemOnGUI += ChangeIcon;

		data_type_filter = new List<string>();
		folders_styles = new Dictionary<string, FolderStyle>();

		InitFolderStyle( "01 Arts", SetHexToColor( "#EB645F" ) );
		InitFolderStyle( "02 Prefabs", SetHexToColor( "#6CC1FD" ), "folder_tag_object.png", "folder_tag_object_L.png" );
		InitFolderStyle( "03 Scripts", SetHexToColor( "#338A36" ), "folder_tag_script.png", "folder_tag_script_L.png");
		InitFolderStyle( "04 Scenes", SetHexToColor( "#565656" ), "folder_tag_scene.png", "folder_tag_scene_L.png");
		InitFolderStyle( "05 Keyword", SetHexToColor( "#A26AE2" ) );
		InitFolderStyle( "06 ToolKit", SetHexToColor( "#828282" ) );

		InitFolderStyle( "Resources", SetHexToColor( "#CC925E" ) );
		InitFolderStyle( "Editor", SetHexToColor( "#36AE7C" ) );
		InitFolderStyle( "Plugins", SetHexToColor( "#5A89A6" ) );
		
		InitFolderDataType
		( 
			".png", ".cs", ".asset", ".prefab", ".unity", 
			".txt", ".xml", ".md", ".csv"
		);
	}

	static Color SetHexToColor( string _hex )
	{
		Color color;
		ColorUtility.TryParseHtmlString(_hex, out color);
		return color;
	}
	
	static void InitFolderStyle(string _name, Color _color)
	{
		InitFolderStyle(_name, _color, (Texture2D)null, (Texture2D)null);
	}

	static void InitFolderStyle(string _name, Color _color, Texture2D _tag_icon, Texture2D _tag_icon_L)
	{
		FolderStyle _style;
		_style.color = _color;
		_style.tag_icon = _tag_icon;
		_style.tag_icon_L = _tag_icon_L;
		folders_styles.Add(_name, _style);
	}

	static void InitFolderStyle(string _name, Color _color, string _tag_icon_name, string _tag_icon_L_name)
	{
		FolderStyle _style;
		_style.color = _color;
		_style.tag_icon = AssetDatabase.LoadAssetAtPath ("Assets/01 Arts/EditorGUI/FolderIcons/" + _tag_icon_name, typeof(Texture2D)) as Texture2D;
		_style.tag_icon_L = AssetDatabase.LoadAssetAtPath ("Assets/01 Arts/EditorGUI/FolderIcons/" + _tag_icon_L_name, typeof(Texture2D)) as Texture2D;
		folders_styles.Add(_name, _style);
	}

	static void InitFolderDataType(params string[] _data_type)
	{
		int i;
		for (i = 0; i < _data_type.Length; i ++)
		{
			if (!data_type_filter.Contains(_data_type[i]))
			{
				data_type_filter.Add(_data_type[i]);
			}
		}
	}
	
	static long GetFileSize(string assetPath)
	{
		string fullAssetPath =
			string.Concat(Application.dataPath.Substring(0, Application.dataPath.Length - 7), "/", assetPath);
		long size = new FileInfo(fullAssetPath).Length;
		return size;
	}

	static string GetFileDataType(string assetPath)
	{
		string fullAssetPath =
			string.Concat(Application.dataPath.Substring(0, Application.dataPath.Length - 7), "/", assetPath);
		string type = Path.GetExtension(fullAssetPath).ToLower();
		return type;
	}

	static bool IsFilteredDataType(string _data_type)
	{
		return data_type_filter.Contains(_data_type);
	}

	static string GetFileSizeLabel(string assetPath)
	{
		return EditorUtility.FormatBytes(GetFileSize(assetPath));
	}
 
	private static void ChangeIcon(string guid, Rect rect)
	{
		EditorWindow projectWindow = typeof( EditorWindow ).Assembly.GetType( "UnityEditor.ProjectBrowser" ).GetField( "s_LastInteractedProjectBrowser", STATIC_FLAGS ).GetValue( null ) as EditorWindow;
		object folderTree = projectWindow.GetType().GetField( "m_FolderTree", INSTANCE_FLAGS ).GetValue( projectWindow );
		if( folderTree != null )
		{
			object treeViewDataSource = folderTree.GetType().GetProperty( "data", INSTANCE_FLAGS ).GetValue( folderTree, null );
			
			string _root = AssetDatabase.GUIDToAssetPath(guid);
			Object _obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(_root);

			if ( _obj != null)
			{
				if ( folders_styles.ContainsKey(_obj.name) && IsFolder(_root) && IsEmpty(_root) )
				{
					Rect _rect;
					bool isSearchFilterRootExpanded = (bool) treeViewDataSource.GetType().GetMethod( "IsExpanded", INSTANCE_FLAGS, null, new System.Type[] { typeof( int ) }, null ).Invoke( treeViewDataSource, new object[] { _obj.GetInstanceID() } );
					Color _col_prev = GUI.color;

					if (rect.height > 20)
					{
						_rect = new Rect(rect.x, rect.y, rect.width, rect.width);
						GUI.color = folders_styles[_obj.name].color;
						GUI.DrawTexture(_rect, texture_folder_normal_L);
						if (folders_styles[_obj.name].tag_icon != null)
						{
							GUI.color = _col_prev;
							GUI.DrawTexture(_rect, folders_styles[_obj.name].tag_icon_L);
						}
					}
					else if (rect.x > 15)
					{
						_rect = new Rect(rect.x, rect.y, rect.height, rect.height);
						GUI.color = folders_styles[_obj.name].color;
						if (isSearchFilterRootExpanded)
							GUI.DrawTexture(_rect, texture_folder_opened);
						else
							GUI.DrawTexture(_rect, texture_folder_normal);
						if (folders_styles[_obj.name].tag_icon != null)
						{
							GUI.color = _col_prev;
							GUI.DrawTexture(_rect, folders_styles[_obj.name].tag_icon);
						}
					}
					else
					{ 
						_rect = new Rect(rect.x + 3f, rect.y, rect.height, rect.height);
						GUI.color = folders_styles[_obj.name].color;
						GUI.DrawTexture(_rect, texture_folder_normal);
						if (folders_styles[_obj.name].tag_icon != null)
						{
							GUI.color = _col_prev;
							GUI.DrawTexture(_rect, folders_styles[_obj.name].tag_icon);
						}
					}
					GUI.color = _col_prev;
				}
				else if (!IsFolder(_root) && rect.height <= 20)
				{
					GUIStyle _style = new GUIStyle();
					_style.alignment = TextAnchor.MiddleRight;
					_style.normal.textColor = new Color(1f, 1f, 1f, 0.4f);
					Rect _rect = rect;
					_rect.width = rect.width - 16f;
					GUI.Label(_rect, GetFileSizeLabel(_root).ToString(), _style);

					var _data_type = GetFileDataType(_root);
					if (IsFilteredDataType(_data_type))
					{
						_rect.width = rect.width - 80f;
						GUI.Label(_rect, _data_type, _style);
					}
				}
			}
		}
		projectWindow.Repaint();
	}

	static bool IsFolder(string _root)
	{
		return (AssetDatabase.IsValidFolder(_root));
	}

	static bool IsEmpty(string _root)
	{
		return (System.IO.Directory.GetFiles(_root).Length > 0);
	}
}