using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
public class ItemSprite : MonoBehaviour
{
    public ItemType item_type;
    public int config_id;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            View.CurrentScene.GetView<GameInfoView>().SetInactiveType(InactiveType.拾取);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

        }
    }
    private void Awake()
    {
         
        switch (item_type)
        {
            case ItemType.武器:
               GetComponent<SpriteRenderer>().sprite =  WeaponConfig.Get(config_id).GetSprite();
                GetComponentInChildren<TextMesh>().text = WeaponConfig.Get(config_id).武器名字;
                break;
    
            default:
                break;
        }
    }
}
