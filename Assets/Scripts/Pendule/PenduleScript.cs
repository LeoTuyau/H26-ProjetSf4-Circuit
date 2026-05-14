using UnityEngine;


/// <summary>
/// Simule le mouvement physique d'un pendule simple en 2D.
/// Calcule l'angle, la vitesse et l'accélération angulaire à chaque frame
/// en utilisant l'équation différentielle du pendule, puis applique
/// la rotation à l'objet Unity et envoie les données au graphique.
/// </summary>
public class PenduleScript : MonoBehaviour
{

    [Header("Paramètres physiques du pendule")]

    // Longueur du fil du pendule en mètres — contrôlée par LongueurSlider
    public float length = 1f;

     // Intensité de la gravité en m/s² — contrôlée par GraviteSlider
    // (9.81 = gravité terrestre standard)
    public float gravity = 9.81f;  

// Coefficient d'amortissement (friction de l'air) — valeur entre 0 et 1
    // 1.0 = aucune perte d'énergie, 0.999 = légère friction réaliste
    public float damping = 0.999f; 


[Header("État interne du pendule")]

    // Angle initial du pendule converti en radians dès la déclaration
    // 45° est un angle de départ visible et physiquement intéressant
    private float angle = 45f * Mathf.Deg2Rad; 

    // Vitesse angulaire actuelle en radians/seconde (0 = au repos au départ)
    private float angularVelocity = 0f;

[Header("Références - Graphique")]

    // Référence au script du graphique pour lui envoyer les valeurs d'angle
    public GraphScript graph;

    // Référence au panneau UI du graphique (utilisé pour l'activer/désactiver)
    public GameObject graphPanel;
    

 /// <summary>
    /// Met à jour la longueur du pendule depuis le slider UI.
    /// Connectée à l'événement OnValueChanged du slider "LongueurSlider".
    /// </summary>
    /// <param name="newValue">Nouvelle longueur en mètres</param>
public void UpdateLength(float newValue)
    {
        length = newValue;
    }

/// <summary>
    /// Met à jour la gravité depuis le slider UI.
    /// Connectée à l'événement OnValueChanged du slider "GraviteSlider".
    /// </summary>
    /// <param name="newValue">Nouvelle valeur de gravité en m/s²</param>
    public void UpdateGravity(float newValue)
    {
        gravity = newValue;
    }

 /// <summary>
    /// Calcule et applique la physique du pendule à chaque frame.
    /// Utilise l'intégration d'Euler pour approximer le mouvement.
    /// </summary>
    void Update()
    {

        // Temps écoulé depuis la dernière frame (en secondes)
        // Permet une simulation indépendante du framerate
        float dt = Time.deltaTime;

        // ── Équation différentielle du pendule simple ──────────────────
        // θ'' = -(g / L) * sin(θ)
        // où θ = angle, g = gravité, L = longueur
        // La vérification length != 0 évite une division par zéro
        // si le slider de longueur est poussé à son minimum
        float angularAcceleration = 0;
        if (length != 0) {
            angularAcceleration = -(gravity / length) * Mathf.Sin(angle);
        }

         // ── Intégration d'Euler ────────────────────────────────────────
        // Méthode numérique simple pour mettre à jour vitesse et position

        // 1. Met à jour la vitesse angulaire avec l'accélération calculée
        angularVelocity += angularAcceleration * dt;

         // 2. Applique l'amortissement pour simuler la friction de l'air
        //    (multiplie la vitesse par 0.999 à chaque frame → ralentissement progressif)
        angularVelocity *= damping; 

        // 3. Met à jour l'angle avec la vitesse angulaire actuelle
        angle += angularVelocity * dt;

       // ── Application visuelle ───────────────────────────────────────
        // Convertit l'angle de radians en degrés et applique la rotation
        // sur l'axe Z (rotation 2D dans le plan de la scène)
        transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);

        // Envoie l'angle actuel au graphique pour tracer la courbe en temps réel
        graph.AddValue(angle);
    }
}
