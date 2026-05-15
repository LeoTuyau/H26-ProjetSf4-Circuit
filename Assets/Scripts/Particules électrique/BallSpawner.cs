using UnityEngine;

/// <summary>
/// BallSpawner est responsable de la création des balles dans la scène
/// ainsi que de leur initialisation dans le système de simulation.
/// Il crée également un texte UI associé à chaque balle pour la suivre.
/// </summary>
public class BallSpawner : MonoBehaviour
{
    /// <summary>
    /// Prefab de la balle physique à instancier dans la scène.
    /// </summary>
    [SerializeField] GameObject prefab;

    /// <summary>
    /// Prefab du texte UI (ou objet visuel) qui suit la balle.
    /// </summary>
    [SerializeField] GameObject textPrefab;

    /// <summary>
    /// Référence vers le manager principal (ForceManager)
    /// qui gère la logique physique des balles.
    /// </summary>
    [SerializeField] GameObject manager;

    /// <summary>
    /// Appelé une seule fois au démarrage du script.
    /// Actuellement inutilisé.
    /// </summary>
    void Start()
    {
    }

    /// <summary>
    /// Appelé à chaque frame.
    /// Actuellement inutilisé.
    /// </summary>
    void Update()
    {
    }

    /// <summary>
    /// Crée une nouvelle balle dans la scène, l’enregistre dans le ForceManager,
    /// et crée un objet texte qui suit cette balle.
    /// </summary>
    public void CreateBall()
    {
        // Instancie la balle à une position fixe (5,5,0)
        GameObject gameObj = Instantiate(
            prefab,
            new Vector3(4, -15, 0),
            Quaternion.Euler(0, 0, 0)
        );

        // Ajoute la balle au système physique (gestion des forces)
        manager.GetComponent<ForceManager>().AddBall(gameObj);

        // Crée un texte associé à la balle
        GameObject textObj = Instantiate(textPrefab);

        // Lie le texte à la balle pour qu’il la suive à l’écran
        textObj.GetComponent<FollowBall>().SetBall(gameObj);
    }
}