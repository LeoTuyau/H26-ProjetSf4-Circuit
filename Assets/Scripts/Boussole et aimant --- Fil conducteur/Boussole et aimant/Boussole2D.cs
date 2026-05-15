using UnityEngine;

public class Boussole2D : MonoBehaviour
{
    // Référence vers l'aimant qui produit le champ magnétique.
    public Aimant2D aimant;

    // Valeur utilisée pour lisser la rotation de la boussole.
    // Plus la valeur est grande, plus la boussole tourne rapidement.
    public float lissage = 12f;

    // Distance minimale utilisée pour éviter des calculs trop extrêmes
    // lorsque la boussole est très proche de l'aimant.
    public float distanceMin = 0.25f;

    void Update()
    {
        // Si aucun aimant n'est assigné, on ne peut pas calculer le champ magnétique.
        if (aimant == null) return;

        // Vecteur allant de l'aimant vers la boussole.
        Vector2 r = (Vector2)transform.position - (Vector2)aimant.transform.position;

        // Distance entre l'aimant et la boussole.
        // On impose une distance minimale pour éviter une division par un nombre trop petit.
        float dist = Mathf.Max(r.magnitude, distanceMin);

        // Direction normalisée entre l'aimant et la boussole.
        Vector2 rHat = r / dist;

        // Moment magnétique de l'aimant.
        Vector2 m = aimant.Moment;

        // Calcul simplifié du champ magnétique d'un dipôle.
        // Cette formule donne la direction du champ magnétique à la position de la boussole.
        Vector2 B = (3f * Vector2.Dot(m, rHat) * rHat - m) / (dist * dist * dist);

        // Si le champ est presque nul, on évite de faire tourner la boussole.
        if (B.sqrMagnitude < 1e-8f) return;

        // Conversion de la direction du champ magnétique en angle.
        // Le -90f sert à aligner l'aiguille avec le sprite de la boussole.
        float angle = Mathf.Atan2(B.y, B.x) * Mathf.Rad2Deg - 90f;

        // On interpole l'angle actuel vers le nouvel angle pour avoir une rotation fluide.
        float a = Mathf.LerpAngle(transform.eulerAngles.z, angle, lissage * Time.deltaTime);

        // Application de la rotation finale à la boussole.
        transform.rotation = Quaternion.Euler(0f, 0f, a);
    }
}