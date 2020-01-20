#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;
using System;
 using System.Linq;
using Sirenix.OdinInspector;

 
public class FaceEditorWindow : OdinMenuEditorWindow
{
    public static FaceEditorWindow _window;
    public bool isCreate = false;
    public FaceType config_type;
    public OdinMenuTree _tree;
    
    // Start is called before the first frame update
        [MenuItem("DreamerEditor/相貌编辑器")]
        private static void Open()
        {
            _window = GetWindow<FaceEditorWindow>();
        _window.titleContent = new GUIContent("相貌编辑器");
        _window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
        }
          protected override OdinMenuTree BuildMenuTree()
        {
            _tree= new OdinMenuTree(true);
            _tree.DefaultMenuStyle.IconSize = 28.00f;
            _tree.Config.DrawSearchToolbar = true;
            switch (config_type)
            {
                case FaceType.眼睛:
                    break;
                case FaceType.嘴巴:
                    break;
                case FaceType.发型:
                    break;
                case FaceType.胡子:
                    break;
                default:
                    break;
            }
        return _tree;
        }
   
    public void CreateItem<T>(T i,ref OdinMenuTree t) where T:FaceConfig<T>
    {
        i.ID = _tree.MenuItems.Count + 1;
        _tree.Add("New "+typeof(T).Name, i);
        _tree.MenuItems[_tree.MenuItems.Count - 1].Select();
    }
  
    // Update is called once per frame
    protected override void OnBeginDrawEditors()
        {
            var selected = this.MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;
            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                foreach (var temp in Enum.GetNames(typeof(FaceType)))
                {
                    if (SirenixEditorGUI.ToolbarButton(new GUIContent("     "+temp+"    " )))
                    {
                        config_type = (FaceType)Enum.Parse(typeof(FaceType),temp);
                        ForceMenuTreeRebuild();
                    }
                }
                SirenixEditorGUI.ToolbarTab(false,"");
                if (SirenixEditorGUI.ToolbarButton(new GUIContent("    +   ")) && !isCreate)
                {
                    isCreate=true;
                switch (config_type)
                {
                    case FaceType.眼睛:
                        break;
                    case FaceType.嘴巴:
                        break;
                    case FaceType.发型:
                        break;
                    case FaceType.胡子:
                        break;
                    default:
                        break;
                }
            }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }
}
#endif