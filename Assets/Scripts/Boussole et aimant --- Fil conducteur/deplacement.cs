using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class deplacement : MonoBehaviour
{
    private Camera cameraPrincipale;
    private bool estEnDeplacement;
    private Vector3 decalage;
    private float distanceCamera;

    void Awake()
    {
        cameraPrincipale = Camera.main;
    }

    void OnMouseDown()
    {
        if (cameraPrincipale == null)
            return;

        Vector2 positionSourisMonde = cameraPrincipale.ScreenToWorldPoint(Input.mousePosition);

        Collider2D[] objetsSousSouris = Physics2D.OverlapPointAll(positionSourisMonde);
        if (objetsSousSouris == null || objetsSousSouris.Length == 0)
            return;

        Collider2D meilleurCollider = null;
        int meilleurSortingOrder = int.MinValue;

        foreach (Collider2D col in objetsSousSouris)
        {
            SpriteRenderer sr = col.GetComponent<SpriteRenderer>();
            int ordre = sr != null ? sr.sortingOrder : 0;

            if (ordre > meilleurSortingOrder)
            {
                meilleurSortingOrder = ordre;
                meilleurCollider = col;
            }
        }

        if (meilleurCollider == null || meilleurCollider.gameObject != gameObject)
            return;

        Vector3 positionEcran = cameraPrincipale.WorldToScreenPoint(transform.position);
        distanceCamera = positionEcran.z;

        Vector3 positionSouris = Input.mousePosition;
        positionSouris.z = distanceCamera;

        Vector3 positionMonde = cameraPrincipale.ScreenToWorldPoint(positionSouris);
        decalage = transform.position - positionMonde;
        estEnDeplacement = true;
    }

    void OnMouseDrag()
    {
        if (!estEnDeplacement || cameraPrincipale == null)
            return;

        Vector3 positionSouris = Input.mousePosition;
        positionSouris.z = distanceCamera;

        Vector3 positionMonde = cameraPrincipale.ScreenToWorldPoint(positionSouris);
        transform.position = positionMonde + decalage;
    }

    void OnMouseUp()
    {
        estEnDeplacement = false;
    }
}
