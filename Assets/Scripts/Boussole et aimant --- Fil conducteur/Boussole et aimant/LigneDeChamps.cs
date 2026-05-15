using System.Collections.Generic;
using UnityEngine;

public class FieldLinesArrows2D_Optimized : MonoBehaviour
{
    // Référence vers l'aimant qui produit le champ magnétique.
    public Aimant2D aimant;

    [Header("Lines")]

    // Nombre de lignes de champ à dessiner autour de l'aimant.
    public int nbLignes = 16;

    // Nombre de points utilisés pour tracer chaque ligne.
    public int steps = 160;

    // Distance parcourue entre deux points de la ligne.
    public float stepSize = 0.12f;

    // Distance de départ des lignes par rapport au centre de l'aimant.
    public float startRadius = 0.7f;

    // Distance minimale pour éviter des calculs trop forts près de l'aimant.
    public float distanceMin = 0.35f;

    [Header("Arrows")]

    // Une flèche est placée à chaque nombre de points défini ici.
    public int arrowEvery = 12;

    // Taille des flèches.
    public float arrowScale = 0.8f;

    // Ordre d'affichage des lignes et des flèches.
    public int sortingOrder = 5000;

    [Header("Rendering")]

    // Largeur des lignes de champ.
    public float lineWidth = 0.025f;

    // Nom du shader utilisé avec le rendu 2D de Unity URP.
    private const string Urp2DShader = "Universal Render Pipeline/2D/Sprite-Unlit-Default";

    // Liste des LineRenderer utilisés pour dessiner les lignes de champ.
    private readonly List<LineRenderer> _lines = new();

    // Liste de groupes de flèches.
    // Chaque ligne de champ possède sa propre liste de flèches.
    private readonly List<List<Transform>> _arrowPools = new();

    // Sprite utilisé pour toutes les flèches.
    private Sprite _arrowSprite;

    // Matériau utilisé pour les lignes et les flèches.
    private Material _spriteMat;

    void Awake()
    {
        // On crée une seule fois le sprite de flèche pour éviter de le recréer inutilement.
        _arrowSprite = CreateArrowSpriteOnce();

        // On récupère le matériau compatible avec le rendu 2D.
        _spriteMat = Get2DSpriteMaterial();
    }

    void Start()
    {
        // On construit les lignes et les flèches une seule fois au démarrage.
        BuildOnce();
    }

    void Update()
    {
        // Si aucun aimant n'est assigné, on ne peut pas calculer les lignes de champ.
        if (aimant == null) return;

        // On met à jour les lignes et les flèches à chaque frame.
        // Cela permet aux lignes de suivre l'aimant s'il se déplace ou tourne.
        UpdateLinesAndArrows();
    }

    Material Get2DSpriteMaterial()
    {
        // On essaie d'abord d'utiliser le shader 2D d'URP.
        var shader = Shader.Find(Urp2DShader);

        // Si le shader existe, on crée un matériau avec celui-ci.
        if (shader != null) return new Material(shader);

        // Sinon, on utilise le shader 2D par défaut de Unity.
        shader = Shader.Find("Sprites/Default");
        return new Material(shader);
    }

    void BuildOnce()
    {
        // On détruit les anciennes lignes si elles existent déjà.
        foreach (var lr in _lines)
            if (lr)
                Destroy(lr.gameObject);

        // On vide la liste des lignes.
        _lines.Clear();

        // On détruit les anciennes flèches si elles existent déjà.
        foreach (var pool in _arrowPools)
            foreach (var tr in pool)
                if (tr)
                    Destroy(tr.gameObject);

        // On vide la liste des flèches.
        _arrowPools.Clear();

        // Calcul du nombre de flèches par ligne.
        int arrowsPerLine = Mathf.Max(1, steps / Mathf.Max(1, arrowEvery));

        // On crée chaque ligne de champ.
        for (int i = 0; i < nbLignes; i++)
        {
            // Création d'un objet pour contenir une ligne de champ.
            var lineGO = new GameObject($"FieldLine_{i}");
            lineGO.transform.SetParent(transform, false);

            // Ajout d'un LineRenderer pour dessiner la ligne.
            var lr = lineGO.AddComponent<LineRenderer>();
            lr.material = _spriteMat;
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;

            // Les points sont placés en coordonnées du monde.
            lr.useWorldSpace = true;

            // Nombre de points de la ligne.
            lr.positionCount = steps;

            // Réglages de l'ordre d'affichage.
            lr.sortingLayerName = "Default";
            lr.sortingOrder = sortingOrder - 100;

            // On ajoute la ligne à la liste.
            _lines.Add(lr);

            // Création d'une réserve de flèches pour cette ligne.
            var pool = new List<Transform>(arrowsPerLine);

            for (int a = 0; a < arrowsPerLine; a++)
            {
                // Création d'un objet pour une flèche.
                var arrowGO = new GameObject($"Arrow_{i}_{a}");
                arrowGO.transform.SetParent(transform, false);

                // Ajout d'un SpriteRenderer pour afficher la flèche.
                var sr = arrowGO.AddComponent<SpriteRenderer>();
                sr.sprite = _arrowSprite;

                // Couleur des flèches.
                sr.color = Color.blue;

                // Réglages d'affichage pour que les flèches apparaissent au-dessus des lignes.
                sr.sortingLayerName = "Default";
                sr.sortingOrder = sortingOrder;
                sr.sharedMaterial = _spriteMat;

                // Application de la taille de la flèche.
                arrowGO.transform.localScale = Vector3.one * arrowScale;

                // On force la position Z à 0 pour rester dans un plan 2D.
                var p = arrowGO.transform.position;
                arrowGO.transform.position = new Vector3(p.x, p.y, 0f);

                // On ajoute la flèche dans la réserve.
                pool.Add(arrowGO.transform);
            }

            // On ajoute la réserve de flèches dans la liste principale.
            _arrowPools.Add(pool);
        }
    }

