using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;

namespace ReferenceExplorer
{
	
	public class ObjectReferenceExplorer : EditorWindow {

		ToReferenceWindow toRefView = new ToReferenceWindow();
		FromObjectReferenceWindow fromRefView = new FromObjectReferenceWindow();

		Texture2D icon;

		  
		Vector2 scrollCurrent;


		[MenuItem("Window/Referenced/ReferenceViewer")]
		static void Init()
		{
			ObjectReferenceExplorer.GetWindow<ObjectReferenceExplorer>("Reference View");
 		}



		void OnSelectionChange () {
			toRefView.OnSelectionChange();
			fromRefView.OnSelectionChange();
			icon = EditorGUIUtility.Load("Icons/Generated/PrefabNormal Icon.asset") as Texture2D;
			Repaint ();
		}
		
		
		void OnGUI()
		{
			var iconSize = EditorGUIUtility.GetIconSize();
			EditorGUIUtility.SetIconSize(Vector2.one * 20);

			EditorGUILayout.BeginHorizontal("box", GUILayout.Width(Screen.width * 0.96f));

			GUIStyle style = new GUIStyle (){ richText = true, fontSize = 20 };
			GUILayout.Label(string.Format("<color=blue>{0} <size=10>components</size> -> </color> ", fromRefView.referenceObjectList.Count ),style, GUILayout.ExpandWidth(false));
			GUILayout.Label(icon, GUILayout.Width(20));
			var objName = Selection.activeGameObject.name;
			objName = objName.Replace("(", "<size=10>(").Replace(")", ")</size>");
			GUILayout.Label(objName, style,  GUILayout.ExpandWidth(false));
			GUILayout.Label(string.Format("<color=red> ->{0} <size=10>components</size></color> ", toRefView.referenceObjectList.Count),style);

			EditorGUIUtility.SetIconSize(iconSize);
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginScrollView(scrollCurrent);

			EditorGUILayout.BeginHorizontal();

			
			fromRefView.OnGUI();
			toRefView.OnGUI();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndScrollView();
		}

	}
}
