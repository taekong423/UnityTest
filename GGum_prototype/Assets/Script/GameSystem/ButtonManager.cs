﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EButtonCode
{
    MoveX,
    MoveY,
    Attack,
    Jump,
    Test,
}

[System.Serializable]
public class ButtonData
{
    public delegate float AxisFunction();
    public delegate bool ButtonFunction();

    [Header("Set Button Code Or Name")]
    public EButtonCode _buttonCode;
    public string _buttonName;

    [Header("Key Mapping")]
    List<AxisFunction> _KeyAxis;
    List<AxisFunction> _KeyAxisRaw;

    List<ButtonFunction> _buttonDowns;
    List<ButtonFunction> _buttonPress;

    public List<KeyCode> _keyCodes;
    public List<string> _keyNames;

    public void RegisterDown(ButtonFunction func)
    {
        if (_buttonDowns == null)
            _buttonDowns = new List<ButtonFunction>();

        _buttonDowns.Add(func);
    }

    public void RegisterPress(ButtonFunction func)
    {
        if (_buttonPress == null)
            _buttonPress = new List<ButtonFunction>();

        _buttonPress.Add(func);
    }

    public void RegisterAxis(AxisFunction func)
    {
        if (_KeyAxis == null)
            _KeyAxis = new List<AxisFunction>();

        _KeyAxis.Add(func);
    }

    public void RegisterAxisRaw(AxisFunction func)
    {

        if (_KeyAxisRaw == null)
            _KeyAxisRaw = new List<AxisFunction>();

        _KeyAxisRaw.Add(func);
    }

    //EButtonCode - Down, Press, Up

    public bool GetButtonDown()
    {
        if (_buttonDowns == null || _keyCodes.Count <= 0 && _keyNames.Count <= 0 && _buttonDowns.Count <= 0)
            return false;

        for (int i = 0; i < _keyCodes.Count; i++)
        {
            if (Input.GetKeyDown(_keyCodes[i]))
            {
                return true;
            }
        }

        for (int i = 0; i < _keyNames.Count; i++)
        {
            if (Input.GetKeyDown(_keyNames[i]))
            {
                return true;
            }
        }


        for (int i = 0; i < _buttonDowns.Count; i++)
        {
            if (_buttonDowns[i].Invoke())
            {

                return true;
            }
        }

        return false;

    }

    public bool GetButtonPress()
    {
        if (_buttonPress == null || _keyCodes.Count <= 0 && _keyNames.Count <= 0 && _buttonPress.Count <= 0)
            return false;

        for (int i = 0; i < _keyCodes.Count; i++)
        {
            if (Input.GetKey(_keyCodes[i]))
            {
                return true;
            }
        }

        for (int i = 0; i < _keyNames.Count; i++)
        {
            if (Input.GetKey(_keyNames[i]))
            {
                return true;
            }
        }

        for (int i = 0; i < _buttonPress.Count; i++)
        {
            if (_buttonPress[i].Invoke())
            {

                return true;
            }
        }

        return false;

    }

    public bool GetButtonUp()
    {
        if (_keyCodes.Count <= 0 && _keyNames.Count <= 0)
            return false;

        for (int i = 0; i < _keyCodes.Count; i++)
        {
            if (Input.GetKeyUp(_keyCodes[i]))
            {
                return true;
            }
        }

        for (int i = 0; i < _keyNames.Count; i++)
        {
            if (Input.GetKeyUp(_keyNames[i]))
            {
                return true;
            }
        }


        return false;

    }

    //Axis

    public float GetAxis()
    {
        if (_keyNames.Count <= 0)
            return 0.0f;

        for (int i = 0; i < _keyNames.Count; i++)
        {
            if (Input.GetAxis(_keyNames[i]) != 0.0f)
            {
                return Input.GetAxis(_keyNames[i]);
            }
        }

        return 0.0f;
    }

    public float GetAxisFunction()
    {
        if (_KeyAxis == null || _KeyAxis.Count <= 0)
            return 0.0f;

        for (int i = 0; i < _KeyAxis.Count; i++)
        {
            if (_KeyAxis[i].Invoke() != 0.0f)
            {
                return _KeyAxis[i].Invoke();
            }
        }

        return 0.0f;
    }

    //AxisRaw

    public float GetAxisRaw()
    {
        if (_keyNames.Count <= 0)
            return 0;

        for (int i = 0; i < _keyNames.Count; i++)
        {
            if (Input.GetAxisRaw(_keyNames[i]) != 0.0f)
            {
                return Input.GetAxisRaw(_keyNames[i]);
            }
        }

        return 0;
    }

