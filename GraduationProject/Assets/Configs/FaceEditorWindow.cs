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
        _tree = new OdinMenuTree(true);
        _tree.DefaultMenuStyle.IconSize = 28.00f;
        _tree.Config.DrawSearchToolbar = true;
        switch (config_type)
        {
            case FaceType.眼睛:
                LoadItem<EyeConfig>(ref _tree);
                break;
            case FaceType.嘴巴:
                LoadItem<MouthConfig>(ref _tree);
                break;
            case FaceType.发型:
                LoadItem<HairConfig>(ref _tree);
                break;
            case FaceType.耳朵:
                LoadItem<EarConfig>(ref _tree);
                break;
            case FaceType.发饰:
                LoadItem<HairDecorateConfig>(ref _tree);
                break;
            default:
                break;
        }
        return _tree;
    }

    public void CreateItem<T>(T i, ref OdinMenuTree t) where T : FaceConfig<T>
    {
        i.ID = _tree.MenuItems.Count + 1;
        _tree.Add("New " + typeof(T).Name, i);
        _tree.MenuItems[_tree.MenuItems.Count - 1].Select();
    }
    public void LoadItem<T>(ref OdinMenuTree t) where T : FaceConfig<T>
    {
        FaceConfig<T>.Reload();
        if (FaceConfig<T>.Count > 0)
        {
            foreach (var item in FaceConfig<T>.Datas)
            {
                t.Add(item.Value.ID.ToString(), item.Value);
                t.EnumerateTree().AddIcons<T>(x => x.GetSprite());
                t.EnumerateTree().ForEach(AddDragHandles<T>);
            }
        }
    }
    public void AddDragHandles<T>(OdinMenuItem item) where T : FaceConfig<T>
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
            foreach (var temp in Enum.GetNames(typeof(FaceType)))
            {
                if (SirenixEditorGUI.ToolbarButton(new GUIContent("     " + temp + "    ")))
                {
                    config_type = (FaceType)Enum.Parse(typeof(FaceType), temp);
                    ForceMenuTreeRebuild();
                }
            }
            SirenixEditorGUI.ToolbarTab(false, "");
            if (SirenixEditorGUI.ToolbarButton(new GUIContent("    +   ")) && !isCreate)
            {
                isCreate = true;
                switch (config_type)
                {
                    case FaceType.眼睛:
                        CreateItem(new EyeConfig(), ref _tree);
                        break;
                    case FaceType.嘴巴:
                        CreateItem(new MouthConfig(), ref _tree);
                        break;
                    case FaceType.发型:
                        CreateItem(new HairConfig(), ref _tree);
                        break;
                    case FaceType.耳朵:
                        CreateItem(new EarConfig(), ref _tree);
                        break;
                    case FaceType.发饰:
                        CreateItem(new HairDecorateConfig(), ref _tree);
                        break;
                    default:
                        break;
                }
            }
            if (SirenixEditorGUI.ToolbarButton(new GUIContent("    添加所有   ")))
            {
                switch (config_type)
                {
                    case FaceType.眼睛:
                        EyeConfig.LoadAll();
                        break;
                    case FaceType.嘴巴:
                        MouthConfig.LoadAll();
                        break;
                    case FaceType.发型:
                        HairConfig.LoadAll();
                        break;
                    case FaceType.耳朵:
                        EarConfig.LoadAll();
                        break;
                    case FaceType.发饰:
                        HairDecorateConfig.LoadAll();
                        break;
                    default:
                        break;
                }
            }
            if (SirenixEditorGUI.ToolbarButton(new GUIContent("    移除所有   ")))
            {
                switch (config_type)
                {
                    case FaceType.眼睛:
                        EyeConfig.RemoveAll();
                        break;
                    case FaceType.嘴巴:
                        MouthConfig.RemoveAll();
                        break;
                    case FaceType.发型:
                        HairConfig.RemoveAll();
                        break;
                    case FaceType.耳朵:
                        EarConfig.RemoveAll();
                        break;
                    case FaceType.发饰:
                        HairDecorateConfig.RemoveAll();
                        break;
                    default:
                        break;
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }
    }
}
#endif