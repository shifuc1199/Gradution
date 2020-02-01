using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace DreamerTool.UI
{
    public class Scene : MonoBehaviour
    {
        public Transform _root;
        public Dictionary<string, View> _views = new Dictionary<string, View>();
        public virtual void Awake()
        { 
            View.CurrentScene = this;
            if (_root)
            {
                for (int i = 0; i < _root.childCount; i++)
                {
                    var child = _root.GetChild(i);
                    _views.Add(child.name, child.GetComponent<View>());
                }
            }
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
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
        public void SceneChange(string scene_name)
        {
            SceneManager.LoadScene(scene_name);
        }
        public virtual void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene,LoadSceneMode mode)
        {
            GameObjectPool.GameObjectPoolManager.ClearAll();
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
        public virtual void OnCloseClick()
        {
            gameObject.SetActive(false);
        }
        public virtual void OnShowClick()
        {
            gameObject.SetActive(true);
        }
    }
}
