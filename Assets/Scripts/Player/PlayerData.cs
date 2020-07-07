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


    public List<string> characters = new List<string>();    // Inventory of characters owned.
    public int usedCharacter;                               // Currently equipped character.
    public int usedAccessory = -1;
    public List<string> characterAccessories = new List<string>();  // List of owned accessories, in the form "charName:accessoryName".
    public List<string> themes = new List<string>();                // Owned themes.
    public int usedTheme;                                           // Currently used theme.

    public string previousName = "Trash Cat";

    public void AddCharacter(string name)
    {
        characters.Add(name);
    }

    static public void Create()
    {
        if (m_Instance == null)
        {
            m_Instance = new PlayerData();
        }

        m_Instance.saveFile = Application.persistentDataPath + "/save.bin";

        m_Instance.characters.Add("Player");
    }
}
