using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class SpriteProcessor : AssetPostprocessor
{
     void OnPreprocessTexture()
    {
        
       /* if(assetImporter.assetPath.Contains(".png"))
        {
            TextureImporter textureImporter = assetImporter as TextureImporter;
            textureImporter.textureType = TextureImporterType.Sprite;
        }*/
    }
}
