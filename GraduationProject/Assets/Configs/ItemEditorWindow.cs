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
      
        _tree = new OdinMenuTree(true);
            _tree.DefaultMenuStyle.IconSize = 28.00f;
            _tree.Config.DrawSearchToolbar = true;
            switch(config_type)
            {
             case ItemType.武器:
                LoadItem<WeaponConfig>(ref _tree);
                break;
            case ItemType.上衣:
                LoadItem<TorsoConfig>(ref _tree);
                break;
            case ItemType.手链:
                LoadItem<SleeveConfig>(ref _tree);
                break;
            case ItemType.肩膀:
                LoadItem<ArmConfig>(ref _tree);
                break;
            case ItemType.鞋子:
                LoadItem<FootConfig>(ref _tree);
                break;
            case ItemType.裤子:
                LoadItem<PelvisConfig>(ref _tree);
                break;
            case ItemType.盾牌:
                LoadItem<ShieldConfig>(ref _tree);
                break;
            case ItemType.消耗品:
                LoadItem<ConsumablesConfig>(ref _tree);
                break;
        }        
            return _tree;
        }
    public void AddDragHandles<T>(OdinMenuItem item) where T:ItemConfig<T>
    {
        item.OnDrawItem += (t) => { (t.Value as T).SetEditorSprite(); };
    }
    public void CreateItem<T>(T i,ref OdinMenuTree t) where T:ItemConfig<T>
    {
        i.物品ID = _tree.MenuItems.Count + 1;
        _tree.Add("New "+typeof(T).Name, i);
        _tree.MenuItems[_tree.MenuItems.Count - 1].Select();
    }
    public void LoadItem<T>(ref OdinMenuTree t) where T : ItemConfig<T>
    {
        ItemConfig<T>.Reload();
        if (ItemConfig<T>.Count > 0)
        {
            foreach (var item in ItemConfig<T>.Datas)
            {
                t.Add(item.Value.物品名字, item.Value);
                t.EnumerateTree().AddIcons<T>(x => x.GetSprite());
                t.EnumerateTree().ForEach(AddDragHandles<T>);
            }
        }
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
                        case ItemType.鞋子:
                            CreateItem(new FootConfig(), ref _tree);
                            break;
                        case ItemType.裤子:
                            CreateItem(new PelvisConfig(), ref _tree);
                            break;
                        case ItemType.肩膀:
                            CreateItem(new ArmConfig(), ref _tree);
                            break;
                        case ItemType.手链:
                            CreateItem(new SleeveConfig(), ref _tree);
                            break;
                        case ItemType.武器:
                            CreateItem(new WeaponConfig(), ref _tree);
                            break;
                        case ItemType.上衣:
                            CreateItem(new TorsoConfig(), ref _tree);
                            break;
                    case ItemType.盾牌:
                        CreateItem(new ShieldConfig(), ref _tree);
                        break;
                    case ItemType.消耗品:
                        CreateItem(new ConsumablesConfig(), ref _tree);
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