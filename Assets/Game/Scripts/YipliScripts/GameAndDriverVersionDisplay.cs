using UnityEngine;
using TMPro;

public class GameAndDriverVersionDisplay : MonoBehaviour
{
    public TextMeshProUGUI gameAndDriverVersonText;

    // Start is called before the first frame update
    void Start()
    {
        gameAndDriverVersonText.text = PlayerSession.Instance.GetDriverAndGameVersion();
    }

    // Update is called once per frame
    void Update()
    {
        gameAndDriverVersonText.text = PlayerSession.Instance.GetDriverAndGameVersion();
    }
}
