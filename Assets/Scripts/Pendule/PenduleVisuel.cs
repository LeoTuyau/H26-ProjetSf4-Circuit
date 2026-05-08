using UnityEngine;

/// <summary>
/// Met à jour la représentation visuelle du fil du pendule (Cylinder)
/// en temps réel lorsque la longueur change via le slider ou l'Inspector.
/// </summary>
public class PenduleVisuel : MonoBehaviour
{
    [Header("Références")]
    // Le Cylinder qui représente le fil du pendule dans la scène
    public Transform fil;

    // Le Sphere qui représente la boule du pendule
    public Transform boule;

    // Référence au script physique pour lire la longueur actuelle
    public PenduleScript penduleScript;

    [SerializeField] private GameObject camera;

    private float derniereLongueur = -1f;

    // Position de base fixe de la caméra (longueur = 1, état initial)
    // Toujours calculée depuis ce point — jamais accumulée
    private Vector3 positionBaseCamera;

    void Start()
    {
        // Sauvegarde la position initiale de la caméra comme référence fixe
        // Tous les ajustements futurs partiront toujours de ce point
        positionBaseCamera = camera.transform.position;

        // Initialise le visuel avec la longueur de départ
        MettreAJourVisuel(penduleScript.length);
    }

    void Update()
    {
        float longueur = penduleScript.length;

        // Ne redessine que si la longueur a changé (optimisation)
        if (longueur == derniereLongueur) return;
        derniereLongueur = longueur;

        MettreAJourVisuel(longueur);
        AjusterCamera(longueur);
    }

    /// <summary>
    /// Ajuste la position de la caméra selon la longueur du pendule.
    /// Toujours calculée depuis la position de base — jamais accumulée.
    /// </summary>
    void AjusterCamera(float longueur)
{
    // La caméra doit voir le pendule du pivot (Y=0) jusqu'à la boule (Y=-longueur)
    // Centre vertical = milieu du pendule = -longueur/2
    float centreY = 1f - longueur / 2f;

    // Recule proportionnellement à la longueur pour garder tout le pendule visible
    // À longueur 1 → Z = -10 (position de base), à longueur 10 → Z = -19
    float nouvelleZ = -10f - (longueur - 1f);

    camera.transform.position = new Vector3(0, centreY, nouvelleZ);
}

    void MettreAJourVisuel(float longueur)
    {
        // ── Redimensionne le fil ──────────────────────────────────────
        // Le Cylinder Unity a une hauteur de 2 unités par défaut (scale Y = 1)
        // donc on divise par 2 pour avoir la bonne longueur en mètres
        fil.localScale = new Vector3(fil.localScale.x, longueur / 2f, fil.localScale.z);

        // ── Repositionne le fil ───────────────────────────────────────
        // Le fil part du pivot (point d'attache en haut) vers le bas
        fil.localPosition = new Vector3(0, -longueur / 2f, 0);

        // ── Repositionne la boule au bout du fil ──────────────────────
        boule.localPosition = new Vector3(0, -longueur, 0);
    }
}