/*****************************
Created by 师鸿博
*****************************/
using cn.bmob.api;
using cn.bmob.io;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserBmobDao : BmobTable
{
 
    public string id { get; set; }

    public string data { get; set; }
    //构造函数
    public UserBmobDao(string id,string data) {
        this.id = id;
        this.data = data;

    }
    public UserBmobDao() { }
    public void Insert(Action insert_call_back)
    {
 
        BmobManager.Instance.Bmob.Create(GameConstData.USER_TABLE_NAME, this, (resp, exce) =>
        {
            if (exce != null)
            {
                Debug.LogError("Exists Error: " + exce.Message);
                return;
            }
            Debug.Log("插入成功!");
            insert_call_back();
        });
        
    }
    public void Update(Action update_call_back)
    {
        FindByID((dao)=> {

            if (dao == null)
            {
                Debug.Log("更新失败 没找到");
                return;
            }
            var object_id = dao.objectId;
  
            BmobManager.Instance.Bmob.Update(GameConstData.USER_TABLE_NAME, object_id, this, (resp, exce) =>
            {
                if (exce != null)
                {
                    Debug.LogError("Exists Error: " + exce.Message);
                    return;
                }
                Debug.Log("更新成功!");
                update_call_back.Invoke();
            });


        });
       
    }
    public void FindByID(Action<UserBmobDao> call_back)
    {
 
        BmobQuery query = new BmobQuery();
        BmobManager.Instance.Bmob.Find<UserBmobDao>(GameConstData.USER_TABLE_NAME, query.WhereEqualTo("id", id.ToString()), (resp, exce) =>
        {
            UserBmobDao result = null;
            if (exce != null)
            {
                Debug.LogError(exce.Message);
                return;
            }
            if (resp.results.Count > 0)
                result = resp.results[0];
            else
                Debug.Log(" 没有这个ID");

            call_back(result);
        });
        
    }
 

    public override void readFields(BmobInput input)
    {
        base.readFields(input);
   
        this.id = input.getString("id");
        this.data = input.getString("data");
      
    }

    public override void write(BmobOutput output, bool all)
    {
        base.write(output, all);

        output.Put("id", this.id);
        output.Put("data", this.data);
 
    }
}
