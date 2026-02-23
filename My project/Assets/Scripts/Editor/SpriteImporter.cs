using UnityEditor;
using UnityEngine;

/// <summary>
/// Automatically sets correct import settings for all sprites in Assets/Art/.
/// Point filter, PPU 64, no compression, single sprite mode.
/// </summary>
public class SpriteImporter : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith("Assets/Art/") || assetPath.Contains("_SourceAssets"))
            return;

        TextureImporter importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 64;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.maxTextureSize = 256;
    }
}
