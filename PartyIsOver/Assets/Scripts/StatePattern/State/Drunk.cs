using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Drunk : MonoBehaviourPun, IDebuffState
{
    public Actor MyActor { get; set; }
    public float CoolTime { get; set; }
    public GameObject effectObject { get; set; }
    public Transform playerTransform { get; set; }
    GameObject drunkEffectObject;
    AudioClip _audioClip = null;
    AudioSource _audioSource;
    public void EnterState()
    {
        drunkEffectObject = Managers.Resource.PhotonNetworkInstantiate("Flamethrower");
        effectObject = null;
        playerTransform = this.transform.Find("GreenHip").GetComponent<Transform>();
        Transform SoundSourceTransform = transform.Find("GreenHip");
        _audioSource = SoundSourceTransform.GetComponent<AudioSource>();

        PlayerDebuffSound("PlayerEffect/Cartoon-UI-049");
        InstantiateEffect("Effects/Fog_poison");

    }

    public void UpdateState()
    {
        if (effectObject != null)
        {
            effectObject.transform.position = playerTransform.position;
        }
        if(drunkEffectObject != null)
        {
            drunkEffectObject.transform.position = playerTransform.position + playerTransform.forward;
            drunkEffectObject.transform.rotation = Quaternion.LookRotation(-playerTransform.right);
        }
        if(MyActor.PlayerController.isDrunk)
        {
            MyActor.BodyHandler.Head.PartRigidbody.AddForce(-MyActor.BodyHandler.Hip.PartTransform.up * 100f);
            MyActor.BodyHandler.Head.PartRigidbody.AddForce(-MyActor.BodyHandler.Hip.PartTransform.forward * 30f);
        }
        else
        {
            MyActor.PlayerController.IsFlambe = false;
            RemoveObject("Flamethrower");
        }

    }

    public void ExitState()
    {
        RemoveObject("Fog_poison");
        MyActor.PlayerController.isDrunk = false;

        MyActor.debuffState = Actor.DebuffState.Default;
        _audioClip = null;
    }
    public void InstantiateEffect(string path)
    {
        effectObject = Managers.Resource.PhotonNetworkInstantiate($"{path}");
    }
    public void RemoveObject(string name)
    {
        GameObject go = GameObject.Find($"{name}");
        Managers.Resource.Destroy(go);
        effectObject = null;
    }
    void PlayerDebuffSound(string path)
    {
        //���� ���� ����
        _audioClip = Managers.Sound.GetOrAddAudioClip(path);
        _audioSource.clip = _audioClip;
        _audioSource.volume = 0.2f;
        _audioSource.spatialBlend = 1;
        Managers.Sound.Play(_audioClip, Define.Sound.PlayerEffect, _audioSource);
    }
}
