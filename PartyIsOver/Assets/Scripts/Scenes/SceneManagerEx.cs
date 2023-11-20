using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx 
{
    public BaseScene CurrentScene{ get { return GameObject.FindObjectOfType<BaseScene>(); } }

    public void LoadScene(Define.Scene type)
    {
        //���� ����ߴ� ���� �����ְ� ���� ������ �̵�
        Managers.Clear();
        SceneManager.LoadScene(GetSceneName(type));
    }

    //Define�� �ִ� ���ڿ��� �̾Ƴ��� ����̴�. LoadScene�� ���ڿ��� �޾ƾ��ϱ� ������ �̷��� ��� ���
    public string GetSceneName(Define.Scene type)
    {
        string name = System.Enum.GetName(typeof(Define.Scene),type);
        return name;
    }

    public string GetCurrentSceneName()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        return currentScene.name;
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }

}
