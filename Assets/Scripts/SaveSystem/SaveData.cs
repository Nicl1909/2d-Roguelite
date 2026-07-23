using System;
using UnityEngine;
using System.IO;
using System.Text.Json;
using NUnit.Framework;
[Serializable]
public class SaveData
{
    public string characterName = "Timon";
    public int characterLevel = 3;
    public float characterHealth = 10;
    public float characterMana = 10;
    public int characterExperience = 0;
    public string characterClass = "Unclassified";
}

