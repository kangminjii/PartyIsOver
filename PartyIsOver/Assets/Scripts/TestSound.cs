using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSound : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public AudioClip audioClip;
    private void OnTriggerEnter(Collider other)
    {
        AudioSource audio = GetComponent<AudioSource>();
        //���� ��ġ�� ���� �Ҹ��� �Է�
        //audio.PlayClipAtPoint();

        /*AudioSource audio = GetComponent<AudioSource>();
        audio.PlayOneShot(audioClip);
        //������� �ΰ� ���� �� ���� ����� �ϰ� ������Ʈ�� ���� �Ѵ�.
        float lifeTime = Mathf.Max(audioClip.length, 0.5f);
        GameObject.Destroy(gameObject, lifeTime);*/

        Managers.Sound.Play(audioClip, Define.Sound.Bgm);

    }

}
