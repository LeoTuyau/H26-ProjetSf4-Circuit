using UnityEngine;
using UnityEngine.UI; // Nécessaire pour accéder au composant Toggle de l'UI Unity

/// <summary>
/// Contrôle la visibilité du graphique de position X du pendule
/// via un Toggle dans l'interface utilisateur.
/// Permet à l'utilisateur d'afficher ou masquer le graphique à la volée.
/// </summary>
public class GraphToggleController : MonoBehaviour
{
    [Header("Références UI")]
    // Référence au composant Toggle Unity UI (objet "GraphToggle" dans la hiérarchie)
    public Toggle graphToggle;

    // Référence au GameObject contenant le graphique (objet "GraphPanel" dans la hiérarchie)
    public GameObject graph; 


 /// <summary>
    /// Initialisation : synchronise l'état du graphique avec le toggle
    /// et enregistre l'écouteur d'événement.
    /// </summary>
    void Start()
    {
         // Synchronise la visibilité initiale du graphique avec l'état du toggle au lancement
        // (ex: si le toggle est décoché au départ, le graphique sera caché dès le début)
        graph.SetActive(graphToggle.isOn);

        // Abonne la méthode OnToggleChanged à l'événement OnValueChanged du toggle
        // → sera appelée automatiquement chaque fois que l'utilisateur clique sur le toggle
        graphToggle.onValueChanged.AddListener(OnToggleChanged);
    }


    /// <summary>
    /// Callback déclenché automatiquement lorsque l'utilisateur change l'état du toggle.
    /// </summary>
    /// <param name="isOn">True si le toggle est activé (graphique visible), 
    /// false si désactivé (graphique caché)</param>
    void OnToggleChanged(bool isOn)
    {
         // Active ou désactive le GraphPanel selon l'état du toggle
        graph.SetActive(isOn);
    }
}