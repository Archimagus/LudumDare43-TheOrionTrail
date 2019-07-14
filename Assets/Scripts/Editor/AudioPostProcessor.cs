using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class AudioPostProcessor : AssetPostprocessor
{
	void OnPostprocessAudio(AudioClip g)
	{
		Apply();
	}
	[MenuItem("Tools/Fix AudioClips")]
	private static void Apply()
	{
		string[] guids2 = AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets/Audio/Resources" });

		var sb = new StringBuilder();
		sb.AppendLine("// Auto Generated File, Don't Modify");
		sb.AppendLine("public enum AudioClips");
		sb.AppendLine("{");

		foreach (string guid2 in guids2)
		{
			var path = AssetDatabase.GUIDToAssetPath(guid2);
			var name = Path.GetFileNameWithoutExtension(path);
			sb.AppendLine($"\t{name},");
		}
		sb.Remove(sb.Length - 3, 1);
		sb.AppendLine("}");
		var dataPath = Application.dataPath.Replace('/','\\');
		dataPath = Path.Combine(dataPath, @"Scripts\Utilities\AudioClips.cs");
		File.WriteAllText(dataPath, sb.ToString());
		AssetDatabase.ImportAsset(@"Assets/Scripts/Utilities/AudioClips.cs");
	}
}
