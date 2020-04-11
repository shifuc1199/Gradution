using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
 
using UnityEngine.Networking;
using DreamerTool.ScriptableObject;
 
using System;
using System.Data;
using UnityEngine.SceneManagement;
//http://www.hko.gov.hk/cgi-bin/gts/time5a.pr?a=1 时间
namespace DreamerTool.UI
{
    public class Scene : MonoBehaviour
    {
        public Transform _root;
        public static Camera UICamera;
        public Dictionary<string, View> _views = new Dictionary<string, View>();

        public virtual void Awake()
        {
            View.CurrentScene = this;

            var UICameraGameObject = GameObject.FindGameObjectWithTag("UICamera");

            if (UICameraGameObject)
                UICamera = UICameraGameObject.GetComponent<Camera>();

            if (_root)
            {
                for (int i = 0; i < _root.childCount; i++)
                {
                    var child = _root.GetChild(i);
                    if (child.GetComponent<View>())
                        _views.Add(child.name, child.GetComponent<View>());
                }
            }

        }
        private void OnEnable()
        {
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
        public virtual void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
        {
            GameObjectPool.GameObjectPoolManager.ClearAll();
        }

    }

    public class View : MonoBehaviour
    {
        public static Scene CurrentScene;

        public virtual void OnShow()
        {

        }
        public virtual void OnHide()
        {

        }
        private void OnEnable()
        {
            OnShow();
        }
        private void OnDisable()
        {
            OnHide();
        }
        public void OnCloseClick()
        {
            gameObject.SetActive(false);
        }
        public void OnShowClick()
        {
            gameObject.SetActive(true);
        }
    }
}
namespace DreamerTool.Singleton
{
    public class Singleton<T> where T : new()
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                }
                return instance;
            }
        }
    }
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    GameObject temp = new GameObject();
                    temp.name = typeof(T).Name;
                    instance = temp.AddComponent<T>();
                }
                return instance;
            }
        }
    }
}


namespace DreamerTool.Util
{
    public static class DreamerUtil
    {
        public static string GetColorRichText(string t,Color c)
        {
            return "<color=#"+ ColorUtility.ToHtmlStringRGBA(c) + ">" + t + "</color>";
        }
        public static string GetColorRichText(string t, string c)
        {
            return "<color=#" + c + ">" + t + "</color>";
        }

        public static System.Collections.IEnumerator GetDateTimeFromURL(UnityAction<System.DateTime> action)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get("http://www.hko.gov.hk/cgi-bin/gts/time5a.pr?a=1");
            yield return webRequest.SendWebRequest();
            if(webRequest.isNetworkError)
            {
                yield break;
            }
             
            System.DateTime start =System.TimeZone.CurrentTimeZone.ToLocalTime( new System.DateTime(1970, 1, 1));
            start =  start.AddMilliseconds(long.Parse(webRequest.downloadHandler.text.Substring(2)));
            
            action(start);
            yield return  null;

        }
    
        public static double GetHurtValue(double a, double d)
        {
            if ((a + d) == 0)
                return 0;

            return a * a / (a + d);
        }
         
       
        public static List<int> GetRandomNonRepeat(List<int> temp, int number)
        {
            List<int> result = new List<int>();

            for (int i = 0; i < number; i++)
            {
                var index = UnityEngine.Random.Range(0, temp.Count);
                result.Add(temp[index]);
                temp.RemoveAt(index);
            }
            return result;
        }
      
    }
}
namespace DreamerTool.FSM
{
    public class StateMachine
{
     public StateBase current_state;
     public Dictionary<string,StateBase> states=new Dictionary<string,StateBase>();
     public void AddState(StateBase state)
     {
        states.Add(state.id,state);
     }
     public void RemoveState(StateBase state)
     {
        states.Remove(state.id);
     }
    public void ChangeState(string id)
    {
        if(!states.ContainsKey(id))
        {
            return;
        }
        if(current_state!=null)
        {
            current_state.OnExit();
        }
        states[id].OnEnter();
        current_state = states[id];
 
        }
}
 
public class  StateBase
{
   
