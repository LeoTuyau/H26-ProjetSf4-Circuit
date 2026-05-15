using UnityEngine;

public class BoussoleFilConducteur : MonoBehaviour
{
    // Référence vers le fil conducteur qui produit le champ magnétique.
    public FilConducteur fil;

    // Référence vers l'aiguille de la boussole.
    // C'est cet objet qui va tourner selon la direction du champ magnétique.
    public Transform aiguille;

    // Vitesse à laquelle l'aiguille tourne vers sa nouvelle direction.
    public float vitesseRotation = 5f;

    void Update()
    {
        // Si le fil ou l'aiguille n'est pas assigné,
        // on arrête la fonction pour éviter une erreur.
        if (fil == null || aiguille == null)
            return;

        // On demande au fil conducteur la direction du champ magnétique
        // à la position actuelle de la boussole.
        Vector2 directionChamp = fil.GetDirectionChamp(transform.position);

        // Si la direction retournée est nulle, cela signifie que la boussole
        // est trop proche du centre du fil ou que la direction ne peut pas être calculée.
        if (directionChamp == Vector2.zero)
            return;

        // On convertit la direction du champ magnétique en angle.
        // Mathf.Atan2 donne l'angle en radians, puis Mathf.Rad2Deg le convertit en degrés.
        // Le -90f sert à aligner correctement l'aiguille avec le sprite utilisé.
        float angle = Mathf.Atan2(directionChamp.y, directionChamp.x) * Mathf.Rad2Deg - 90f;

        // On fait tourner l'aiguille graduellement vers l'angle calculé.
        // Quaternion.Lerp permet d'obtenir une rotation plus fluide.
        aiguille.rotation = Quaternion.Lerp(
            aiguille.rotation,
            Quaternion.Euler(0f, 0f, angle),
            vitesseRotation * Time.deltaTime
        );
    }
}