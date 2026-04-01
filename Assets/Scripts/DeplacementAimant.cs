using UnityEngine;
using UnityEngine.EventSystems;

public class DeplacementAimant : MonoBehaviour
{
    private Camera cameraPrincipale;
    private bool estEnDeplacement;
    private Vector3 decalage;
    private SpriteRenderer[] rendusSpritesEnfants;

    void Awake()
    {
        cameraPrincipale = Camera.main;
        rendusSpritesEnfants = GetComponentsInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (cameraPrincipale == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            Vector3 positionSourisMonde = cameraPrincipale.ScreenToWorldPoint(Input.mousePosition);
            positionSourisMonde.z = transform.position.z;

            Vector2 positionSouris2D = new Vector2(positionSourisMonde.x, positionSourisMonde.y);

            if (SourisSurUnEnfant(positionSouris2D))
            {
                estEnDeplacement = true;
                decalage = transform.position - positionSourisMonde;
            }
        }

        if (estEnDeplacement && Input.GetMouseButton(0))
        {
            Vector3 positionSourisMonde = cameraPrincipale.ScreenToWorldPoint(Input.mousePosition);
            positionSourisMonde.z = transform.position.z;

            transform.position = new Vector3(
                positionSourisMonde.x + decalage.x,
                positionSourisMonde.y + decalage.y,
                transform.position.z
            );
        }

        if (Input.GetMouseButtonUp(0))
        {
            estEnDeplacement = false;
        }
    }

    private bool SourisSurUnEnfant(Vector2 positionSouris)
    {
        foreach (SpriteRenderer renduSprite in rendusSpritesEnfants)
        {
            if (renduSprite == null || !renduSprite.enabled) continue;

            Bounds bornes = renduSprite.bounds;

            if (positionSouris.x >= bornes.min.x &&
                positionSouris.x <= bornes.max.x &&
                positionSouris.y >= bornes.min.y &&
                positionSouris.y <= bornes.max.y)
            {
                return true;
            }
        }

        return false;
    }
}