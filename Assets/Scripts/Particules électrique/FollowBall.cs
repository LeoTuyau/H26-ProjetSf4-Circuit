using Unity.Properties;
using UnityEngine;

/// <summary>
/// FollowBall fait suivre un objet (généralement un texte ou UI) à une balle.
/// Il met à jour sa position chaque frame pour rester attaché à la balle.
/// </summary>
public class FollowBall : MonoBehaviour
{
    /// <summary>
    /// Référence vers la balle que cet objet doit suivre.
    /// </summary>
    [SerializeField] private GameObject ball;

    /// <summary>
    /// Initialisation appelée une seule fois au démarrage.
    /// Actuellement inutilisée.
    /// </summary>
    void Start()
    {
    }

    /// <summary>
    /// Met à jour la position de l’objet chaque frame
    /// pour suivre la balle avec un offset fixe.
    /// </summary>
    void Update()
    {
        transform.position = ball.transform.position + new Vector3(0.25f, 1.7f, 0);
    }

    /// <summary>
    /// Assigne la balle que cet objet doit suivre.
    /// </summary>
    /// <param name="ball">Objet balle à suivre.</param>
    public void SetBall(GameObject ball)
    {
        this.ball = ball;
    }
}