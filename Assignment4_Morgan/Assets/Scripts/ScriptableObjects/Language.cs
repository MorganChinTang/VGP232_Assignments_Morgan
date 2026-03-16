using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Language", menuName = "Scriptable Objects/Language")]
public class Language : ScriptableObject
{
    public List<LocData> locData;
}

[System.Serializable]
public class LocData
{
    public string locKey;
    public string en;
    public string fr;
    public string sp;
}
