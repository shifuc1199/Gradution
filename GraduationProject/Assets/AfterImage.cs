using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;


[Serializable]
public struct AfterimageMatrix4x4
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 localScale;

    public Sprite sprite;
    public Material material;

    public AfterimageMatrix4x4(Vector3 position, Quaternion rotation, Vector3 localScale, Sprite sprite, Material material)
    {
        this.position = position;
        this.rotation = rotation;
        this.localScale = localScale;
        this.sprite = sprite;
        this.material = material;
    }


}

/// <summary>
/// 一个幻影的数据
/// </summary>
public class AfterimageData
{
    public List<AfterimageMatrix4x4> AM4x4List = new List<AfterimageMatrix4x4>();
    public Color color = Color.white;
    public float live;
}


/// <summary>
/// 基于SpriteRenderer的幻影插件，可以用于2D幻影
/// </summary>
public class AfterImage : MonoBehaviour
{

    List<SpriteRenderer> spriteList;



    private CommandBuffer buffer;

    private List<AfterimageData> AfterimageDataList = new List<AfterimageData>();

    private static int idMainTex = Shader.PropertyToID("_MainTex");

    private static int idColor = Shader.PropertyToID("_Color");


    private Vector3 lastPosition;

    public float spacing = 1;
    public float live = 0.2f;
    public Color color = Color.white;

    private void Awake()
    {
        SpriteRenderer[] collection = GetComponentsInChildren<SpriteRenderer>();
        spriteList = new List<SpriteRenderer>(collection);

        spriteList.Sort((x, y) =>
        {
            return x.sortingOrder - y.sortingOrder;
        });

    }

    private void Start()
    {
        lastPosition = transform.position;
    }



    private void Update()
    {

        for (int i = 0; i < AfterimageDataList.Count; i++)
        {
            AfterimageDataList[i].live -= Time.deltaTime;
            AfterimageDataList[i].color = new Color(AfterimageDataList[i].color.r, AfterimageDataList[i].color.g, AfterimageDataList[i].color.b, AfterimageDataList[i].live / live);
            if (AfterimageDataList[i].live < 0)
            {
                AfterimageDataList.RemoveAt(i);
                break;
            }

        }
 
        //数据刷新
        DataUptate();

        if (buffer != null)
        {
            Camera.main.RemoveCommandBuffer(CameraEvent.BeforeForwardAlpha, buffer);
        }



        //设置绘制数据
        DrawAfterimage(AfterimageDataList);

        if (buffer != null)
        {
            Camera.main.AddCommandBuffer(CameraEvent.BeforeForwardAlpha, buffer);
        }
    }

    /// <summary>
    /// 幻影数据刷新
    /// </summary>
    public bool IsUpdate = false;
    private void DataUptate()
    {

        if ((transform.position - lastPosition).magnitude < spacing)
        {
            return;
        }

        if (!IsUpdate)
            return;


        lastPosition = transform.position;


        AfterimageData afterimageData = new AfterimageData();
        //设置时间
        afterimageData.live = live;

        afterimageData.color = color;

        //遍历所有精灵创建一个幻影
        for (int i = 0; i < spriteList.Count; i++)
        {
            var item = spriteList[i];
           
            //获得当前精灵的信息
            AfterimageMatrix4x4 afterimageMatrix4X4 = new AfterimageMatrix4x4(
                item.transform.position,
                item.transform.rotation,
                item.transform.lossyScale,
                item.sprite,
                item.material);
            //添加当前精灵到幻影中
            afterimageData.AM4x4List.Add(afterimageMatrix4X4);


        }
        //将新的幻影添加到数据中
        AfterimageDataList.Add(afterimageData);

    }

    /// <summary>
    /// 绘制所有幻影
    /// </summary>
    /// <param name="afterimageDatas"></param>
    private void DrawAfterimage(List<AfterimageData> afterimageDatas)
    {
        buffer = new CommandBuffer();
        var propertyBlock = new MaterialPropertyBlock();


        for (int i = 0; i < afterimageDatas.Count; i++)
        {
            AfterimageData item = afterimageDatas[i];

            //绘制一张幻影
            //设置颜色
            propertyBlock.SetColor(idColor, item.color);
            //设置绘制数据
            DrawAfterimageItem(propertyBlock, item);
        }
    }
     
    /// <summary>
    /// 绘制一个幻影
    /// </summary>
    /// <param name="propertyBlock"></param>
    /// <param name="afterimageData"></param>
    private void DrawAfterimageItem(MaterialPropertyBlock propertyBlock, AfterimageData afterimageData)
    {
        for (int i = 0; i < afterimageData.AM4x4List.Count; i++)
        {
            AfterimageMatrix4x4 item = afterimageData.AM4x4List[i];
            if (item.sprite == null)
                return;
            //设置贴图

            propertyBlock.SetTexture(idMainTex, item.sprite.texture);
            //获得网格
            var mesh = SpriteToMesh(item.sprite);


            buffer.DrawMesh(mesh, Matrix4x4.TRS(item.position, item.rotation, item.localScale), item.material, 0, 0, propertyBlock);
        }
    }

    private Mesh SpriteToMesh(Sprite sprite)
    {
        var mesh = new Mesh();
        mesh.SetVertices(Array.ConvertAll(sprite.vertices, c => (Vector3)c).ToList());
        mesh.SetUVs(0, sprite.uv.ToList());
        mesh.SetTriangles(Array.ConvertAll(sprite.triangles, c => (int)c), 0);

        return mesh;
    }
}
 