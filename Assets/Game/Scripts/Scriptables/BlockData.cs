using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BlockInfo
{
    public float length;
    public GameObject blockPrefab;
}

[CreateAssetMenu(fileName = "BlockData", menuName = "Godspeed/BlockData")]
public class BlockData : ScriptableObject
{
    [Header("Tutorial Blocks")]
    public GameObject[] tutorialBlocks;

    [Header("Starting Block")]
    public GameObject[] startBlocks;
    [Header("Ending Block")]
    public GameObject[] endBlocks;

    [Header("Easy Blocks")]
    public BlockInfo[] easyBlocks;
    [Header("Medium Blocks")]
    public BlockInfo[] mediumBlocks;
    [Header("Hard Blocks")]
    public BlockInfo[] hardBlocks;
}
