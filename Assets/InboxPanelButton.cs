using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InboxPanelButton : MonoBehaviour
{
    [SerializeField]
    Button button;
    public Button Button => button;

    [SerializeField]
    TextMeshProUGUI buttonTMP;
    TextMeshProUGUI ButtonTMP => ButtonTMP;
    public string ButtonText
    {
        get
        {
            return buttonTMP.text;
        }
        set
        {
            buttonTMP.text = value;
        }
    }
}