using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gère l'affichage d'un graphique en temps réel dans l'interface Unity UI.
/// Trace la position X du pendule au fil du temps sous forme de points.
/// </summary>


public class GraphScript : MonoBehaviour
{
    [Header("Références UI")]
    // Conteneur RectTransform dans lequel les points du graphique sont instanciés (GraphPanel)
    public RectTransform graphContainer;

    // Prefab représentant un point sur le graphique (objet UI "Point")
    public GameObject pointPrefab;

// Liste des valeurs de position X enregistrées au fil du temps
    private List<float> values = new List<float>();

    // Nombre maximum de points affichés simultanément sur le graphique
    // (les plus anciens sont supprimés quand on dépasse cette limite)
    private int maxPoints = 100;


[Header("Paramètres du graphique")]
    // Amplitude maximale attendue du pendule (en mètres),
    // utilisée pour normaliser l'axe Y du graphique
    public float maxAmplitude = 1f;


    /// <summary>
    /// Ajoute une nouvelle valeur de position X au graphique.
    /// Appelée à chaque frame par PenduleScript pour mettre à jour l'affichage.
    /// </summary>
    /// <param name="value">Position X actuelle du pendule (en mètres)</param>
    public void AddValue(float value)
    {
        // Enregistre la nouvelle valeur dans l'historique
        values.Add(value);

        // Si on dépasse la limite de points affichables,
        // on retire le plus ancien pour garder un défilement fluide
        if (values.Count > maxPoints)
            values.RemoveAt(0);

        // Redessine le graphique avec la liste mise à jour
        DrawGraph();
    }

/// <summary>
    /// Efface et redessine tous les points du graphique à partir des valeurs actuelles.
    /// Appelée à chaque fois qu'une nouvelle valeur est ajoutée.
    /// </summary>
    void DrawGraph()
    {

        // Supprime tous les points existants dans le conteneur,
        // sauf les objets statiques marqués avec le tag "GraphStatic"
        // (ex: axes, labels, décorations fixes du graphique)
        foreach (Transform child in graphContainer)
{
    if (child.CompareTag("GraphStatic"))
        continue; // On garde les éléments statiques intacts

    Destroy(child.gameObject); // Supprime les anciens points
}
// Récupère les dimensions du conteneur graphique (en pixels UI)
        float width = graphContainer.sizeDelta.x;
        float height = graphContainer.sizeDelta.y;

// Instancie un point pour chaque valeur enregistrée
        for (int i = 0; i < values.Count; i++)
        {

            // Calcule la position X du point :
            // répartit les points uniformément sur la largeur du graphique,
            // centré à l'origine (de -width/2 à +width/2)
            float x = i * (width / maxPoints) - width / 2;

             // Calcule la position Y du point :
            // normalise la valeur par rapport à l'amplitude maximale,
            // puis la met à l'échelle sur la moitié de la hauteur du graphique
            // (0 = centre, ±height/2 = extrémités haut/bas)
            float y = (values[i] / maxAmplitude) * (height / 2);

            // Instancie le prefab Point en tant qu'enfant du conteneur
            GameObject point = Instantiate(pointPrefab, graphContainer);
             // Positionne le point aux coordonnées calculées dans l'espace UI
            point.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
        }
    }
}