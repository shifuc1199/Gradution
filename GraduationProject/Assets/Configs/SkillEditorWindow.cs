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

 
public class SkillEditorWindow : OdinMenuEditorWindow
{
    
    public static SkillEditorWindow _window;
    public OdinMenuTree _tree;
    public bool isCreate;
    // Start is called before the first frame update
        [MenuItem("DreamerEditor/技能编辑器")]
        private static void Open()
        {
            _window = GetWindow<SkillEditorWindow>();
        _window.titleContent = new GUIContent("技能编辑器");
            _window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
        }
          protected override OdinMenuTree BuildMenuTree()
        {
            _tree= new OdinMenuTree(true);
            _tree.DefaultMenuStyle.IconSize = 28.00f;
            _tree.Config.DrawSearchToolbar = true;
            SkillConfig.Reload();
            if (SkillConfig.Count > 0)
            {

                foreach (var item in SkillConfig.Datas)
                {

                    // Adds the character overview table.

                    _tree.Add(item.Value.SkillName, item.Value);
                    _tree.EnumerateTree().AddIcons<SkillConfig>(x => x.GetSprite());
                    _tree.EnumerateTree().ForEach(AddDragHandles);
                }
            }
        return _tree;
        }
    public void AddDragHandles(OdinMenuItem item)
    {
        item.OnDrawItem += (t) => { (t.Value as SkillConfig).SetEditorSprite();  };
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
                SkillConfig w = new SkillConfig();
                w.ID = _tree.MenuItems.Count + 1;
                _tree.Add("New Skill", w);
                _tree.MenuItems[_tree.MenuItems.Count - 1].Select();

            }


        }
            
            SirenixEditorGUI.EndHorizontalToolbar();

            
            
        }
}
#endif