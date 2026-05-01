using UnityEngine;
using TMPro;

public class SliderController : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI longueurTexte;
    [SerializeField] private TextMeshProUGUI graviteTexte;

    // Cette méthode sera appelée par le Slider de Longueur
    public void UpdateLongueurUI(float value)
    {
        longueurTexte.text = value.ToString("0");
    }

    // Cette méthode sera appelée par le Slider de Gravité
    public void UpdateGraviteUI(float value)
    {
        graviteTexte.text = value.ToString("0.0"); // "0.0" pour voir une décimale
    }

}
