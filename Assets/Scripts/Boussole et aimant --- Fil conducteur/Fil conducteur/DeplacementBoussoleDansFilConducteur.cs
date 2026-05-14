using UnityEngine;

// Ce script nécessite un Collider2D pour que l'objet puisse détecter les clics de souris.
[RequireComponent(typeof(Collider2D))]
public class DeplacementSimple : MonoBehaviour
{
    // Référence à la caméra principale de la scène.
    private Camera cameraPrincipale;

    // Indique si l'objet est actuellement déplacé par la souris.
    private bool estEnDeplacement;

    // Décalage entre la position de l'objet et la position de la souris au moment du clic.
    private Vector3 decalage;

    // Distance entre l'objet et la caméra.
    private float distanceCamera;

    void Awake()
    {
        // On récupère la caméra principale dès le début.
        cameraPrincipale = Camera.main;
    }

    void OnMouseDown()
    {
        // Si aucune caméra principale n'est trouvée, on arrête la fonction.
        if (cameraPrincipale == null)
            return;

        // On transforme la position de l'objet dans le monde en position à l'écran.
        Vector3 positionEcran = cameraPrincipale.WorldToScreenPoint(transform.position);

        // On garde la distance entre l'objet et la caméra.
        distanceCamera = positionEcran.z;

        // On récupère la position actuelle de la souris.
        Vector3 positionSouris = Input.mousePosition;

        // On ajoute la bonne distance en profondeur pour convertir correctement la position.
        positionSouris.z = distanceCamera;

        // On convertit la position de la souris de l'écran vers le monde Unity.
        Vector3 positionMonde = cameraPrincipale.ScreenToWorldPoint(positionSouris);

        // On calcule le décalage pour que l'objet ne se téléporte pas au centre de la souris.
        decalage = transform.position - positionMonde;

        // On active le déplacement.
        estEnDeplacement = true;
    }

    void OnMouseDrag()
    {
        // Si l'objet n'est pas en déplacement ou s'il n'y a pas de caméra,
        // on ne fait rien.
        if (!estEnDeplacement || cameraPrincipale == null)
            return;

        // On récupère la position actuelle de la souris.
        Vector3 positionSouris = Input.mousePosition;

        // On conserve la même distance par rapport à la caméra.
        positionSouris.z = distanceCamera;

        // On convertit la position de la souris en position dans le monde.
        Vector3 positionMonde = cameraPrincipale.ScreenToWorldPoint(positionSouris);

        // On déplace l'objet en suivant la souris tout en gardant le décalage initial.
        transform.position = positionMonde + decalage;
    }

    void OnMouseUp()
    {
        // Quand le bouton de la souris est relâché, on arrête le déplacement.
        estEnDeplacement = false;
    }
}