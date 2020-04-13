using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
public class SkillJoyStick : JoyStick
{
    public int skill_joy_stick_id;

    public SplatManager splat_manager;
    SkillModel model;
    bool isCoolDown;
    double cool_timer;
    public Image no_enrgy_mask_image;
    public Image cool_mask_image;
    public Image skill_image;
    private void Awake()
    {
        EventManager.OnChangeEnergy += UpdateJoyStick;
        
        
    }
    private void Start()
    {
        splat_manager = FindObjectOfType<SplatManager>();
        UpdateModel();
    }
    private void OnDestroy()
    {
        EventManager.OnChangeEnergy -= UpdateJoyStick;
    }

    public void UpdateJoyStick()
    {
        if (model == null)
            return;
        if (model.GetConsumeEnergy() > ActorModel.Model.GetEngery())
        {
            isDisable = true;
            no_enrgy_mask_image.gameObject.SetActive(true);

        }
        else
        {
            no_enrgy_mask_image.gameObject.SetActive(false);
            isDisable = false;
        }
    }
    public void UpdateModel()
    {
        
        model = ActorModel.Model.equip_skil[skill_joy_stick_id];
     
        if (model == null)
        {
            isDisable = true;
            skill_image.gameObject.SetActive(false);
           
        }
        else
        {
            isDisable = false;
            skill_image.gameObject.SetActive(true);
            skill_image.sprite = model._config.GetSprite();
            cool_timer = model.GetCoolTime();
            if (model._config.skill_type == SkillType.点击)
            {
                isDrag = false;
            }
            else
            {
                isDrag = true;
            }
        }
 

    }
    public override void onJoystickDown(Vector2 V,float R)
    {
        base. onJoystickDown(V,R);
        
        switch (model._config.skill_type)
        {
            case SkillType.点:
                splat_manager.SelectSpellIndicator("skill" + model._config.ID + "_indicator");
                splat_manager.CurrentSpellIndicator.transform.position = (Vector3)V * R + ActorController.Controller.transform.position;
                break;
            case SkillType.线:
                splat_manager.SelectSpellIndicator("skill" + model._config.ID + "_indicator");
                splat_manager.CurrentSpellIndicator.transform.rotation = Quaternion.FromToRotation(Vector2.up, V);
                break;
            case SkillType.点击:
                break;
            default:
                break;
        }
        
    }
    public override void onJoystickUp(Vector2 V, float R)
    {
        base.onJoystickUp(V,R);
        if (model._config.skill_type != SkillType.点击)
        {
            splat_manager.CancelSpellIndicator();
        }
        isCoolDown = true;
      
        ActorController.Controller.skill_controller.ExecuteSkill(model._config.ID,V, (Vector3)V * R + ActorController.Controller.transform.position);
    }
    public override void onJoystickMove(Vector2 V, float R)
    {
        base.onJoystickMove(V, R);
        switch (model._config.skill_type)
        {
            case SkillType.点:
                splat_manager.CurrentSpellIndicator.transform.position =  (Vector3)V*R+ActorController.Controller.transform.position;
                break;
            case SkillType.线:
                splat_manager.CurrentSpellIndicator.transform.rotation = Quaternion.FromToRotation(Vector2.up, V);
                break;
            case SkillType.点击:
                return;
            default:

                break;
        }
         

    }
    private void Update()
    {
        if(isCoolDown )
        {
             
            isDisable = true;

            cool_timer -= Time.fixedDeltaTime;
            cool_mask_image.fillAmount = (float)(cool_timer / model.GetCoolTime());
            cool_mask_image.GetComponentInChildren<Text>().text = cool_timer.ToString("f1") + "s";
            if (cool_timer<=0)
            {
                cool_mask_image.GetComponentInChildren<Text>().text = "";
                cool_timer = model.GetCoolTime();
                if (!no_enrgy_mask_image.gameObject.activeSelf)
                    isDisable = false;
                isCoolDown = false;
            }

        }
    }
}
