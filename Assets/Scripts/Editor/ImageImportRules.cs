using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ImageImportRules : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if (assetPath.Contains("UI"))
        {
            TextureImporter textureImporter = (TextureImporter)assetImporter;
            textureImporter.textureType = TextureImporterType.Sprite;
        }
    }
}