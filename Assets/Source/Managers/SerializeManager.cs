using System;
using UnityEngine;

public static class SerializeManager
{
    private static float GetFloat(string key, float defaultValue)
    {
        if (PlayerPrefs.HasKey(key))
            return PlayerPrefs.GetFloat(key);
        else
            return defaultValue;
    }

    private static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }

    private static int GetInt(string key, int defaultValue)
    {
        if (PlayerPrefs.HasKey(key))
            return PlayerPrefs.GetInt(key);
        else
            return defaultValue;
    }

    private static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    private static bool GetBool(string key, bool defaultValue)
    {
        if (PlayerPrefs.HasKey(key))
            return Convert.ToBoolean(PlayerPrefs.GetInt(key));
        else
            return defaultValue;
    }

    private static void SetBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, Convert.ToInt32(value));
    }

    private static Vector3 GetVector3(string key, Vector3 defaultValue)
    {
        if (!PlayerPrefs.HasKey(key))
            return defaultValue;

        return new Vector3(PlayerPrefs.GetFloat(key),
            PlayerPrefs.GetFloat(key + "_1"),
            PlayerPrefs.GetFloat(key + "_2"));
    }

    private static void SetVector3(string key, Vector3 value)
    {
        PlayerPrefs.SetFloat(key, value.x);
        PlayerPrefs.SetFloat(key + "_1", value.y);
        PlayerPrefs.SetFloat(key + "_2", value.z);
    }

    public static float GetMasterVolume()
    {
        return GetFloat("master_volume", 0.5f);
    }

    public static void SetMasterVolume(float value)
    {
        SetFloat("master_volume", value);
    }

    public static float GetSFXVolume()
    {
        return GetFloat("sfx_volume", 0.5f);
    }

    public static void SetSFXVolume(float value)
    {
        SetFloat("sfx_volume", value);
    }

    public static float GetMusicVolume()
    {
        return GetFloat("music_volume", 0.5f);
    }

    public static void SetMusicVolume(float value)
    {
        SetFloat("music_volume", value);
    }

    public static bool GetIsFirstPlay()
    {
        return GetBool("first_play", true);
    }

    public static void SetIsFirstPlay(bool value)
    {
        SetBool("first_play", value);
    }

    public static int GetPlayerClassID()
    {
        return GetInt("player_class_id", 0);
    }

    public static void SetPlayerClassID(int value)
    {
        SetInt("player_class_id", value);
    }

    public static int GetHighScore()
    {
        return GetInt("high_score", 0);
    }

    public static void SetHighScore(int value)
    {
        SetInt("high_score", value);
    }
}