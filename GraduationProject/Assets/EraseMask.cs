
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
public class EraseMask : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{

    //是否擦除了
    public bool isStartEraser;

    //是否擦除结束了
    public bool isEndEraser;

    //开始事件
    public UnityEvent eraserStartEvent;

    //结束事件
    public UnityEvent eraserEndEvent;

    public RawImage uiTex;
    public Texture2D tex;
    Texture2D MyTex;
    int mWidth;
    int mHeight;

    [Header("Brush Size")]
    public int brushSize = 50;

    [Header("Rate")]
    public int rate = 90;

    float maxColorA;
    float colorA;

    public void SetTexture()
    {
        uiTex.DOFade(1, 0);
        MyTex = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);
        mWidth = MyTex.width;
        mHeight = MyTex.height;

        MyTex.SetPixels(tex.GetPixels());
        MyTex.Apply();
        uiTex.texture = MyTex;
        maxColorA = MyTex.GetPixels().Length;
        colorA = 0;
        isEndEraser = false;
        isStartEraser = false;
    }
 
 

    /// <summary>
    /// 贝塞尔平滑
    /// </summary>
    /// <param name="start">起点</param>
    /// <param name="mid">中点</param>
    /// <param name="end">终点</param>
    /// <param name="segments">段数</param>
    /// <returns></returns>
    public Vector2[] Beizier(Vector2 start, Vector2 mid, Vector2 end, int segments)
    {
        float d = 1f / segments;
        Vector2[] points = new Vector2[segments - 1];
        for (int i = 0; i < points.Length; i++)
        {
            float t = d * (i + 1);
            points[i] = (1 - t) * (1 - t) * mid + 2 * t * (1 - t) * start + t * t * end;
        }
        List<Vector2> rps = new List<Vector2>();
        rps.Add(mid);
        rps.AddRange(points);
        rps.Add(end);
        return rps.ToArray();
    }



    bool startDraw = false;
    bool twoPoints = false;
    Vector2 lastPos;//最后一个点
    Vector2 penultPos;//倒数第二个点
    float radius = 12f;
    float distance = 1f;



    #region 事件
    public void OnPointerDown(PointerEventData eventData)
    {
        if (isEndEraser) { return; }
        startDraw = true;
        penultPos = eventData.position;
        CheckPoint(penultPos);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isEndEraser) { return; }
        if (twoPoints && Vector2.Distance(eventData.position, lastPos) > distance)//如果两次记录的鼠标坐标距离大于一定的距离，开始记录鼠标的点
        {
            Vector2 pos = eventData.position;
            float dis = Vector2.Distance(lastPos, pos);

            CheckPoint(eventData.position);
            int segments = (int)(dis / radius);//计算出平滑的段数                                              
            segments = segments < 1 ? 1 : segments;
            if (segments >= 10) { segments = 10; }
            Vector2[] points = Beizier(penultPos, lastPos, pos, segments);//进行贝塞尔平滑
            for (int i = 0; i < points.Length; i++)
            {
                CheckPoint(points[i]);
            }
            lastPos = pos;
            if (points.Length > 2)
                penultPos = points[points.Length - 2];
        }
        else
        {
            twoPoints = true;
            lastPos = eventData.position;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isEndEraser) { return; }
        //CheckPoint(eventData.position);
        startDraw = false;
        twoPoints = false;
    }


    #endregion



    void CheckPoint(Vector3 pScreenPos)
    {
  
        Vector3 worldPos =DreamerTool.UI.Scene.UICamera.ScreenToWorldPoint(pScreenPos);
        Vector3 localPos = uiTex.gameObject.transform.InverseTransformPoint(worldPos);
        localPos.x *= mWidth/uiTex.rectTransform.sizeDelta.x ;
        localPos.y *= mHeight / uiTex.rectTransform.sizeDelta.y;
        if (localPos.x > -mWidth / 2 && localPos.x < mWidth / 2 && localPos.y > -mHeight / 2 && localPos.y < mHeight / 2)
        {
            for (int i = (int)localPos.x - brushSize; i < (int)localPos.x + brushSize; i++)
            {
                for (int j = (int)localPos.y - brushSize; j < (int)localPos.y + brushSize; j++)
                {
                    if (Mathf.Pow(i - localPos.x, 2) + Mathf.Pow(j - localPos.y, 2) > Mathf.Pow(brushSize, 2))
                        continue;
                    if (i < 0) { if (i < -mWidth / 2) { continue; } }
                    if (i > 0) { if (i > mWidth / 2) { continue; } }
                    if (j < 0) { if (j < -mHeight / 2) { continue; } }
                    if (j > 0) { if (j > mHeight / 2) { continue; } }

                    Color col = MyTex.GetPixel(i + (int)mWidth / 2, j + (int)mHeight / 2);
                    if (col.a != 0f)
                    {
                        col.a = 0.0f;
                        colorA++;
                        MyTex.SetPixel(i + (int)mWidth / 2, j + (int)mHeight / 2, col);
                    }
                }
            }


            //开始刮的时候 去判断进度
            if (!isStartEraser)
            {
                isStartEraser = true;
                InvokeRepeating("getTransparentPercent", 0f, 0.2f);
                if (eraserStartEvent != null)
                    eraserStartEvent.Invoke();
            }

            MyTex.Apply();
        }
    }



    double fate;


    /// <summary> 
    /// 检测当前刮刮卡 进度
    /// </summary>
    /// <returns></returns>
    public void getTransparentPercent()
    {
        if (isEndEraser) { return; }


        fate = colorA / maxColorA * 100;

        fate = (float)Math.Round(fate, 2);

        // Debug.LogError("当前百分比: " + fate);

        if (fate >= rate)
        {
            isEndEraser = true;
            CancelInvoke("getTransparentPercent");
            uiTex.DOFade(0, 0.5f).SetEase(Ease.Linear);

            //触发结束事件
            if (eraserEndEvent != null)
                eraserEndEvent.Invoke();

        }
    }

}