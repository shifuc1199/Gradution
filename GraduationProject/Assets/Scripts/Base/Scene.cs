using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene : MonoBehaviour
{
    public Transform _root;

    private Dictionary<string, View> Views = new Dictionary<string, View>();

    private void Awake()
    {
        _root = GameObject.Find("Root").transform;
        View.CurrentScene = this;
    }

    public void OpenView<T>()
    {
        string _name = typeof(T).Name;
        if (Views.ContainsKey(_name))
        {
            Views[_name].gameObject.SetActive(true);
        }
        else
        {
            GameObject _view = Resources.Load<GameObject>("Views/"+_name);
            Views.Add(_name, _view.GetComponent<View>());
        } 
    }

    public void CloseView<T>()
    {

    }

    public void CloseAllView()
    {
        foreach (var item in Views)
        {
            item.Value.gameObject.SetActive(false);
        }
    }

    public void CloseAllViewExcept<T>()
    {
        string name = typeof(T).Name;
        foreach (var item in Views)
        {
            if(item.Key != name)
            item.Value.gameObject.SetActive(false);
        }
    }


}
