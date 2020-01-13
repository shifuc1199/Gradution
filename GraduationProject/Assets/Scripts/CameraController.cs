using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.GameObjectPool;
using DG.Tweening;
public class CameraController : MonoBehaviour
{
    public GameObject aim_prefab;
    public GameObject effect_prefab;
    public GameObject separated_body;
    public List<Transform> enemys = new List<Transform>();
      private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<BaseEnemyController>().IsLocked();
            enemys.Add(other.transform);
        }

      
    }
    public void Skill_Click()
    {
        StartCoroutine(Skill());
    }
    bool isskill = false;
    public IEnumerator Skill()
    {
        isskill = true;
        ActorController._controller.gameObject.SetActive(false);
        GetComponent<Collider2D>().enabled=true;
        yield return new WaitForFixedUpdate();
        foreach(var item in enemys)
        {
          GameObject p =  Instantiate(aim_prefab,item);
            
        }
        Timer.Register(5, () => { ActorController._controller.gameObject.SetActive(true); });
        
    }
    Vector2 pos ;
    GameObject temp ;
    
    private void Awake()
    {
        GameObjectPoolManager.AddPool("attack_effect_pool",effect_prefab);
    }
    public void Update()
    {
        
        if (isskill)
        {
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 100000,LayerMask.GetMask("enemy"));
                if (hit.collider)
                {
                    separated_body.SetActive(true);
                    var pos_list = new List<int>(){-1,0,1};
                    var x = pos_list[Random.Range(0,pos_list.Count)];
                    var y = pos_list[Random.Range(0,pos_list.Count)];
                    while (x==y&& x==0)
                    {
                        x  = pos_list[Random.Range(0,pos_list.Count)];
                    }
                    separated_body.transform.position = hit.collider.transform.position + new Vector3(x, y, 0).normalized * 18;
                    separated_body.transform.right =  (hit.collider.transform.position - separated_body.transform.position).normalized;
                    pos = separated_body.transform.position +separated_body.transform.right*36;
                    temp = GameObjectPoolManager.GetPool("attack_effect_pool").Get(separated_body.transform.position+separated_body.transform.right*15, Quaternion.Euler(new Vector3(180-Quaternion.FromToRotation(Vector2.right, separated_body.transform.right).eulerAngles.z,90,0)),0.5f);
                    temp.GetComponentInChildren<BaseAttackTrigger>().attack_type = HitType.普通;
                     
                }
            }
             separated_body.transform.position = Vector3.MoveTowards(separated_body.transform.position,pos,4f);
           
        }
    }
 
}
