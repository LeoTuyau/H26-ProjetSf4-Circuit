using UnityEngine;
using UnityEngine.SceneManagement;// Nécessaire pour naviguer entre les scènes

/// <summary>
/// Gère le menu pause du simulateur de pendule.
/// Permet à l'utilisateur de mettre la simulation en pause, de la reprendre,
/// de la redémarrer ou de retourner au menu principal.
/// </summary>
public class PauseMenu : MonoBehaviour
{

// Référence au GameObject du panneau pause (PauseMenuCanvas)
    // Assigné depuis l'Inspector — affiché/caché selon l'état de la simulation
    [SerializeField] GameObject pauseMenu;


    /// <summary>
    /// Met la simulation en pause.
    /// Connectée au bouton "Pause" de l'interface.
    /// </summary>
    public void Pause() {

      // Affiche le panneau du menu pause
       pauseMenu.SetActive(true);

        // Gèle le temps Unity → stoppe la physique du pendule et toutes les animations
        // (Time.deltaTime retournera 0 tant que timeScale est à 0)
       Time.timeScale = 0;
    }


    /// <summary>
    /// Retourne au menu principal.
    /// Connectée au bouton "Home" du menu pause.
    /// </summary>
    public void Home() {

      // Charge la scène du menu principal par son nom
         SceneManager.LoadScene("Menu Principal");

         // Réinitialise le temps avant de quitter — indispensable car si on
        // revient à cette scène sans reset, le jeu resterait gelé (timeScale = 0)
         Time.timeScale = 1;
    }

    /// <summary>
    /// Reprend la simulation depuis l'état actuel du pendule.
    /// Connectée au bouton "Resume" du menu pause.
    /// </summary>
    public void Resume() {

      // Cache le panneau du menu pause
    pauseMenu.SetActive(false);

     // Réactive le temps Unity → le pendule reprend son mouvement
    Time.timeScale = 1;

    }


    /// <summary>
    /// Redémarre la simulation en rechargeant la scène courante.
    /// Remet le pendule à son angle initial (45°) et réinitialise tous les sliders.
    /// Connectée au bouton "Restart" du menu pause.
    /// </summary>
    public void Restart() {

      // Recharge la scène active via son index dans les Build Settings
        // (plus robuste que de passer le nom, fonctionne même si la scène est renommée)
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

      // Réinitialise le temps avant le rechargement pour éviter
        // que la scène redémarre avec le temps gelé
      Time.timeScale = 1;
    }


}