    public float GetAxisRawFunction()
    {
        if (_KeyAxisRaw == null || _KeyAxisRaw.Count <= 0)
            return 0.0f;

        for (int i = 0; i < _KeyAxisRaw.Count; i++)
        {
            if (_KeyAxisRaw[i].Invoke() != 0.0f)
            {
                return _KeyAxisRaw[i].Invoke();
            }
        }

        return 0.0f;
    }

}

public class ButtonManager : MonoBehaviour {

    static ButtonManager _instance;

    public ButtonData[] _buttonDatas;


    public ButtonManager()
    {
        _instance = this;
    }

    public static ButtonManager This
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("ButtonManager");
                _instance = obj.AddComponent<ButtonManager>();
            }

            return _instance;
        }
    }

    ButtonData GetButtonData(EButtonCode buttonCode)
    {
        if (_buttonDatas.Length <= 0)
            return null;

        for (int i = 0; i < _buttonDatas.Length; i++)
        {
            if (_buttonDatas[i]._buttonCode == buttonCode)
            {
                return _buttonDatas[i];
            }
        }

        return null;
    }

    ButtonData GetButtonData(string buttonName)
    {
        if (_buttonDatas.Length <= 0)
            return null;

        for (int i = 0; i < _buttonDatas.Length; i++)
        {
            if (_buttonDatas[i]._buttonName == buttonName)
            {
                return _buttonDatas[i];
            }
        }

        return null;
    }

    public void RegisterDown(EButtonCode buttonCode, ButtonData.ButtonFunction func)
    {

        ButtonData data = GetButtonData(buttonCode);

        if (data == null)
        {
            Debug.Log("ButtonData == null");
            return;
        }

        data.RegisterDown(func);
    }

    public void RegisterPress(EButtonCode buttonCode, ButtonData.ButtonFunction func)
    {

        ButtonData data = GetButtonData(buttonCode);

        if (data == null)
        {
            Debug.Log("ButtonData == null");
            return;
        }

        data.RegisterPress(func);
    }

    public void RegisterAxis(EButtonCode buttonCode, ButtonData.AxisFunction func)
    {
        ButtonData data = GetButtonData(buttonCode);

        if (data == null)
        {
            Debug.Log("ButtonData == null");
            return;
        }

        data.RegisterAxis(func);
    }

    public void RegisterAxisRaw(EButtonCode buttonCode, ButtonData.AxisFunction func)
    {
        ButtonData data = GetButtonData(buttonCode);

        if (data == null)
        {
            Debug.Log("ButtonData == null");
            return;
        }

        data.RegisterAxisRaw(func);
    }

    //EButtonCode - Down, Press, Up

    public bool GetButtonDown(EButtonCode buttonCode)
    {
        ButtonData data = GetButtonData(buttonCode);

        if (data == null)
        {
            Debug.Log("ButtonData == null");
            return false;
        }

        return data.GetButtonDown();
    }

    public bool GetButtonPress(EButtonCode buttonCode)
    {
        ButtonData data = GetButtonData(buttonCode);

        if (data == null)
        {
            Debug.Log("ButtonData == null");
            return false;
        }

        return data.GetButtonPress();
    }

    public bool GetButtonUp(EButtonCode buttonCode)
    {
        ButtonData data = GetButtonData(buttonCode);

        if (data == null)
        {
            Debug.Log("ButtonData == null");
            return false;
        }

        return data.GetButtonUp();
    }

    //String - Down, Press, Up

    public bool GetButtonDown(string buttonName)
    {
        ButtonData data = GetButtonData(buttonName);

        if (data == null)
        {
            Debug.Log("ButtonData == null");
            return false;
        }

        return data.GetButtonDown();
    }

    public bool GetButtonPress(string buttonName)
    {
        ButtonData data = GetButtonData(buttonName);

        if (data == null)
        {
            Debug.Log("ButtonData == null");
            return false;
        }

        return data.GetButtonPress();
    }

    public bool GetButtonUp(string buttonName)
    {
        ButtonData data = GetButtonData(buttonName);

        if (data == null)
        {
            Debug.Log("ButtonData == null");
            return false;
        }

        return data.GetButtonUp();
    }

    //Axis

    public float GetButtonAxis(EButtonCode buttonCode)
    {
        ButtonData data = GetButtonData(buttonCode);

        if (data == null)
        {
            Debug.Log("ButtonData == null");
            return 0.0f;
        }

        if (data.GetAxisFunction() != 0.0f)
            return data.GetAxisFunction();
        else
            return data.GetAxis();
    }

    //AxisRaw

    public float GetButtonAxisRaw(EButtonCode buttonCode)
    {
        ButtonData data = GetButtonData(buttonCode);

        if (data == null)
        {
            Debug.Log("ButtonData == null");
            return 0.0f;
        }

        if (data.GetAxisRawFunction() != 0.0f)
            return data.GetAxisRawFunction();
        else
            return data.GetAxisRaw();
    }

}
