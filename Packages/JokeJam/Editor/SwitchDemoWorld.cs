using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SwitchDemoWorld
{

    private const string USharpTag = "USharp";
    private const string UGraphTag = "UGraph";
    
    [MenuItem("JokeJam/Switch to Graph")]
    public static void SwitchToGraph()
    {
        foreach (var graphObject in FindObjectsWithTag(UGraphTag))
        {
            graphObject.SetActive(true);
        }
        
        foreach (var graphObject in FindObjectsWithTag(USharpTag))
        {
            graphObject.SetActive(false);
        }
    }

    [MenuItem("JokeJam/Switch to UdonSharp")]
    public static void SwitchToUSharp()
    {
        foreach (var graphObject in FindObjectsWithTag(USharpTag))
        {
            graphObject.SetActive(true);
        }
        
        foreach (var graphObject in FindObjectsWithTag(UGraphTag))
        {
            graphObject.SetActive(false);
        }
    }
    
    private static List<GameObject> FindObjectsWithTag(string tag)
    {
        List<GameObject> rootObjects = new List<GameObject>();
        List<GameObject> result = new List<GameObject>();
 
        var scene = EditorSceneManager.GetActiveScene();
        scene.GetRootGameObjects( rootObjects );
 
        // iterate root objects
        for (int i = 0; i < rootObjects.Count; ++i)
        {
            var objectsWithTag = FindObjectsWithTag(tag, rootObjects[i]);
 
            if (objectsWithTag != null)
            {
                result.AddRange(objectsWithTag);
            }
        }
 
        return result;
    }
 
    private static List<GameObject> FindObjectsWithTag(string tag, GameObject parent)
    {
 
        List<GameObject> result = new List<GameObject>();
        var children = parent.GetComponentsInChildren<Transform>(true);
   
        if (parent.CompareTag(tag))
        {
            result.Add(parent);
        }
   
        foreach (Transform child in children)
        {
            if (child.CompareTag(tag))
            {
                result.Add(child.gameObject);
            }
        }
 
        return result;
    }
}
