using System.Collections.Generic;
using UnityEngine;

public class FilsLigneChamps : MonoBehaviour
{
    [Header("Référence")]

    // Référence vers le transform du fil autour duquel les lignes de champ seront dessinées.
    public Transform fil;

    // Référence vers le script FilConducteur pour connaître le sens du courant.
    public FilConducteur filConducteur;

    [Header("Cercles")]

    // Nombre de cercles de champ magnétique à afficher autour du fil.
    public int nombreCercles = 5;

    // Nombre de points utilisés pour dessiner chaque cercle.
    // Plus ce nombre est grand, plus le cercle est lisse.
    public int segmentsParCercle = 100;

    // Rayon du premier cercle.
    public float rayonInitial = 0.8f;

    // Distance entre chaque cercle.
    public float espacement = 0.6f;

    [Header("Apparence lignes")]

    // Largeur des lignes de champ.
    public float largeurLigne = 0.03f;

    // Matériau utilisé pour dessiner les lignes.
    public Material lineMaterial;

    // Ordre d'affichage des lignes.
    public int sortingOrderLignes = -1;

    [Header("Flèches")]

    // Nombre de flèches placées sur chaque cercle.
    public int flechesParCercle = 8;

    // Taille des flèches.
    public float tailleFleche = 0.18f;

    // Couleur des flèches.
    public Color couleurFleche = Color.white;

    // Ordre d'affichage des flèches.
    public int sortingOrderFleches = 2;

    // Liste contenant tous les LineRenderer des cercles.
    private readonly List<LineRenderer> lignes = new();

    // Liste contenant les flèches associées à chaque cercle.
    private readonly List<List<Transform>> fleches = new();

    void Start()
    {
        // Si aucun fil n'est assigné, on utilise l'objet actuel comme référence.
        if (fil == null)
            fil = transform;

        // On crée les cercles et les flèches une seule fois au démarrage.
        CreerCerclesEtFleches();

        // On place immédiatement les éléments correctement.
        MettreAJourTout();
    }

    void Update()
    {
        // On met à jour la position et l'orientation des cercles et des flèches à chaque frame.
        // Cela permet aux lignes de suivre le fil s'il est déplacé.
        MettreAJourTout();
    }

    void CreerCerclesEtFleches()
    {
        // On crée chaque cercle autour du fil.
        for (int i = 0; i < nombreCercles; i++)
        {
            // Calcul du rayon du cercle actuel.
            float rayon = rayonInitial + i * espacement;

            // Création d'un nouvel objet qui représentera le cercle.
            GameObject cercle = new GameObject("CercleChamp_" + i);
            cercle.transform.SetParent(transform);
            cercle.transform.position = fil.position;

            // Ajout d'un LineRenderer pour dessiner le cercle.
            LineRenderer lr = cercle.AddComponent<LineRenderer>();

            // Le cercle est dessiné en coordonnées locales.
            lr.useWorldSpace = false;

            // On ferme la ligne pour obtenir un cercle complet.
            lr.loop = true;

            // Nombre de points du cercle.
            lr.positionCount = segmentsParCercle;

            // Largeur de la ligne.
            lr.startWidth = largeurLigne;
            lr.endWidth = largeurLigne;

            // Ordre d'affichage.
            lr.sortingOrder = sortingOrderLignes;

            // Utilisation du matériau assigné ou d'un matériau par défaut.
            lr.material = lineMaterial != null
                ? lineMaterial
                : new Material(Shader.Find("Sprites/Default"));

            // On calcule les points du cercle avec cosinus et sinus.
            for (int s = 0; s < segmentsParCercle; s++)
            {
                float angle = s * Mathf.PI * 2f / segmentsParCercle;
                float x = Mathf.Cos(angle) * rayon;
                float y = Mathf.Sin(angle) * rayon;
                lr.SetPosition(s, new Vector3(x, y, 0f));
            }

            // On ajoute ce cercle à la liste des lignes.
            lignes.Add(lr);

            // Liste des flèches pour ce cercle.
            List<Transform> flechesCercle = new();

            // Création des flèches sur le cercle.
            for (int a = 0; a < flechesParCercle; a++)
            {
                // On crée une flèche sous forme de sprite triangulaire.
                GameObject fleche = CreerFlecheSprite("Fleche_" + i + "_" + a);

                // La flèche devient enfant de cet objet pour rester organisée dans la hiérarchie.
                fleche.transform.SetParent(transform);

                // On applique la taille de la flèche.
                fleche.transform.localScale = Vector3.one * tailleFleche;

                // On récupère son SpriteRenderer pour modifier son apparence.
                SpriteRenderer sr = fleche.GetComponent<SpriteRenderer>();
                sr.color = couleurFleche;
                sr.sortingOrder = sortingOrderFleches;

                // On ajoute la flèche dans la liste du cercle.
                flechesCercle.Add(fleche.transform);
            }

            // On ajoute toutes les flèches de ce cercle dans la liste principale.
            fleches.Add(flechesCercle);
        }
    }

