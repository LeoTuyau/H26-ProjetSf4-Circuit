using UnityEngine;

public class Aimant2D : MonoBehaviour
{
    // Le moment magnétique de l'aimant correspond à sa direction principale.
    // transform.up représente l'axe local vers le haut de l'objet.
    // Donc, si on tourne l'aimant, son moment magnétique change aussi.
    public Vector2 Moment => transform.up;
}