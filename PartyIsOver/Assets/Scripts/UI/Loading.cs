using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class Loading : MonoBehaviour
{
    AsyncOperation async;


    void Start()
    {
        //StartCoroutine(LoadingNextScene(GameManager.Instance.nextSceneName));    
    }

    void Update()
    {
        DelayTime();
    }

    IEnumerator LoadingNextScene(string sceneName)
    {
        // LoadSceneAsync: �ٷ� ���� ��ȯ���� �ʰ�, ����ȭ�� �� �� ���� �ѱ��� ���� ������
        async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;

        while(async.progress < 0.9f) // 0~1
        {
            yield return true;
        }

        while(async.progress >= 0.9f)
        {
            yield return new WaitForSeconds(0.1f);
            if (delayTime > 5.0f) // 5���� delay�� �� ��
                break;
        }

        // ���� �ٲ���
        async.allowSceneActivation = true;
    }

    float delayTime = 0.0f;
    void DelayTime()
    {
        delayTime += Time.deltaTime;
        ImageHPBar.fillAmount = delayTime / 5;

    }

    // hp��
    public Image ImageHPBar = null;
}
