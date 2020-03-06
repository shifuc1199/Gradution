using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
public class ItemSprite : MonoBehaviour
{
    public ItemType item_type;
    public int config_id;
    public TextMesh _text;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {

            View.CurrentScene.GetView<GameInfoView>().SetInactiveType(InactiveType.拾取,this);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            View.CurrentScene.GetView<GameInfoView>().SetInactiveType(InactiveType.攻击);
        }
    }
    private void Awake()
    { 
        switch (item_type)
        {
            case ItemType.武器:
                GetComponent<SpriteRenderer>().sprite =  WeaponConfig.Get(config_id).GetSprite();
                transform.localScale = Vector2.one * 3;
                transform.rotation = Quaternion.Euler(0, 0, -90);
                _text.text = WeaponConfig.Get(config_id).物品名字;
                _text.color = GameStaticData.ITEM_COLOR_DICT[WeaponConfig.Get(config_id).物品阶级];
                break;
            case ItemType.上衣:
                GetComponent<SpriteRenderer>().sprite = TorsoConfig.Get(config_id).GetSprite();
                _text.text = TorsoConfig.Get(config_id).物品名字;
                _text.color = GameStaticData.ITEM_COLOR_DICT[TorsoConfig.Get(config_id).物品阶级];
                break;
            case ItemType.手链:
                GetComponent<SpriteRenderer>().sprite = SleeveConfig.Get(config_id).GetSprite();
                _text.text = SleeveConfig.Get(config_id).物品名字;
                _text.color = GameStaticData.ITEM_COLOR_DICT[SleeveConfig.Get(config_id).物品阶级];
                break;
            case ItemType.肩膀:
                GetComponent<SpriteRenderer>().sprite = ArmConfig.Get(config_id).GetSprite();
                _text.text = ArmConfig.Get(config_id).物品名字;
                _text.color = GameStaticData.ITEM_COLOR_DICT[ArmConfig.Get(config_id).物品阶级];
                break;
            case ItemType.裤子:
                GetComponent<SpriteRenderer>().sprite = PelvisConfig.Get(config_id).GetSprite();
                _text.text = PelvisConfig.Get(config_id).物品名字;
                _text.color = GameStaticData.ITEM_COLOR_DICT[PelvisConfig.Get(config_id).物品阶级];
                break;
            case ItemType.鞋子:
                GetComponent<SpriteRenderer>().sprite = FootConfig.Get(config_id).GetSprite();
                _text.text = FootConfig.Get(config_id).物品名字;
                _text.color = GameStaticData.ITEM_COLOR_DICT[FootConfig.Get(config_id).物品阶级];
                break;
            default:
                break;
        }
        if (!GetComponent<PolygonCollider2D>())
        {
            gameObject.AddComponent<PolygonCollider2D>().isTrigger=true;
        }
    }
}
