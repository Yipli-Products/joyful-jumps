using UnityEngine;


[CreateAssetMenu]
public class YipliConfig : ScriptableObject
{
    [HideInInspector]
    public string callbackLevel;

    [HideInInspector]
    public YipliPLayerInfo playerInfo;

    [HideInInspector]
    public YipliMatInfo matInfo;

    [HideInInspector]
    public string userId;
}
