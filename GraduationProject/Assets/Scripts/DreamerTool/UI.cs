using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace DreamerTool.UI
{
    public class Scene : MonoBehaviour
    {
        public Transform _root;
        public Dictionary<string, View> _views = new Dictionary<string, View>();
        private void Awake()
        {
            View.CurrentScene = this;
            for (int i = 0; i < _root.childCount; i++)
            {
                var child = _root.GetChild(i);
                _views.Add(child.name, child.GetComponent<View>());
            }
        }

        public T OpenView<T>() where T : View
        {
            var _name = typeof(T).Name;
            if (!_views.ContainsKey(_name))
                return null;
            _views[_name].gameObject.SetActive(true);

            return (T)_views[_name];
        }
        public T GetView<T>() where T : View
        {
            var _name = typeof(T).Name;
            if (!_views.ContainsKey(_name))
                return null;

            return (T)_views[_name];
        }
        public T CloseView<T>() where T : View
        {
            var _name = typeof(T).Name;
            if (!_views.ContainsKey(_name))
                return null;
            _views[_name].gameObject.SetActive(false);
            return (T)_views[_name];
        }
    }

    public class View: MonoBehaviour
    {
        public static Scene CurrentScene;

        public virtual void OnShow()
        {

        }
        public virtual void OnHide()
        {

        }
        private  void OnEnable()
        {
            OnShow();
        }
        private  void OnDisable()
        {
            OnHide();
        }
    }
}
