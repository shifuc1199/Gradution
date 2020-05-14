#if UNITY_EDITOR
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;
using System.Linq;
 
 
public class EnemyEditorWindow : OdinMenuEditorWindow
{
    
    public static EnemyEditorWindow _window;
    public OdinMenuTree _tree;
    public bool isCreate;
    // Start is called before the first frame update
        [MenuItem("DreamerEditor/敌人编辑器")]
        private static void Open()
        {
        
            _window = GetWindow<EnemyEditorWindow>();
        _window.titleContent = new GUIContent("敌人编辑器");
            _window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
        }
          protected override OdinMenuTree BuildMenuTree()
        {
            _tree= new OdinMenuTree(true);
            _tree.DefaultMenuStyle.IconSize = 28.00f;
            _tree.Config.DrawSearchToolbar = true;
            EnemyConfig.Reload();
            if (EnemyConfig.Count > 0)
            {

                foreach (var item in EnemyConfig.Datas)
                {

                    // Adds the character overview table.

                    _tree.Add(item.Value.EnemyName, item.Value);
                    _tree.EnumerateTree().AddIcons<EnemyConfig>(x => x.GetSprite());
                    _tree.EnumerateTree().ForEach(AddDragHandles);
                }
            }
        return _tree;
        }
    public void AddDragHandles(OdinMenuItem item)
    {
        item.OnDrawItem += (t) => { (t.Value as EnemyConfig).SetEditorSprite(); (t.Value as EnemyConfig).SetEditorPrefab(); };
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
                EnemyConfig w = new EnemyConfig();
                w.EnemyID = _tree.MenuItems.Count + 1;
                _tree.Add("New Enemy", w);
                _tree.MenuItems[_tree.MenuItems.Count - 1].Select();

            }


        }
            
            SirenixEditorGUI.EndHorizontalToolbar();

            
            
        }
}
#endif