    void MettreAJourTout()
    {
        // Si aucun fil n'est assigné, on ne peut pas mettre à jour les lignes.
        if (fil == null)
            return;

        // Centre des cercles, basé sur la position du fil.
        Vector3 centre = fil.position;

        // On vérifie le sens du courant.
        // Si filConducteur est null, le courant est considéré comme sortant par défaut ici.
        bool courantEntre = filConducteur != null && filConducteur.courantEntreDansLEcran;

        // On met à jour chaque cercle et ses flèches.
        for (int i = 0; i < nombreCercles; i++)
        {
            // Rayon du cercle actuel.
            float rayon = rayonInitial + i * espacement;

            // On place le cercle au centre du fil.
            if (lignes[i] != null)
                lignes[i].transform.position = centre;

            // On place et oriente chaque flèche du cercle.
            for (int a = 0; a < flechesParCercle; a++)
            {
                // Angle de la flèche autour du cercle.
                float angle = a * Mathf.PI * 2f / flechesParCercle;

                // Position de la flèche sur le cercle.
                Vector3 position = centre + new Vector3(
                    Mathf.Cos(angle) * rayon,
                    Mathf.Sin(angle) * rayon,
                    0f);

                fleches[i][a].position = position;

                // Vecteur radial qui part du centre vers la flèche.
                Vector2 radial = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                // La tangente représente la direction du champ magnétique.
                Vector2 tangente;

                if (courantEntre)
                {
                    // Si le courant entre dans l'écran,
                    // le champ magnétique tourne dans le sens horaire.
                    tangente = new Vector2(radial.y, -radial.x);
                }
                else
                {
                    // Si le courant sort de l'écran,
                    // le champ magnétique tourne dans le sens antihoraire.
                    tangente = new Vector2(-radial.y, radial.x);
                }

                // Conversion de la direction de la tangente en angle.
                // Le -90f sert à aligner correctement le sprite de la flèche.
                float angleRotation = Mathf.Atan2(tangente.y, tangente.x) * Mathf.Rad2Deg - 90f;

                // Application de la rotation à la flèche.
                fleches[i][a].rotation = Quaternion.Euler(0f, 0f, angleRotation);
            }
        }
    }

    GameObject CreerFlecheSprite(string nom)
    {
        // Création d'un GameObject pour représenter une flèche.
        GameObject go = new GameObject(nom);

        // Ajout d'un SpriteRenderer pour afficher une image 2D.
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();

        // Création et assignation d'un sprite triangulaire.
        sr.sprite = CreerSpriteTriangle();

        return go;
    }

    Sprite CreerSpriteTriangle()
    {
        // Dimensions de la texture utilisée pour créer la flèche.
        int w = 32;
        int h = 32;

        // Création d'une texture transparente.
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;

        Color transparent = new Color(0, 0, 0, 0);

        // On rend tous les pixels transparents au départ.
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                tex.SetPixel(x, y, transparent);

        // Centre horizontal de la texture.
        int centre = w / 2;

        // On dessine un triangle blanc pixel par pixel.
        for (int y = 0; y < h; y++)
        {
            float t = y / (float)(h - 1);

            // La largeur du triangle diminue progressivement pour former une pointe.
            int demiLargeur = Mathf.RoundToInt((1f - t) * (w * 0.22f));

            for (int x = centre - demiLargeur; x <= centre + demiLargeur; x++)
                tex.SetPixel(x, y, Color.white);
        }

        // On applique les modifications à la texture.
        tex.Apply();

        // On transforme la texture en Sprite utilisable par Unity.
        return Sprite.Create(
            tex,
            new Rect(0, 0, w, h),
            new Vector2(0.5f, 0.5f),
            64f
        );
    }
}