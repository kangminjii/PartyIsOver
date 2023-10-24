using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    public Action KeyAction = null;
    public Action<Define.MouseEvent> MouseAction = null;
    public Action<Define.KeyboardEvent> KeyboardAction = null;

    bool _pressed = false;
    float _pressedTime = 0;

    bool _keyPressed = false;
    float _keyPressedTime = 0;

    bool _rkeyPressed = false;
    float _rkeyPressedTime = 0;

    float _chargeTime = 0;
    bool _isCharge = false;
    float _chargeThreshold = 1.0f;


    public void OnUpdate()
    {
        // UI�� Ŭ���� ���� ĳ���Ͱ� �������� �ʰ� ��
        if (EventSystem.current.IsPointerOverGameObject()) 
            return;

        if (Input.anyKey && KeyAction != null)
            KeyAction.Invoke();

        if (KeyboardAction != null)
        {
            if (Input.GetKey(KeyCode.H))
            {
                if (!_keyPressed)
                {
                    KeyboardAction.Invoke(Define.KeyboardEvent.PointerDown);
                    _keyPressedTime = Time.time;
                }
                KeyboardAction.Invoke(Define.KeyboardEvent.Press);
                _keyPressed = true;
            }
            else 
            {
                if ((_keyPressed))
                {
                    if (Time.time < _keyPressedTime + 0.2f)
                        KeyboardAction.Invoke(Define.KeyboardEvent.Click);
                    KeyboardAction.Invoke(Define.KeyboardEvent.PointerUp);
                }
                _keyPressed = false;
                _keyPressedTime = 0;
            }
            
        }

        if (KeyboardAction != null)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (!_rkeyPressed)
                {
                    KeyboardAction.Invoke(Define.KeyboardEvent.PointerDown);
                    _rkeyPressedTime = Time.time;
                }
                _rkeyPressed = true;
                _chargeTime = 0;
                _isCharge = true;
            }
            else if (Input.GetKey(KeyCode.R))
            {
                if (_isCharge)
                {
                    _chargeTime += Time.deltaTime;
                    if (_chargeTime > _chargeThreshold)
                        KeyboardAction.Invoke(Define.KeyboardEvent.Hold);
                }
            }
            else if (Input.GetKeyUp(KeyCode.R))
            {
                if (_rkeyPressed)
                {
                    if (Time.time < _rkeyPressedTime + 0.2f)
                        KeyboardAction.Invoke(Define.KeyboardEvent.Click);
                    KeyboardAction.Invoke(Define.KeyboardEvent.Charge);
                }
                _rkeyPressed = false;
                _isCharge = false;
            }
        }

        if (MouseAction != null)
        {
            if (Input.GetMouseButton(0))
            {
                if (!_pressed)
                {
                    MouseAction.Invoke(Define.MouseEvent.PointerDown);
                    _rkeyPressedTime = Time.time;
                }
                MouseAction.Invoke(Define.MouseEvent.Press);
                _pressed = true;
            }
            else 
            {
                if (_pressed)
                {
                    if (Time.time < _rkeyPressedTime + 0.2f)
                        MouseAction.Invoke(Define.MouseEvent.Click);
                    MouseAction.Invoke(Define.MouseEvent.PointerUp);
                }
                _pressed = false;
                _rkeyPressedTime = 0;
            }
        }
    }

    public void Clear()
    {
        KeyAction = null;
        MouseAction = null;
        KeyboardAction = null;
    }

}