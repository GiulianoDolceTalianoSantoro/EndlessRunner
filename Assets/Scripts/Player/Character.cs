using UnityEngine;
using System;

public class Character : MonoBehaviour
{
    public string characterName;
    //public int cost;
    //public int premiumCost;

    //public CharacterAccessories[] accessories;

    public Animator animator;
    //public Sprite icon;

    //[Header("Sound")]
    //public AudioClip jumpSound;
    //public AudioClip hitSound;
    //public AudioClip deathSound;

    // Called by the game when an accessory changes, enable/disable the accessories children objects accordingly
    // a value of -1 as parameter disables all accessory.
    //public void SetupAccesory(int accessory)
    //{
    //    for (int i = 0; i < accessories.Length; ++i)
    //    {
    //        accessories[i].gameObject.SetActive(i == PlayerData.instance.usedAccessory);
    //    }
    //}
}