    void UpdateLinesAndArrows()
    {
        // Position actuelle de l'aimant.
        Vector2 magnetPos = aimant.transform.position;

        // Moment magnétique de l'aimant.
        Vector2 m = aimant.Moment;

        // Nombre de flèches disponibles pour chaque ligne.
        int arrowsPerLine = _arrowPools[0].Count;

        // On met à jour chaque ligne de champ.
        for (int i = 0; i < nbLignes; i++)
        {
            // Angle de départ de la ligne autour de l'aimant.
            float t = ((i + 0.5f) / (float)nbLignes) * Mathf.PI * 2f;

            // Point de départ de la ligne autour de l'aimant.
            Vector2 p = magnetPos + new Vector2(Mathf.Cos(t), Mathf.Sin(t)) * startRadius;

            // Index de la prochaine flèche à utiliser sur cette ligne.
            int arrowIndex = 0;

            // On calcule progressivement les points de la ligne.
            for (int s = 0; s < steps; s++)
            {
                // On place le point actuel dans le LineRenderer.
                _lines[i].SetPosition(s, new Vector3(p.x, p.y, 0f));

                // On calcule le champ magnétique au point actuel.
                Vector2 B = DipoleField(p, magnetPos, m, distanceMin);

                // Si le champ est trop faible, on arrête le calcul de cette ligne.
                if (B.sqrMagnitude < 1e-10f)
                    break;

                // Distance entre le point actuel et l'aimant.
                float d = Vector2.Distance(p, magnetPos);

                // On évite de placer des flèches trop près de l'aimant.
                bool tooClose = d < (distanceMin * 1.5f);

                // On place une flèche à intervalles réguliers sur la ligne.
                if (!tooClose && s % arrowEvery == 0 && arrowIndex < arrowsPerLine)
                {
                    // On récupère une flèche disponible dans la réserve.
                    var tr = _arrowPools[i][arrowIndex];

                    // On place la flèche au point actuel.
                    tr.position = new Vector3(p.x, p.y, 0f);

                    // On oriente la flèche dans la direction du champ magnétique.
                    // Le -90f sert à aligner correctement le sprite.
                    float ang = Mathf.Atan2(B.y, B.x) * Mathf.Rad2Deg - 90f;
                    tr.rotation = Quaternion.Euler(0f, 0f, ang);

                    // On applique la taille de la flèche.
                    tr.localScale = Vector3.one * arrowScale;

                    // On passe à la prochaine flèche.
                    arrowIndex++;
                }

                // On avance le point dans la direction du champ magnétique.
                p += B.normalized * stepSize;
            }

            // Les flèches non utilisées sont envoyées très loin pour les cacher.
            for (int k = arrowIndex; k < arrowsPerLine; k++)
                _arrowPools[i][k].position = new Vector3(9999f, 9999f, 0f);
        }
    }

    static Vector2 DipoleField(Vector2 point, Vector2 magnetPos, Vector2 m, float distanceMin)
    {
        // Vecteur allant de l'aimant vers le point étudié.
        Vector2 r = point - magnetPos;

        // Distance entre l'aimant et le point.
        // On impose une distance minimale pour éviter une division par zéro ou une valeur trop grande.
        float dist = Mathf.Max(r.magnitude, distanceMin);

        // Direction normalisée entre l'aimant et le point.
        Vector2 rHat = r / dist;

        // Formule simplifiée du champ magnétique d'un dipôle.
        return (3f * Vector2.Dot(m, rHat) * rHat - m) / (dist * dist * dist);
    }

    static Sprite CreateArrowSpriteOnce()
    {
        // Dimensions de la texture utilisée pour créer la flèche.
        int w = 32, h = 32;

        // Création d'une texture transparente.
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;

        Color clear = new Color(0, 0, 0, 0);

        // On rend tous les pixels transparents au départ.
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                tex.SetPixel(x, y, clear);

        // Centre horizontal de la texture.
        int mid = w / 2;

        // On dessine un triangle blanc pixel par pixel.
        for (int y = 0; y < h; y++)
        {
            float k = y / (float)(h - 1);

            // La largeur augmente progressivement pour former une flèche triangulaire.
            int half = Mathf.RoundToInt(k * (w * 0.35f));

            for (int x = mid - half; x <= mid + half; x++)
                tex.SetPixel(x, y, Color.white);
        }

        // On applique les pixels modifiés.
        tex.Apply();

        // Nombre de pixels correspondant à une unité Unity.
        const float pixelsPerUnit = 40f;

        // On transforme la texture en Sprite.
        return Sprite.Create(
            tex,
            new Rect(0, 0, w, h),
            new Vector2(0.5f, 0.5f),
            pixelsPerUnit
        );
    }
}