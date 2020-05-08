using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudienceLoader : MonoBehaviour
{
    public CharacterScriptable characterScriptable;

    public Transform[] spawnPoints;

    void Start()
    {
        LoadAudience();
    }

    void LoadAudience()
    {
        int playerIndex = 0;
        for (int i = 0; i < spawnPoints.Length && i < characterScriptable.characterData.Length; i++)
        {
            if (playerIndex == PlayerData.SelectedAvatarIndex)
                playerIndex += 1;

            GameObject playerAvatar = Instantiate<GameObject>(characterScriptable.characterData[playerIndex].playerPrefab);
            playerAvatar.transform.SetParent(spawnPoints[i]);
            playerAvatar.transform.localPosition = Vector3.zero;
            playerAvatar.transform.localRotation = Quaternion.identity;
            playerAvatar.AddComponent<Audience>();
            playerIndex++;
        }
    }
}
