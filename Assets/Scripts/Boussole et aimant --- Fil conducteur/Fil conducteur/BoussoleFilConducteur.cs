using UnityEngine;

public class BoussoleFilConducteur : MonoBehaviour
{
    public FilConducteur fil;
    public Transform aiguille;
    public float vitesseRotation = 5f;

    void Update()
    {
        if (fil == null || aiguille == null)
            return;

        Vector2 directionChamp = fil.GetDirectionChamp(transform.position);

        if (directionChamp == Vector2.zero)
            return;

        float angle = Mathf.Atan2(directionChamp.y, directionChamp.x) * Mathf.Rad2Deg - 90f;

        aiguille.rotation = Quaternion.Lerp(
            aiguille.rotation,
            Quaternion.Euler(0f, 0f, angle),
            vitesseRotation * Time.deltaTime
        );
    }
}