    public string id;
    public StateBase(string _id)
    {
        this.id = _id;
    }
    public virtual void OnEnter(params object[] args){}
    public virtual void OnStay(params object[] args){ Debug.Log("123");}
    public virtual void OnExit(params object[] args){}
    
}
public class StateBaseTemplate<T>:StateBase
{
    public  T owner;
    public StateBaseTemplate(string _id,T owner):base(_id)
    {
        this.id = _id;
        this.owner  = owner;
    }
}
}
namespace DreamerTool.GameObjectPool
{
    public static class GameObjectPoolManager
    {
        private static Dictionary<string, GameObjectPool> pools = new Dictionary<string, GameObjectPool>();
        public static void InitByScriptableObject()
        {
            var prefabs = ScriptableObjectUtil.GetScriptableObject<GameObjectPoolPrefabs>();
            foreach (var item in prefabs.Prefabs)
            {
                if(!pools.ContainsKey(item.prefab_name))
                pools.Add(item.prefab_name, new GameObjectPool(item.prefab));
            }
        }
        public static void ClearAll()
        {
            foreach (var pool in pools)
            {
                pool.Value.Clear();
            }
        }
        public static GameObjectPool AddPool(string pool_id, GameObject prefab)
        {
            if (pools.ContainsKey(pool_id))
            {
                return null;
            }
            var pool = new GameObjectPool(prefab);
            pools.Add(pool_id, pool);
            return pool;
        }

        public static GameObjectPool GetPool(string pool_id)
        {
            return pools[pool_id];
        }
    }
    public class GameObjectPool
    {
       
        private Queue<GameObject> object_pool_queue = new Queue<GameObject>();
        private GameObject _prefab;

        public GameObjectPool(GameObject _prefab)
        {
            this._prefab = _prefab;
        }

        public virtual GameObject Get(Vector3 pos,Quaternion rot, float life_time)
        {
            GameObject get_object = null;
            if (object_pool_queue.Count == 0)
            {
                get_object = GameObject.Instantiate(_prefab);
                ObjectRecover _recover = get_object.AddComponent<ObjectRecover>();
                _recover.recover_call_back = Remove;
            }
            else
            {
                get_object = object_pool_queue.Dequeue();
            }
            get_object.GetComponent<ObjectRecover>().Recover(life_time);
            get_object.SetActive(true);
            get_object.transform.position = pos;
            get_object.transform.rotation = rot;
            return get_object;
        }
        public virtual void Remove(GameObject tempObject)
        {
            tempObject.SetActive(false);
            object_pool_queue.Enqueue(tempObject);
        }

        public void Clear()
        {
            object_pool_queue.Clear();
        }
    }

    public class ObjectRecover : MonoBehaviour
    {
        public UnityAction<GameObject> recover_call_back;
        public void Recover(float timer)
        {
            if (timer == -1)
                return;

            Invoke("Recover", timer);
        }
        public void RecoverImmediately()
        {
            CancelInvoke();
            Recover();
        }
        private void Recover()
        {
            recover_call_back?.Invoke(gameObject);
        }
    }
}
namespace DreamerTool.ScriptableObject
{
    public class ScriptableObjectUtil
    {
        public static T GetScriptableObject<T>() where T:UnityEngine.Object
        {
            return Resources.Load<T>("ScriptableObject/"+typeof(T).Name);
        }
        public static T GetScriptableObject<T>(string _name) where T : UnityEngine.Object
        {
            return Resources.Load<T>("ScriptableObject/" + _name);
        }
    }


}
namespace DreamerTool.EditorTool
{
#if UNITY_EDITOR
    using System.IO;
    using UnityEngine;
    using UnityEditor;
    using System.Linq;

    public class SpriteToSplit
    {
        /// <summary>
        /// 切割Sprite导出单个对象
        /// </summary>
        [MenuItem("DreamTool/SpriteSplit2Export", false, 12)]
        public static void SpriteChildToExport()
        {
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                //获得选择对象路径;
                string spritePath = AssetDatabase.GetAssetPath(Selection.objects[i]);
                //所有子Sprite对象;
                Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(spritePath).OfType<Sprite>().ToArray();
                if (sprites.Length < 1)
                {
                    EditorUtility.DisplayDialog("错误", "当前选择文件不是Sprite!", "确认");
                    Debug.LogError("Sorry,There is not find sprite!");
                    return;
                }
                string[] splitSpritePath = spritePath.Split(new char[] { '/' });
                //文件夹路径 通过完整路径再去掉文件名称即可;
                string fullFolderPath = Inset(spritePath, 0, splitSpritePath[splitSpritePath.Length - 1].Length + 1) + "/" + Selection.objects[i].name;
                //同名文件夹;
                string folderName = Selection.objects[i].name;
                string adjFolderPath = InsetFromEnd(fullFolderPath, Selection.objects[i].name.Length + 1);
                //验证路径;
                if (!AssetDatabase.IsValidFolder(fullFolderPath))
                {
                    AssetDatabase.CreateFolder(adjFolderPath, folderName);
                }

                for (int j = 0; j < sprites.Length; j++)
                {   //进度条;
                    string pgTitle = (i + 1).ToString() + "/" + Selection.objects.Length + " 开始导出Sprite";
                    string info = "当前Srpte: " + j + "->" + sprites[j].name;
                    float nowProgress = (float)j / (float)sprites.Length;
                    EditorUtility.DisplayProgressBar(pgTitle, info, nowProgress);
                    //创建Texture;
                    Sprite sprite = sprites[j];
                    Texture2D tex = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, sprite.texture.format, false);
                    tex.SetPixels(sprite.texture.GetPixels((int)sprite.rect.xMin, (int)sprite.rect.yMin,
                        (int)sprite.rect.width, (int)sprite.rect.height));
                    tex.Apply();
                    //判断保存路径;
                    string savePath = fullFolderPath + "/" + sprites[j].name + ".png";
 
                    //生成png;
                    File.WriteAllBytes(savePath, tex.EncodeToPNG());
                }
                //释放进度条;
                EditorUtility.ClearProgressBar();
               
