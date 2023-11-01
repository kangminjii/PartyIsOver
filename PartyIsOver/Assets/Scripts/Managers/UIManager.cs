using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��� ����
// UI_Button ui = Managers.UI.ShowPopupUI<UI_Button>(); // UI��ư ����
// Managers.UI.ClosePopupUI(ui); // ������ ��ư ����

public class UIManager
{
    int _order = 10;

    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
    UI_Scene _sceneUI = null;

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };

            return root;
        }
    }


    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true; // ĵ���� �ȿ� ĵ������ ���� ��, �θ� ���� �����ϰ� �� sort���� ���ڴ�
        
        if(sort) // �˾� UI�� sorting��
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else // �˾��� �ƴ� UI�� sorting ����
        {
            canvas.sortingOrder = 0;
        }
    }


    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        // UI_Scene Prefab �ҷ�����
        GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");
        T sceneUI = Util.GetOrAddComponent<T>(go);
        _sceneUI = sceneUI;

        go.transform.SetParent(Root.transform);

        return sceneUI;
    }

    // <T> : ������ �˾� prefab , ���: Resources/UI/Popup/____
    public T ShowPopupUI<T> (string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        // UI_Button Prefab �ҷ�����
        GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");
        T popup = Util.GetOrAddComponent<T>(go);
        _popupStack.Push(popup);

        go.transform.SetParent(Root.transform);

        return popup;
    }


    public void ClosePopupUI(UI_Popup popup)
    {
        if (_popupStack.Count == 0)
            return;
        
        // �������� �˾�â�� �ƴ� ��
        if(_popupStack.Peek() != popup)
        {
            Debug.Log("Close popup Failed!");
            return;
        }

        ClosePopupUI();
    }

    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
            return;

        UI_Popup popup = _popupStack.Pop(); // ���� �ֱٿ� ��� �˾�â
        Managers.Resource.Destroy(popup.gameObject);
        popup = null;
        _order--;
    }

    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
            ClosePopupUI();
    }

}
