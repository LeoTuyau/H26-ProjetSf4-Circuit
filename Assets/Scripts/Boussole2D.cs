using UnityEngine;

public class Boussole2D : MonoBehaviour
{
    public Aimant2D aimant;

    public float lissage = 12f;        // vitesse de rotation
    public float distanceMin = 0.25f;  // évite force infinie près de l’aimant

    void Update()
    {
        if (aimant == null) return;

        Vector2 r = (Vector2)transform.position - (Vector2)aimant.transform.position;
        float dist = Mathf.Max(r.magnitude, distanceMin);
        Vector2 rHat = r / dist;

        Vector2 m = aimant.Moment; // direction N->S

        // Champ dipôle 2D (on utilise la formule 3D mais en vecteurs 2D)
        Vector2 B = (3f * Vector2.Dot(m, rHat) * rHat - m) / (dist * dist * dist);

        if (B.sqrMagnitude < 1e-8f) return;

        // Angle pour aligner transform.up avec B
        float angle = Mathf.Atan2(B.y, B.x) * Mathf.Rad2Deg - 90f;

        // Rotation douce vers la cible
        float a = Mathf.LerpAngle(transform.eulerAngles.z, angle, lissage * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, a);
    }
}