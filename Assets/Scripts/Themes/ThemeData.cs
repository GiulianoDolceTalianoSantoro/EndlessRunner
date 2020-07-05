using UnityEngine;
using UnityEngine.AddressableAssets;

[System.Serializable]
public struct ThemeZone
{
    public int length;
    public AssetReference[] prefabList;
}

/// <summary>
/// This is an asset which contains all the data for a theme.
/// As an asset it live in the project folder, and get built into an asset bundle.
/// </summary>
[CreateAssetMenu(fileName = "themeData", menuName = "Trash Dash/Theme Data")]
public class ThemeData : ScriptableObject
{
    [Header("Theme Data")]
    public string themeName;

    [Header("Objects")]
    public ThemeZone[] zones;
}