                //刷新资源，不然导出后你以为没导出，还要手动刷新才能看到;
                AssetDatabase.Refresh();
            }
        }
        /// <summary>
        /// 截取路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="leftIn">左起点</param>
        /// <param name="rightIn">右起点</param>
        /// <returns></returns>
        public static string Inset(string path, int leftIn, int rightIn)
        {
 
            return path.Substring(leftIn, path.Length - rightIn - leftIn);
        }
        /// <summary>
        /// 截取路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="inset"></param>
        /// <returns></returns>
        public static string InsetFromEnd(string path, int inset)
        {
            return path.Substring(0, path.Length - inset);
        }
    }
#endif
}

namespace DreamerTool.Inactive
{
    public class InactiveTrigger : MonoBehaviour
    {
        public string _inactive_key;
        public GameObject _inactive_key_show;

        [Header("-----------事件-----------")]
        public UnityEvent _inactive_event;

        public virtual void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag != "Player")
                return;
            _inactive_key_show.SetActive(true);
            
        }

        public virtual void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag != "Player")
                return;
            _inactive_key_show.SetActive(false);
        }

        public virtual void OnTriggerStay(Collider other)
        {
            if (_inactive_event == null || other.gameObject.tag != "Player")
                return;
            if (Input.GetKeyDown(_inactive_key))
                _inactive_event.Invoke();
        }
    }
}
 
namespace DreamerTool.Extra
{
    
    public static class Extra
    {
        public static void Copy<Tkey, Tvalue>(this Dictionary<Tkey, Tvalue> dict, Dictionary<Tkey, Tvalue> copy_dict)
        {
            foreach (var item in copy_dict)
            {
                if (dict.ContainsKey(item.Key))
                    dict[item.Key] = item.Value;
                else
                    dict.Add(item.Key, item.Value);
            }
        }
        public static KeyValuePair<Tkey, Tvalue> Get<Tkey, Tvalue>(this Dictionary<Tkey, Tvalue> dict, int index)
        {
            int temp = 0;
            foreach (var item in dict)
            {

                if (temp == index)
                {
                    return item;
                }
                temp++;
            }
            return default;
        }
        public static List<int> GetKeys<Tvalue>(this Dictionary<int, Tvalue> dic)
        {
            List<int> temp = new List<int>();
            foreach (var item in dic)
            {
                temp.Add(item.Key);
            }
            return temp;
        }
        public static T GetLast<T>(this List<T> list)
        {
            if (list.Count == 0)
                return default;

            return list[list.Count - 1];
        }
        public static List<Transform> GetChildren(this Transform tran)
        {
            List<Transform> result = new List<Transform>();
            for (int i = 0; i <  tran.childCount; i++)
            {
                result.Add(tran.GetChild(i));
            }
            return result;
        }
        public static void ResetVelocity(this Rigidbody2D _rigi)
        {
            _rigi.velocity = Vector2.zero;
        }
        public static void SetPositionX(this Transform tran, float x)
        {
            tran.position = new Vector3(x, tran.position.y, tran.position.z);
        }
        public static void SetPositionY(this Transform tran, float y)
        {
            tran.position = new Vector3(tran.position.x, y, tran.position.z);
        }
        public static void SetPositionZ(this Transform tran, float z)
        {
            tran.position = new Vector3(tran.position.x, tran.position.y, z);
        }
        public static void ClearGravity(this Rigidbody2D _rigi)
        {
            _rigi.gravityScale = 0;
        }
        public static Color GetColorByString(this string color_s)
        {
            Color result;
            ColorUtility.TryParseHtmlString(color_s, out result);
            return result;
        }
        public static void SetGravity(this Rigidbody2D _rigi, float v)
        {
            _rigi.gravityScale = v;
        }
        public static bool IsAnim(this Animator animator, string name)
        {
            return animator.GetCurrentAnimatorStateInfo(0).IsName(name);
        }
    }
}

