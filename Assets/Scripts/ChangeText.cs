using TMPro;
using UnityEngine;

public class ChangeText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI myText;

    public void SetText(float charge)
    {
        myText.text = charge.ToString()+"C";
    }
}