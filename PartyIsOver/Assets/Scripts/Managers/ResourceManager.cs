using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ResourceManager
{

    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }
        
    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");

        if( prefab == null )
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        return Object.Instantiate(prefab, parent);
    }

    public GameObject PhotonNetworkItemInstantiate(string path, Transform parent = null)
    {
        GameObject prefab = Load<GameObject>($"Item/{path}");

        if (prefab == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }
        
        return PhotonNetwork.Instantiate($"Item/{path}", Vector3.zero, Quaternion.identity);
        
    }

    public GameObject PhotonNetworkPlayerInstantiate(string path, Transform parent = null)
    {
        GameObject prefab = Load<GameObject>($"Player/{path}");

        if (prefab == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }
        return PhotonNetwork.Instantiate($"Player/{path}",Vector3.zero, Quaternion.identity);
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        Object.Destroy(go);
    }

}