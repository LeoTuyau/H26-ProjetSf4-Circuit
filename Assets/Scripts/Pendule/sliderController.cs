using UnityEngine;
using TMPro; // Bibliothèque TextMeshPro pour les textes UI haute qualité


/// <summary>
/// Contrôle l'affichage des valeurs des sliders dans l'interface utilisateur.
/// Met à jour les labels de texte (longueur et gravité) en temps réel
/// lorsque l'utilisateur déplace les sliders correspondants.
/// </summary>
public class SliderController : MonoBehaviour
{

    [Header("Références UI - Textes des sliders")]

    // Référence au TextMeshPro affichant la valeur actuelle du slider de longueur
    // Assigné depuis l'Inspector Unity → objet "LongueurSliderTexte"
   [SerializeField] private TextMeshProUGUI longueurTexte;

    // Référence au TextMeshPro affichant la valeur actuelle du slider de gravité
    // Assigné depuis l'Inspector Unity → objet "GraviteSliderTexte"
    [SerializeField] private TextMeshProUGUI graviteTexte;

    /// <summary>
    /// Met à jour l'affichage de la longueur du pendule.
    /// Connectée à l'événement OnValueChanged du slider "LongueurSlider".
    /// </summary>
    /// <param name="value">Longueur du pendule en mètres (ex: 1 → affiche "1")</param>
    public void UpdateLongueurUI(float value)
    {
         // Formate la valeur sans décimales (ex: 1.7 → "2", 0.5 → "1")
        // car la longueur s'affiche en nombre entier dans la simulation
        longueurTexte.text = value.ToString("0");
    }

    /// <summary>
    /// Met à jour l'affichage de la gravité.
    /// Connectée à l'événement OnValueChanged du slider "GraviteSlider".
    /// </summary>
    /// <param name="value">Gravité en m/s² (ex: 9.8 → affiche "9.8")</param>
    public void UpdateGraviteUI(float value)
    {
        // Formate la valeur avec une décimale (ex: 9.8 → "9.8", 1.0 → "1.0")
        // pour refléter la précision physique de la gravité (m/s²)
        graviteTexte.text = value.ToString("0.0"); 
    }

}
