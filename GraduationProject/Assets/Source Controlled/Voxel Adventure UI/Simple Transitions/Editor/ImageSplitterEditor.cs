//using UnityEngine;
//using UnityEditor;
//using System.Collections;

//[CustomEditor(typeof(ImageSplitter))]

//public class ImageSplitterEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        ImageSplitter current = (ImageSplitter)target;

//        current.splitCount = EditorGUILayout.IntField(new GUIContent("Split Count", "How many times to split the given image. I.E 8 would become an 8 by 8 split and thus 64 pieces"), current.splitCount);
//        current.destination = EditorGUILayout.ObjectField(new GUIContent("Destination", "A placeholder end point for your animation"), current.destination, typeof(Transform), true) as Transform;
		
//        if (GUILayout.Button(new GUIContent("Split", "Split the image")))
//        {
//            GameObject parent = current.gameObject;
//            GameObject temp;
//            Transform endPoint = current.destination;
//            int splitCount = current.splitCount;
//            float boxSize = 1.0f / (float)splitCount;
//            Material material;
//            TransitionalObject transition = parent.GetComponent<TransitionalObject>();
//            TransitionalObject tempTransition;

//            DestroyImmediate(current);//destroy the image splitting script on the object But not the object itself

//            for (int i = 0; i < splitCount; i++)
//                for (int j = 0; j < splitCount; j++)
//                {
//                    temp = (GameObject)GameObject.Instantiate(parent);
//                    temp.name += "X" + i + " Y" + j;
//                    material = new Material(parent.GetComponent<Renderer>().sharedMaterial);//temp.renderer.sharedMaterial
//                    material.SetFloat("_PosX", i * boxSize);
//                    material.SetFloat("_PosY", j * boxSize);
//                    material.SetFloat("_Width", boxSize);

//                    temp.GetComponent<Renderer>().sharedMaterial = material;

//                    if (transition != null)
//                    {
//                        transition = temp.gameObject.GetComponent<TransitionalObject>();

//                        transition.startPoint = CopyTransform(temp.transform, "Transition Start ");
//                        transition.endPoint = endPoint.transform;
//                    }
//                }

//            DestroyImmediate(parent);
//        }
//    }

//    Transform CopyTransform(Transform copyFrom, string label)
//    {
//        GameObject returnedTransform = new GameObject(label + copyFrom.name);
//        returnedTransform.transform.position = copyFrom.transform.position;
//        returnedTransform.transform.localScale = copyFrom.transform.lossyScale;

//        if (copyFrom.transform.parent != null)
//            returnedTransform.transform.parent = copyFrom.transform.parent;

//        return returnedTransform.transform;
//    }
//}
