#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class SpriteProcessor : AssetPostprocessor
{
   /*  void OnPreprocessTexture()
    {
        
        if(assetImporter.assetPath.Contains(".png"))
        {
            TextureImporter textureImporter = assetImporter as TextureImporter;
            textureImporter.spritePivot = new Vector2(0.5f, 0.6f);
        }
    }*/
}
#endif
