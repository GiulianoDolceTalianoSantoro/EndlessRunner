using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine;

public class CharacterDatabase
{
    static protected Dictionary<string, Character> m_CharactersDict;

    static public Dictionary<string, Character> dictionary { get { return m_CharactersDict; } }

    static protected bool m_Loaded = false;
    static public bool loaded { get { return m_Loaded; } }

    static public Character GetCharacter(string type)
    {
        Character c;

        var b = m_CharactersDict.TryGetValue(type, out c);
        //TestDebug.Debugging("getting character of type: " + type 
        //    + " and getting value with result of " + b + "\n because dict has " + m_CharactersDict.Count
        //    + " items with value" + m_CharactersDict.Keys );

        if (m_CharactersDict == null || !b)
            return null;

        return c;
    }

    static public IEnumerator LoadDatabase()
    {
        if (m_CharactersDict == null)
        {
            m_CharactersDict = new Dictionary<string, Character>();

            yield return Addressables.LoadAssetsAsync<GameObject>("default", op =>
            {
                Character c = op.GetComponent<Character>();

                /* This message displays on Editor but not in Android apk, so it means the issue occurs 
                before loading this database, and, as the GameManager with its text doesn't exists yet,
                we can't use TestDebug helper*/
                TestDebug.Debugging("assets loaded, " + c.gameObject.name);

                if (c != null)
                {
                    m_CharactersDict.Add(c.characterName, c);
                }
            });

            m_Loaded = true;
        }
    }
}
