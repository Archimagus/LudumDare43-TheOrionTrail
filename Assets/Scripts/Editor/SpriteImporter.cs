using UnityEngine;
using UnityEditor;


class SpriteImporter : AssetPostprocessor
{
	void OnPreprocessTexture()
	{
		if (assetPath.Contains("Sprites"))
		{
			TextureImporter textureImporter = (TextureImporter)assetImporter;
			if (textureImporter.textureType != TextureImporterType.Sprite)
			{
				textureImporter.textureType = TextureImporterType.Sprite;
				textureImporter.spriteImportMode = SpriteImportMode.Single;
				textureImporter.alphaIsTransparency = true;
				textureImporter.wrapMode = TextureWrapMode.Clamp;
				textureImporter.anisoLevel = 1;
				textureImporter.mipmapEnabled = false;
			}
		}
	}
	void OnPreprocessModel()
	{
		var modelImporter = (ModelImporter)assetImporter;
		modelImporter.importAnimation = false;
		modelImporter.animationType = ModelImporterAnimationType.None;
	}

}