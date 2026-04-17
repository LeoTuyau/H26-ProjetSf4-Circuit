using System.Collections.Generic;
using UnityEngine;

public class FilsLigneChamps : MonoBehaviour
{
    [Header("Référence")]
    public Transform fil;
    public FilConducteur filConducteur;

    [Header("Cercles")]
    public int nombreCercles = 5;
    public int segmentsParCercle = 100;
    public float rayonInitial = 0.8f;
    public float espacement = 0.6f;

    [Header("Apparence lignes")]
    public float largeurLigne = 0.03f;
    public Material lineMaterial;
    public int sortingOrderLignes = -1;

    [Header("Flèches")]
    public int flechesParCercle = 8;
    public float tailleFleche = 0.18f;
    public Color couleurFleche = Color.white;
    public int sortingOrderFleches = 2;

    private readonly List<LineRenderer> lignes = new();
    private readonly List<List<Transform>> fleches = new();

    void Start()
    {
        if (fil == null)
            fil = transform;

        CreerCerclesEtFleches();
        MettreAJourTout();
    }

    void Update()
    {
        MettreAJourTout();
    }

    void CreerCerclesEtFleches()
    {
        for (int i = 0; i < nombreCercles; i++)
        {
            float rayon = rayonInitial + i * espacement;

            GameObject cercle = new GameObject("CercleChamp_" + i);
            cercle.transform.SetParent(transform);
            cercle.transform.position = fil.position;

            LineRenderer lr = cercle.AddComponent<LineRenderer>();
            lr.useWorldSpace = false;
            lr.loop = true;
            lr.positionCount = segmentsParCercle;
            lr.startWidth = largeurLigne;
            lr.endWidth = largeurLigne;
            lr.sortingOrder = sortingOrderLignes;
            lr.material = lineMaterial != null
                ? lineMaterial
                : new Material(Shader.Find("Sprites/Default"));

            for (int s = 0; s < segmentsParCercle; s++)
            {
                float angle = s * Mathf.PI * 2f / segmentsParCercle;
                float x = Mathf.Cos(angle) * rayon;
                float y = Mathf.Sin(angle) * rayon;
                lr.SetPosition(s, new Vector3(x, y, 0f));
            }

            lignes.Add(lr);

            List<Transform> flechesCercle = new();
            for (int a = 0; a < flechesParCercle; a++)
            {
                GameObject fleche = CreerFlecheSprite("Fleche_" + i + "_" + a);
                fleche.transform.SetParent(transform);
                fleche.transform.localScale = Vector3.one * tailleFleche;

                SpriteRenderer sr = fleche.GetComponent<SpriteRenderer>();
                sr.color = couleurFleche;
                sr.sortingOrder = sortingOrderFleches;

                flechesCercle.Add(fleche.transform);
            }

            fleches.Add(flechesCercle);
        }
    }

    void MettreAJourTout()
    {
        if (fil == null)
            return;

        Vector3 centre = fil.position;
        bool courantEntre = filConducteur != null && filConducteur.courantEntreDansLEcran;

        for (int i = 0; i < nombreCercles; i++)
        {
            float rayon = rayonInitial + i * espacement;

            if (lignes[i] != null)
                lignes[i].transform.position = centre;

            for (int a = 0; a < flechesParCercle; a++)
            {
                float angle = a * Mathf.PI * 2f / flechesParCercle;

                Vector3 position = centre + new Vector3(
                    Mathf.Cos(angle) * rayon,
                    Mathf.Sin(angle) * rayon,
                    0f);

                fleches[i][a].position = position;

                Vector2 radial = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                Vector2 tangente;

                if (courantEntre)
                {
                    // sens horaire
                    tangente = new Vector2(radial.y, -radial.x);
                }
                else
                {
                    // sens antihoraire
                    tangente = new Vector2(-radial.y, radial.x);
                }

                float angleRotation = Mathf.Atan2(tangente.y, tangente.x) * Mathf.Rad2Deg - 90f;
                fleches[i][a].rotation = Quaternion.Euler(0f, 0f, angleRotation);
            }
        }
    }

    GameObject CreerFlecheSprite(string nom)
    {
        GameObject go = new GameObject(nom);
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = CreerSpriteTriangle();
        return go;
    }

    Sprite CreerSpriteTriangle()
    {
        int w = 32;
        int h = 32;

        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;

        Color transparent = new Color(0, 0, 0, 0);

        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                tex.SetPixel(x, y, transparent);

        int centre = w / 2;

        for (int y = 0; y < h; y++)
        {
            float t = y / (float)(h - 1);
            int demiLargeur = Mathf.RoundToInt((1f - t) * (w * 0.22f));

            for (int x = centre - demiLargeur; x <= centre + demiLargeur; x++)
                tex.SetPixel(x, y, Color.white);
        }

        tex.Apply();

        return Sprite.Create(
            tex,
            new Rect(0, 0, w, h),
            new Vector2(0.5f, 0.5f),
            64f
        );
    }
}