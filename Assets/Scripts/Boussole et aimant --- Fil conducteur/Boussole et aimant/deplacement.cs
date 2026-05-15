using UnityEngine;

// Ce script nécessite un Collider2D pour détecter les clics de souris sur l'objet.
[RequireComponent(typeof(Collider2D))]
public class deplacement : MonoBehaviour
{
    // Référence à la caméra principale.
    private Camera cameraPrincipale;

    // Indique si l'objet est actuellement en train d'être déplacé.
    private bool estEnDeplacement;

    // Décalage entre l'objet et la souris au moment du clic.
    // Cela empêche l'objet de se téléporter directement au centre de la souris.
    private Vector3 decalage;

    // Distance entre l'objet et la caméra.
    private float distanceCamera;

    void Awake()
    {
        // On récupère la caméra principale de la scène.
        cameraPrincipale = Camera.main;
    }

    void OnMouseDown()
    {
        // Si aucune caméra n'est trouvée, on arrête la fonction.
        if (cameraPrincipale == null)
            return;

        // On convertit la position de la souris de l'écran vers le monde Unity.
        Vector2 positionSourisMonde = cameraPrincipale.ScreenToWorldPoint(Input.mousePosition);

        // On récupère tous les objets 2D situés sous la souris.
        Collider2D[] objetsSousSouris = Physics2D.OverlapPointAll(positionSourisMonde);

        // Si aucun objet n'est sous la souris, on arrête.
        if (objetsSousSouris == null || objetsSousSouris.Length == 0)
            return;

        // Variable qui contiendra le collider de l'objet le plus visible.
        Collider2D meilleurCollider = null;

        // On commence avec la plus petite valeur possible.
        int meilleurSortingOrder = int.MinValue;

        // On parcourt tous les objets sous la souris.
        foreach (Collider2D col in objetsSousSouris)
        {
            // On récupère le SpriteRenderer de l'objet, s'il existe.
            SpriteRenderer sr = col.GetComponent<SpriteRenderer>();

            // Le sortingOrder permet de savoir quel sprite est affiché au-dessus.
            int ordre = sr != null ? sr.sortingOrder : 0;

            // On garde l'objet avec le plus grand sortingOrder.
            if (ordre > meilleurSortingOrder)
            {
                meilleurSortingOrder = ordre;
                meilleurCollider = col;
            }
        }

        // Si l'objet le plus haut n'est pas cet objet,
        // on ne le déplace pas.
        if (meilleurCollider == null || meilleurCollider.gameObject != gameObject)
            return;

        // On convertit la position de l'objet en position écran.
        Vector3 positionEcran = cameraPrincipale.WorldToScreenPoint(transform.position);

        // On garde la distance entre l'objet et la caméra.
        distanceCamera = positionEcran.z;

        // On récupère la position de la souris.
        Vector3 positionSouris = Input.mousePosition;

        // On ajoute la profondeur correcte.
        positionSouris.z = distanceCamera;

        // On reconvertit la position de la souris vers le monde Unity.
        Vector3 positionMonde = cameraPrincipale.ScreenToWorldPoint(positionSouris);

        // On calcule le décalage entre l'objet et la souris.
        decalage = transform.position - positionMonde;

        // On active le déplacement.
        estEnDeplacement = true;
    }

    void OnMouseDrag()
    {
        // Si l'objet n'est pas en déplacement ou si la caméra n'existe pas,
        // on ne fait rien.
        if (!estEnDeplacement || cameraPrincipale == null)
            return;

        // On récupère la position actuelle de la souris.
        Vector3 positionSouris = Input.mousePosition;

        // On garde la même distance avec la caméra.
        positionSouris.z = distanceCamera;

        // On convertit la position de la souris en position monde.
        Vector3 positionMonde = cameraPrincipale.ScreenToWorldPoint(positionSouris);

        // On déplace l'objet à la position de la souris en gardant le décalage initial.
        transform.position = positionMonde + decalage;
    }

    void OnMouseUp()
    {
        // Quand on relâche la souris, l'objet arrête de se déplacer.
        estEnDeplacement = false;
    }
}