using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using UnityEngine.Events;
namespace DreamerTool.Util
{
    public static class Util
    {
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
        public static List<int> GetRandomNonRepeat(List<int> temp, int number)
        {
            List<int> result = new List<int>();

            for (int i = 0; i < number; i++)
            {
                var index = Random.Range(0, temp.Count);
                result.Add(temp[index]);
                temp.RemoveAt(index);
            }
            return result;
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
    public class BaseGameObjectPool
    {
       
        private Queue<GameObject> object_pool_queue = new Queue<GameObject>();
        private GameObject _prefab;

        public BaseGameObjectPool(GameObject _prefab)
        {
            this._prefab = _prefab;
        }

        public virtual GameObject Get(Vector3 pos, float life_time)
        {
            GameObject get_object = null;
            if (object_pool_queue.Count == 0)
            {
                get_object = GameObject.Instantiate(_prefab);
                ObjectRecover _recover = get_object.AddComponent<ObjectRecover>();
                _recover._life_time = life_time;
                _recover.recover_call_back = Remove;
            }
            else
            {
                get_object = object_pool_queue.Dequeue();
            }
            get_object.SetActive(true);
            get_object.transform.position = pos;
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
        public float _life_time;
        public UnityAction<GameObject> recover_call_back;
        private void OnEnable()
        {
            Invoke("Recover", _life_time);
        }
        public void Recover()
        {
            recover_call_back?.Invoke(gameObject);
        }
    }
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