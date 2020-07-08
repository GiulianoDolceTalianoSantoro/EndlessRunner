using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerData
{
    static protected PlayerData m_Instance;
    static public PlayerData instance { get { return m_Instance; } }

    protected string saveFile = "";


    public List<string> characters = new List<string>();
    public int usedCharacter;

    public void AddCharacter(string name)
    {
        characters.Add(name);
    }

    static public void Create()
    {
        if (m_Instance == null)
        {
            m_Instance = new PlayerData();

            CoroutineHandler.StartStaticCoroutine(CharacterDatabase.LoadDatabase());
        }

        m_Instance.saveFile = Application.persistentDataPath + "/save.bin";

        m_Instance.characters.Add("Cube");
    }
}
