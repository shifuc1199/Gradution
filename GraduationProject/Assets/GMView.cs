/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
using Cinemachine;
using UnityEngine.UI;
public class GMView : View
{
    public GameObject m_text_prefab;
    public Transform m_root;
    public InputField m_input;
     
    public void ExecuteCommond()
    {
        var commond = m_input.text;
        var splits = commond.Split(';');
        var m_text = Instantiate(m_text_prefab, m_root);

        m_text.GetComponent<Text>().text = "<color=gren>"+commond + "  执行成功！"+"</color>";

        switch (splits[0])
        {
            case "change_camera_size":
                float value = float.Parse(splits[1]);
                Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = value;
                 
                break;
            default:
                m_text.GetComponent<Text>().text = "<color=red>无效的命令</color>";
                break;
        }
    }
}
