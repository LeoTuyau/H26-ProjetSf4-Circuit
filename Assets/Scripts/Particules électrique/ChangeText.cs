using TMPro;
using UnityEngine;

/// <summary>
/// ChangeText est un composant responsable de la mise à jour
/// d’un texte UI affichant une charge électrique.
/// </summary>
public class ChangeText : MonoBehaviour
{
    /// <summary>
    /// Référence vers le composant TextMeshProUGUI affiché à l’écran.
    /// Ce texte est mis à jour pour afficher la valeur de charge.
    /// </summary>
    [SerializeField] private TextMeshProUGUI myText;

    /// <summary>
    /// Met à jour le texte affiché avec une valeur de charge.
    /// </summary>
    /// <param name="charge">
    /// Valeur de la charge électrique à afficher (en Coulombs).
    /// </param>
    public void SetText(float charge)
    {
        myText.text = charge.ToString() + "C";
    }
}