using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TestDebug: MonoBehaviour
{
    public static Text debugText;

    private void Awake()
    {
        debugText = FindObjectOfType<GameManager>().debugText;
    }

    public static void Debugging(string message)
    {
        debugText.text = message;
    }
}
