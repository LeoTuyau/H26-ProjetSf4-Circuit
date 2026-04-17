using UnityEngine;

public class FilConducteur : MonoBehaviour
{
    public bool courantEntreDansLEcran = true;

    public Vector2 GetDirectionChamp(Vector2 positionBoussole)
    {
        Vector2 radial = positionBoussole - (Vector2)transform.position;

        if (radial.sqrMagnitude < 0.0001f)
            return Vector2.zero;

        Vector2 directionChamp;

        if (courantEntreDansLEcran)
        {
            // champ horaire
            directionChamp = new Vector2(radial.y, -radial.x);
        }
        else
        {
            // champ antihoraire
            directionChamp = new Vector2(-radial.y, radial.x);
        }

        return directionChamp.normalized;
    }

    public void MettreCourantEntrant()
    {
        courantEntreDansLEcran = true;
    }

    public void MettreCourantSortant()
    {
        courantEntreDansLEcran = false;
    }
}