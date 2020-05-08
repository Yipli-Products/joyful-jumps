using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CharacterData
{
    public GameObject playerPrefab;
    public RenderTexture renderTexture;
}

[CreateAssetMenu(fileName = "CharacterScriptable", menuName = "Godspeed/CharacterScriptable")]
public class CharacterScriptable : ScriptableObject
{
    public CharacterData[] characterData;
}
