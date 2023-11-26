using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using static UnityEngine.UI.Image;

public class EffectManager
{
/*    ParticleSystem[] _
    Dictionary<string, ParticleSystem> _particleSystem = new Dictionary<string, ParticleSystem>();

    public void Clear()
    {
        foreach (ParticleSystem particleSystem in _particleSystem)
        {
            particleSystem.Stop();
        }
        _particleSystem.Clear();
    }*/

    public GameObject Instantiate(ParticleSystem path, Vector3 pos, Quaternion rotation , Transform parent = null)
    {
        GameObject prefab = Managers.Resource.Load<GameObject>($"{path}");

        if (prefab == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        return Object.Instantiate(prefab, pos, rotation);
    }


    //���濡�� �߰��� Instantiate�� �� �� �ִ��� Ȯ�� �غ�����
    public GameObject PhotonNetworkEffectInstantiate(string path, Transform parent = null, Vector3? pos = null, Quaternion? rot = null, byte group = 0, object[] data = null)
    {
        GameObject prefab = Managers.Resource.Load<GameObject>($"Effects/{path}");
        pos = pos ?? Vector3.zero;
        rot = rot ?? Quaternion.identity;

        if (prefab == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        return PhotonNetwork.Instantiate($"Effects/{path}", (Vector3)pos, (Quaternion)rot, group, data);
    }

    public void Play(string path, Vector3 pos, Vector3 normal, Transform parent = null, Define.Effect effect = Define.Effect.PlayerEffect)
    {
        //�⺻�����δ� �÷��̾ �峪���� ����Ʈ�� ���� ����ϹǷ� �ϴ� �����
        var targetPrefab = Managers.Resource.Load<ParticleSystem>(path);

        if (effect == Define.Effect.UIEffect)
        {
            targetPrefab = Managers.Resource.Load<ParticleSystem>(path);
        }

        var instantiatedObject = Instantiate(targetPrefab, pos, Quaternion.LookRotation(normal));

        ParticleSystem playEffect = instantiatedObject.GetComponent<ParticleSystem>();

        if (parent != null) playEffect.transform.SetParent(parent);

        playEffect.Play();
    }

    public void Play(string path, Vector3 pos, Vector3 normal, Transform parent = null)
    {
        //����
        GameObject instantiatedObject = Managers.Resource.Instantiate($"Effects/{path}");

        Transform objectTransform = instantiatedObject.transform;

        //�����̼��̶� ȸ�� �� �־��ֱ�
        objectTransform.position = pos;
        objectTransform.rotation = Quaternion.LookRotation(normal);
    }



}
