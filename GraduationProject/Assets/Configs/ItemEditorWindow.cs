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

 
public class ItemEditorWindow : OdinMenuEditorWindow
{
    public static ItemEditorWindow _window;
    public bool isCreate = false;
    public ItemType config_type;
    public OdinMenuTree _tree;
    
    // Start is called before the first frame update
        [MenuItem("DreamerEditor/物品编辑器")]
        private static void Open()
        {
            _window = GetWindow<ItemEditorWindow>();
        _window.titleContent = new GUIContent("物品编辑器");
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
                    _tree.Add(item.Value.物品名字,item.Value);
                     _tree.EnumerateTree().AddIcons<WeaponConfig>(x => x.GetSprite());
                    _tree.EnumerateTree().ForEach(AddDragHandles<WeaponConfig>);
                    }
                    }
                    break;
            case ItemType.腿部:
                FootConfig.Reload();
                if (FootConfig.Count > 0)
                {

                    foreach (var item in FootConfig.Datas)
                    {
                        _tree.Add(item.Value.物品名字, item.Value);
                        _tree.EnumerateTree().AddIcons<FootConfig>(x => x.GetSprite());
                        _tree.EnumerateTree().ForEach(AddDragHandles<FootConfig>);
                    }
                }
                break;
          
            }        
            return _tree;
        }
    public void AddDragHandles<T>(OdinMenuItem item) where T:ItemConfig<T>
    {
        item.OnDrawItem += (t) => { (t.Value as T).SetEditorSprite(); };
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
                switch (config_type)
                {
                    case ItemType.腿部:
                        FootConfig f = new FootConfig();
                        f.物品ID = _tree.MenuItems.Count + 1;
                        _tree.Add("New Foot", f);
                        _tree.MenuItems[_tree.MenuItems.Count - 1].Select();
                        break;
                    case ItemType.裤子:
                        break;
                    case ItemType.肩膀:
                        break;
                    case ItemType.手腕:
                        break;
                    case ItemType.武器:
                        WeaponConfig w = new WeaponConfig();
                        w.物品ID = _tree.MenuItems.Count + 1;
                        _tree.Add("New Weapon", w);
                        _tree.MenuItems[_tree.MenuItems.Count - 1].Select();
                        break;
                    case ItemType.上衣:
                        break;
                    case ItemType.消耗品:
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