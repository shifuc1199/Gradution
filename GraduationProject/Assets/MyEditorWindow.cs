#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;
 using System.Linq;
public class MyEditorWindow : OdinMenuEditorWindow
{
    public OdinMenuTree _tree;
    // Start is called before the first frame update
        [MenuItem("mywindow/test")]
        private static void Open()
        {
            var window = GetWindow<MyEditorWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
        }
          protected override OdinMenuTree BuildMenuTree()
        {
            _tree= new OdinMenuTree(true);
            _tree.DefaultMenuStyle.IconSize = 28.00f;
            _tree.Config.DrawSearchToolbar = true;

            if(WeaponConfig.Count != 0)
            {
            // Adds the character overview table.
            _tree.Add(WeaponConfig.Get(1).武器名字, WeaponConfig.Get(1));
            _tree.EnumerateTree().AddIcons<WeaponConfig>(x => x.武器图标);
            }
            return _tree;
        }

    // Update is called once per frame
    protected override void OnBeginDrawEditors()
        {
            var selected = this.MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;
            SirenixEditorGUI.BeginHorizontalToolbar(30,100);
            {
                if (SirenixEditorGUI.ToolbarButton(new GUIContent("     武器    ")))
                {
                    

                }

            }
            SirenixEditorGUI.EndHorizontalToolbar();
            // Draws a toolbar with the name of the currently selected menu item.
            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight,100);
            {
 
                if (SirenixEditorGUI.ToolbarButton(new GUIContent("创建新武器")))
                {
                    WeaponConfig w = new WeaponConfig();
                     w.武器ID = _tree.MenuItems.Count+1;
                     _tree.Add("New Weapon",w);
                     _tree.MenuItems[_tree.MenuItems.Count-1].Select();
                      
                }
 
            }
            SirenixEditorGUI.EndHorizontalToolbar();

            
            
        }
}
#endif
