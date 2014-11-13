using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ReferenceExplorer
{
	public class ToReferenceWindow
	{
		public List<ReferenceObject> referenceObjectList = new List<ReferenceObject> ();
		List<PerhapsReferenceObject> perhapsReferenceObjectList = new List<PerhapsReferenceObject> ();
	
		Vector2 current;

		public void OnSelectionChange ()
		{
			referenceObjectList.Clear ();
			SceneObjectUtility.Init ();

			SceneObjectUtility.GetReferenceObject (Selection.activeGameObject, referenceObjectList);
			UpdatePerahpsReferenceObjectList (Selection.activeGameObject);
		}

		void UpdatePerahpsReferenceObjectList (GameObject obj)
		{
			perhapsReferenceObjectList.Clear ();
			if (obj == null)
				return;

			// analytics  source code.

			foreach (var component in  obj.GetComponents<MonoBehaviour>()) {
				foreach (var text in MonoScript.FromMonoBehaviour(component).text.Split(';')) {
					Match m = Regex.Match (text, "GetComponent\\<(?<call>.*?)\\>");
					if (m.Success) {
						var methodName = m.Groups ["call"].ToString ();
						if( perhapsReferenceObjectList.Find((item) =>
						{
							return item.comp == component || item.typeName == methodName ;
						}) == null)
						{
							var method = new PerhapsReferenceObject ()
						{
							comp = component,
							typeName = methodName
						};
							perhapsReferenceObjectList.Add (method);
						}
					}
				}
			}
		}

		public void OnSceneGUI (SceneView sceneView)
		{
			var selection = Selection.activeGameObject as GameObject;
			if (selection == null)
				return;
		
		
			var cameraTransform = SceneView.currentDrawingSceneView.camera.transform;
			var rotate = cameraTransform.rotation;
			var cameraPos = cameraTransform.position;
		
			Color shadowCol = new Color (0.5f, 0, 0, 0.06f);
		
			foreach (var target in referenceObjectList) {
				var obj = SceneObjectUtility.GetGameObject (target.value);
				if (obj == null) {
					continue;
				}
				if (obj == Selection.activeGameObject) {
					continue;
				}

				if( PrefabUtility.GetPrefabType( obj ) == PrefabType.Prefab )
					continue;
			
			
				var startPosition = selection.transform.position;
				var endPosition = obj.transform.position;
			
				var size = Vector3.Distance (endPosition, cameraPos) * 0.02f;
			
				if (startPosition == endPosition)
					continue;
			
				Handles.color = Color.red;
			
				var diffPos = startPosition - endPosition;
				var tan = new Vector3 (diffPos.y, diffPos.x, diffPos.z);
			
			
				var startTan = startPosition;
				var endTan = endPosition + tan * 0.4f;
			
				Handles.CircleCap (1, endPosition, rotate, size);

				for (int i=0; i<3; i++)
					Handles.DrawBezier (startPosition, endPosition, startTan, endTan, shadowCol, null, (i + 1) * 5);
				Handles.DrawBezier (startPosition, endPosition, startTan, endTan, Color.red, null, 1);
			
				Handles.Label (endPosition, obj.name);
			}
		}
	
		public void OnGUI ()
		{
			GUIStyle styles = new GUIStyle ();
			styles.margin.left = 10;
			styles.margin.top = 5;
		
			int preGameObjectID = 0;

			List<Component> comps = new List<Component> ();
			foreach (var referenceObject in referenceObjectList) {
				if (! comps.Contains (referenceObject.rootComponent)) {
					comps.Add (referenceObject.rootComponent);
				}
			}

			try {
				EditorGUILayout.BeginVertical (GUILayout.Width (Screen.width * 0.5f - 10));
				EditorGUILayout.Space();


				foreach (var refObj in comps) {
					var components = referenceObjectList.FindAll ((item) => {
						return item.rootComponent == refObj;
					});
					EditorGUILayout.BeginVertical ("box");
					GUILayout.Label (components [0].rootComponent.GetType ().Name);

					foreach (var toComp in components) {
						string msg = string.Format ("{0} ->  ", toComp.memberName, toComp.value.GetType ().Name);

						EditorGUILayout.ObjectField (msg, (Object)toComp.value, toComp.value.GetType (), true);
					}

					foreach (var compName in perhapsReferenceObjectList) {
						bool isExist = components.Exists ((item) => {
							return item.rootComponent == compName.comp; });
						if (isExist == true)
							EditorGUILayout.LabelField (compName.typeName);
					}

					EditorGUILayout.EndVertical ();
				}

				EditorGUILayout.EndVertical();
			} catch {
				referenceObjectList.Clear ();
			}
		}

		public class PerhapsReferenceObject
		{
			public Component comp;
			public string typeName;
		}

	}

}
