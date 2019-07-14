using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TVariable), true)]
[CanEditMultipleObjects]
public class TVariableEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		var v = target as TVariable;
		var pi = v.GetType().GetProperty("CurrentValue", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
		if(pi != null)
		{
			var value = pi.GetValue(v);
			EditorGUILayout.LabelField(new GUIContent("Current Value"), new GUIContent(System.Convert.ToString(value)));
		}
	}
}