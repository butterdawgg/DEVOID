using System.Collections.Generic;
using UnityEngine;

public enum Action
{
    Pause = 0,
    Accelerate = 1,
    Decelerate = 2,
    RollLeft = 3,
    RollRight = 4,
    Shoot = 5
}

[System.Serializable]
public class Keybind
{
    public Action action;
    public KeyCode key;
}

public class InputManager : MonoBehaviour
{
    [SerializeField] private Keybind[] keybinds;
    [SerializeField] private float crosshairClampRadius;

    private static InputManager Instance { get; set; }
    private Dictionary<Action, KeyCode> keybindDict;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        BuildKeybindDictionary();
    }

    private void BuildKeybindDictionary()
    {
        keybindDict = new Dictionary<Action, KeyCode>();

        foreach (var bind in keybinds)
        {
            if (!keybindDict.ContainsKey(bind.action))
            {
                keybindDict.Add(bind.action, bind.key);
            }
            else
            {
                Debug.LogWarning($"Duplicate keybind for action: {bind.action}");
            }
        }
    }

    public static bool GetKey(Action action)
    {
        if (Instance == null || Instance.keybindDict == null)
        {
            Debug.LogError("InputManager not initialized.");
            return false;
        }

        if (Instance.keybindDict.TryGetValue(action, out KeyCode key))
        {
            return Input.GetKey(key);
        }

        Debug.LogWarning($"No keybind found for action: {action}");
        return false;
    }

    public static bool GetKeyDown(Action action)
    {
        if (Instance == null || Instance.keybindDict == null)
        {
            Debug.LogError("InputManager not initialized.");
            return false;
        }

        if (Instance.keybindDict.TryGetValue(action, out KeyCode key))
        {
            return Input.GetKeyDown(key);
        }

        Debug.LogWarning($"No keybind found for action: {action}");
        return false;
    }

    public static Vector2 GetCursorPosCenter()
    {
        return new Vector2(Input.mousePosition.x - Screen.width * 0.5f,
            Input.mousePosition.y - Screen.height * 0.5f);
    }

    public static Vector2 GetCrosshairPosCenter()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 screenCenter = new(Screen.width * 0.5f, Screen.height * 0.5f);

        float inputRadius =
            (Screen.width >= Screen.height ? Screen.height : Screen.width) *
            Instance.crosshairClampRadius * 0.5f;

        return Vector2.ClampMagnitude(mousePos - screenCenter, inputRadius);
    }

    public static Vector2 GetCrosshairPosCenterNormalized()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 screenCenter = new(Screen.width * 0.5f, Screen.height * 0.5f);

        float inputRadius =
            (Screen.width >= Screen.height ? Screen.height : Screen.width) *
            Instance.crosshairClampRadius * 0.5f;

        return Vector2.ClampMagnitude(mousePos - screenCenter, inputRadius) / inputRadius;
    }

    public static Vector2 GetCursorPosBottomLeft()
    {
        return new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }

    public static Vector2 GetCrosshairPosBottomLeft()
    {
        return GetCrosshairPosCenter() +
            new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
    }

    public static Vector2 GetCursorPosBottomLeftNDC()
    {
        Vector3 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        pos.x /= Screen.width;
        pos.y /= Screen.height;

        return pos;
    }

    public static Vector2 GetCrosshairPosBottomLeftNDC()
    {
        Vector3 pos = GetCrosshairPosBottomLeft();

        pos.x /= Screen.width;
        pos.y /= Screen.height;

        return pos;
    }
}