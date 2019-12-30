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

 
public class MyEditorWindow : OdinMenuEditorWindow
{
    public static MyEditorWindow _window;
    public bool isCreate = false;
    public ItemType config_type;
    public OdinMenuTree _tree;
    // Start is called before the first frame update
        [MenuItem("mywindow/test")]
        private static void Open()
        {
            _window = GetWindow<MyEditorWindow>();
            _window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
        }
          protected override OdinMenuTree BuildMenuTree()
        {
            _tree= new OdinMenuTree(true);
            _tree.DefaultMenuStyle.IconSize = 28.00f;
            _tree.Config.DrawSearchToolbar = true;
            switch(config_type)
            {
                case ItemType.武器:
                    WeaponConfig.Reload();
                    if(WeaponConfig.Count>0)
                    {
                        
                    foreach(var item in WeaponConfig.Datas)
                    {
                    // Adds the character overview table.
                    _tree.Add(item.Value.武器名字,item.Value);
                        _tree.EnumerateTree().AddIcons<WeaponConfig>(x => x.GetSprite());
                    _tree.EnumerateTree().ForEach(AddDragHandles);
                    }
                    }
                    break;
          
            }        
            return _tree;
        }
    public void AddDragHandles(OdinMenuItem item)
    {
        item.OnDrawItem += (t) => { (t.Value as WeaponConfig).SetEditorSprite(); };
    }
    // Update is called once per frame
    protected override void OnBeginDrawEditors()
        {
            var selected = this.MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;
 
 
            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
 
            foreach (var temp in Enum.GetNames(typeof(ItemType)))
            {
                if (SirenixEditorGUI.ToolbarButton(new GUIContent("     "+temp+"    " )))
                {
                    config_type = (ItemType)Enum.Parse(typeof(ItemType),temp);
                    ForceMenuTreeRebuild();
                    
                }

            }
  
                 SirenixEditorGUI.ToolbarTab(false,"");
                if (SirenixEditorGUI.ToolbarButton(new GUIContent("    +   ")) && !isCreate)
                {
                    isCreate=true;
                     WeaponConfig w = new WeaponConfig();
                     w.武器ID = _tree.MenuItems.Count+1;
                     _tree.Add("New Weapon",w);
                     _tree.MenuItems[_tree.MenuItems.Count-1].Select();
                      
                }

            }
            
            SirenixEditorGUI.EndHorizontalToolbar();

            
            
        }
}
