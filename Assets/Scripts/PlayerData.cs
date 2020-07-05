using UnityEngine;
using System.IO;
using System.Collections.Generic;
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

    static public void Create()
    {
        if (m_Instance == null)
        {
            m_Instance = new PlayerData();

            //if we create the PlayerData, mean it's the very first call, so we use that to init the database
            //this allow to always init the database at the earlier we can, i.e. the start screen if started normally on device
            //or the Loadout screen if testing in editor
            //CoroutineHandler.StartStaticCoroutine(CharacterDatabase.LoadDatabase());
            //CoroutineHandler.StartStaticCoroutine(ThemeDatabase.LoadDatabase());
        }

        m_Instance.saveFile = Application.persistentDataPath + "/save.bin";

        //if (File.Exists(m_Instance.saveFile))
        //{
        //    // If we have a save, we read it.
        //    m_Instance.Read();
        //}
        //else
        //{
        //    // If not we create one with default data.
        //    NewSave();
        //}

        //m_Instance.CheckMissionsCount();
    }
}
