using UnityEngine;

// Ce script nécessite obligatoirement un Collider2D sur l'objet.
// Sans Collider2D, les fonctions OnMouseDown, OnMouseDrag et OnMouseUp ne fonctionneraient pas correctement.
[RequireComponent(typeof(Collider2D))]
public class DragObject2D : MonoBehaviour
{
    // Référence à la caméra principale de la scène.
    private Camera cameraPrincipale;

    // Indique si l'objet est actuellement en train d'être déplacé par la souris.
    private bool estEnDeplacement;

    // Décalage entre la position de l'objet et la position de la souris au moment du clic.
    // Cela permet d'éviter que l'objet se téléporte directement au centre de la souris.
    private Vector3 decalage;

    // Distance entre l'objet et la caméra.
    // Cette distance est nécessaire pour convertir correctement la position de la souris en position dans le monde.
    private float distanceCamera;

    void Awake()
    {
        // On récupère la caméra principale de la scène au démarrage du script.
        cameraPrincipale = Camera.main;
    }

    void OnMouseDown()
    {
        // Si aucune caméra principale n'est trouvée, on arrête la fonction.
        if (cameraPrincipale == null)
            return;

        // On convertit la position de la souris à l'écran en position dans le monde 2D de Unity.
        Vector2 positionSourisMonde = cameraPrincipale.ScreenToWorldPoint(Input.mousePosition);

        // On récupère tous les Collider2D qui se trouvent sous la souris.
        // Cela est utile si plusieurs objets sont superposés.
        Collider2D[] objetsSousSouris = Physics2D.OverlapPointAll(positionSourisMonde);

        // S'il n'y a aucun objet sous la souris, on arrête la fonction.
        if (objetsSousSouris == null || objetsSousSouris.Length == 0)
            return;

        // Variable qui va contenir le collider de l'objet le plus visible au-dessus des autres.
        Collider2D meilleurCollider = null;

        // On commence avec la plus petite valeur possible pour trouver le plus grand sortingOrder.
        int meilleurSortingOrder = int.MinValue;

        // On parcourt tous les objets trouvés sous la souris.
        foreach (Collider2D col in objetsSousSouris)
        {
            // On vérifie si l'objet possède un SpriteRenderer.
            SpriteRenderer sr = col.GetComponent<SpriteRenderer>();

            // Si l'objet possède un SpriteRenderer, on récupère son sortingOrder.
            // Sinon, on considère que son ordre est 0.
            int ordre = sr != null ? sr.sortingOrder : 0;

            // On garde l'objet qui a le plus grand sortingOrder.
            // Cela permet de sélectionner l'objet qui est visuellement au-dessus des autres.
            if (ordre > meilleurSortingOrder)
            {
                meilleurSortingOrder = ordre;
                meilleurCollider = col;
            }
        }

        // Si aucun collider valide n'a été trouvé ou si l'objet le plus haut n'est pas celui-ci,
        // alors cet objet ne doit pas être déplacé.
        if (meilleurCollider == null || meilleurCollider.gameObject != gameObject)
            return;

        // On convertit la position de l'objet dans le monde en position à l'écran.
        Vector3 positionEcran = cameraPrincipale.WorldToScreenPoint(transform.position);

        // On garde la distance entre l'objet et la caméra.
        distanceCamera = positionEcran.z;

        // On récupère la position actuelle de la souris.
        Vector3 positionSouris = Input.mousePosition;

        // On ajoute la distance de la caméra à la position de la souris.
        // Cela permet de convertir correctement la position écran vers une position dans le monde.
        positionSouris.z = distanceCamera;

        // On convertit la position de la souris en position dans le monde.
        Vector3 positionMonde = cameraPrincipale.ScreenToWorldPoint(positionSouris);

        // On calcule le décalage entre la position de l'objet et la position de la souris.
        decalage = transform.position - positionMonde;

        // On indique que l'objet est maintenant en déplacement.
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

        // On remet la même distance entre l'objet et la caméra.
        positionSouris.z = distanceCamera;

        // On convertit la position de la souris en position dans le monde.
        Vector3 positionMonde = cameraPrincipale.ScreenToWorldPoint(positionSouris);

        // On déplace l'objet à la position de la souris en ajoutant le décalage calculé au départ.
        // Grâce à cela, l'objet garde la même position relative par rapport à la souris.
        transform.position = positionMonde + decalage;
    }

    void OnMouseUp()
    {
        // Quand le bouton de la souris est relâché, on arrête le déplacement de l'objet.
        estEnDeplacement = false;
    }
}