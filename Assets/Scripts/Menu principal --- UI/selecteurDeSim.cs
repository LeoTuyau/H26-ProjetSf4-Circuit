using UnityEngine;
using UnityEngine.SceneManagement; // Nécessaire pour naviguer entre les scènes

/// <summary>
/// Gère la navigation vers les différentes simulations physiques
/// depuis le menu principal.
/// Un seul script réutilisable pour tous les boutons de sélection,
/// chaque bouton passant simplement le nom de sa scène en paramètre.
/// </summary>
public class selecteurDeSim : MonoBehaviour
{
    /// <summary>
    /// Charge la scène de simulation correspondant au bouton cliqué.
    /// Connectée au bouton de chaque simulation dans le menu principal
    /// — le nom de la scène est passé directement depuis le champ
    /// OnClick() de l'Inspector Unity.
    /// </summary>
    /// <param name="nomDeLaScene">
    /// Nom exact de la scène à charger tel qu'il apparaît
    /// dans File → Build Settings (ex: "Pendule", "Boussole et aimant")
    /// </param>
    public void selectSim(string nomDeLaScene)
    {
        // Charge la scène demandée par son nom
        // Le nom doit correspondre exactement à celui dans les Build Settings,
        // sinon Unity lève une erreur et la scène ne se charge pas
        SceneManager.LoadScene(nomDeLaScene);
    }
}