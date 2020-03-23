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

 
public class SuitEditorWindow : OdinMenuEditorWindow
{
    
    public static SuitEditorWindow _window;
    public OdinMenuTree _tree;
    public bool isCreate;
    // Start is called before the first frame update
        [MenuItem("DreamerEditor/套装编辑器")]
        private static void Open()
        {
        
            _window = GetWindow<SuitEditorWindow>();
        _window.titleContent = new GUIContent("套装编辑器");
            _window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
        }
          protected override OdinMenuTree BuildMenuTree()
        {
            _tree= new OdinMenuTree(true);
            _tree.DefaultMenuStyle.IconSize = 28.00f;
            _tree.Config.DrawSearchToolbar = true;
            SuitConfig.Reload();
            if (SuitConfig.Count > 0)
            {

                foreach (var item in SuitConfig.Datas)
                {
                    _tree.Add(item.Value.suit_name, item.Value);
 
                }
            }
        return _tree;
        }
 
    // Update is called once per frame
    protected override void OnBeginDrawEditors()
        {
            var selected = this.MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;
 
 
            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
        {
            SirenixEditorGUI.ToolbarTab(false, new GUIContent(""));
            if (SirenixEditorGUI.ToolbarButton(new GUIContent("    +   ")) && !isCreate)
            {
                isCreate = true;
                SuitConfig w = new SuitConfig();
                w.ID = _tree.MenuItems.Count + 1;
                _tree.Add("New Suit", w);
                _tree.MenuItems[_tree.MenuItems.Count - 1].Select();

            }


        }
            
            SirenixEditorGUI.EndHorizontalToolbar();

            
            
        }
}
#endif