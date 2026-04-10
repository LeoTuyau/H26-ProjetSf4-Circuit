using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DeplacementSimple : MonoBehaviour
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