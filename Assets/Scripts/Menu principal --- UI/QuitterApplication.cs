using UnityEngine;


/// <summary>
/// Gère la fermeture de l'application depuis le menu principal.
/// </summary>
public class QuitterApplication : MonoBehaviour
{

    /// <summary>
    /// Ferme complètement l'application.
    /// Connectée au bouton "Quitter" du menu principal.
    /// </summary>
    public void Quit()
    {

        // Ferme l'application proprement en libérant les ressources
        // Note : n'a aucun effet dans l'éditeur Unity — pour tester,
        // il faut construire et lancer le build (File → Build & Run)
        Application.Quit();

        // Log de débogage pour confirmer que le bouton fonctionne dans l'éditeur
        // (puisque Application.Quit() est ignoré en mode Play dans l'éditeur)
        Debug.Log("Quit game button works!"); 
    }
}
