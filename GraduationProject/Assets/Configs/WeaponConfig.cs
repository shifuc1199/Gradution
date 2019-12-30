using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using UnityEditor;
 using Sirenix.OdinInspector;
public   class WeaponConfig : BaseConfig<WeaponConfig>
{
	[ReadOnly]
	[BoxGroup("基础信息")]
	[VerticalGroup("基础信息/p/o")]
	public int 武器ID;/*nil*/
	[BoxGroup("基础信息")]
	[VerticalGroup("基础信息/p/o")]
	public string 武器名字;/*nil*/
	[BoxGroup("基础信息")]
	[VerticalGroup("基础信息/p/o")]
	public WeaponType 武器种类;/*nil*/
    
	[BoxGroup("基础信息")]
	[VerticalGroup("基础信息/p/o")]
	[TextArea()]
	public string 武器描述;/*nil*/
	[JsonNonField]
	[HideLabel,PreviewField(100)]
    [AssetsOnly]
	[BoxGroup("基础信息")]
	[HorizontalGroup("基础信息/p",100)]
    [SerializeField]
	private Sprite 编辑器武器图标=null;
	[NonSerialized]
    public string 图标名字;
	[BoxGroup("属性信息")]
	public double 攻击力;
	[BoxGroup("属性信息")]
	public double 法术强度;
	[BoxGroup("属性信息")]
	public double 暴击率;
	[BoxGroup("属性信息")]
	public double 暴击伤害;
	[Button("保存",50)]
	public void Save()
	{
		TextAsset ta =Resources.Load<TextAsset>("all_config");
		JsonData jd = JsonMapper.ToObject(ta.text);
		if(jd["Weapon"]==null)
		jd["Weapon"] = new JsonData();
		JsonData data = new JsonData();
        data["武器ID"] = this.武器ID;
        data["武器名字"] =this.武器名字;
        data["武器种类"] = (int)this.武器种类;
        data["武器描述"] = this.武器描述;
		data["图标名字"] = this.编辑器武器图标?this.编辑器武器图标.name:"";
        data["攻击力"] = this.攻击力;
        data["法术强度"] = this.法术强度;
        data["暴击率"] =this.暴击率;
		data["暴击伤害"] = this.暴击伤害;
		jd["Weapon"][this.武器ID.ToString()] = data;
		using(StreamWriter sw = new StreamWriter(new FileStream("Assets/Resources/all_config.json",FileMode.OpenOrCreate)))
		{
			sw.Write(jd.ToJson());
		}
		AssetDatabase.Refresh();
		MyEditorWindow._window.ForceMenuTreeRebuild();
		MyEditorWindow._window.isCreate=false;
		MyEditorWindow._window._tree.MenuItems[MyEditorWindow._window._tree.MenuItems.Count-1].Select();
	}
    public Sprite GetSprite()
    {
        return Resources.Load<Sprite>(图标名字);
    }
    public void SetEditorSprite()
    {
        if (编辑器武器图标 == null)
            编辑器武器图标 = GetSprite();
    }
 

 
	 
}
 

