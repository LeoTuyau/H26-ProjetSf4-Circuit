using System.Collections.Generic;
using UnityEngine;

public class FieldLinesArrows2D_Optimized : MonoBehaviour
{
    public Aimant2D aimant;

    [Header("Lines")]
    public int nbLignes = 16;
    public int steps = 160;
    public float stepSize = 0.12f;
    public float startRadius = 0.7f;
    public float distanceMin = 0.35f;

    [Header("Arrows")]
    public int arrowEvery = 12;        
    public float arrowScale = 0.8f;    
    public int sortingOrder = 5000;    

    [Header("Rendering")]
    public float lineWidth = 0.025f;

    private const string Urp2DShader = "Universal Render Pipeline/2D/Sprite-Unlit-Default";

    private readonly List<LineRenderer> _lines = new();
    private readonly List<List<Transform>> _arrowPools = new();

    private Sprite _arrowSprite;
    private Material _spriteMat;

    void Awake()
    {
        _arrowSprite = CreateArrowSpriteOnce();
        _spriteMat = Get2DSpriteMaterial();
    }

    void Start()
    {
        BuildOnce();
    }

    void Update()
    {
        if (aimant == null) return;
        UpdateLinesAndArrows();
    }

    Material Get2DSpriteMaterial()
    {
        var shader = Shader.Find(Urp2DShader);
        if (shader != null) return new Material(shader);

        shader = Shader.Find("Sprites/Default");
        return new Material(shader);
    }

    void BuildOnce()
    {
        foreach (var lr in _lines) if (lr) Destroy(lr.gameObject);
        _lines.Clear();

        foreach (var pool in _arrowPools)
            foreach (var tr in pool)
                if (tr) Destroy(tr.gameObject);
        _arrowPools.Clear();

        int arrowsPerLine = Mathf.Max(1, steps / Mathf.Max(1, arrowEvery));

        for (int i = 0; i < nbLignes; i++)
        {
            var lineGO = new GameObject($"FieldLine_{i}");
            lineGO.transform.SetParent(transform, false);

            var lr = lineGO.AddComponent<LineRenderer>();
            lr.material = _spriteMat;
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.useWorldSpace = true;
            lr.positionCount = steps;

            lr.sortingLayerName = "Default";
            lr.sortingOrder = sortingOrder - 100;
            _lines.Add(lr);

            var pool = new List<Transform>(arrowsPerLine);
            for (int a = 0; a < arrowsPerLine; a++)
            {
                var arrowGO = new GameObject($"Arrow_{i}_{a}");
                arrowGO.transform.SetParent(transform, false);

                var sr = arrowGO.AddComponent<SpriteRenderer>();
                sr.sprite = _arrowSprite;
                sr.color = Color.blue;

                sr.sortingLayerName = "Default";
                sr.sortingOrder = sortingOrder;
                sr.sharedMaterial = _spriteMat;

                arrowGO.transform.localScale = Vector3.one * arrowScale;

                var p = arrowGO.transform.position;
                arrowGO.transform.position = new Vector3(p.x, p.y, 0f);

                pool.Add(arrowGO.transform);
            }

            _arrowPools.Add(pool);
        }
    }

    void UpdateLinesAndArrows()
    {
        Vector2 magnetPos = aimant.transform.position;
        Vector2 m = aimant.Moment;

        int arrowsPerLine = _arrowPools[0].Count;

        for (int i = 0; i < nbLignes; i++)
        {
            float t = ((i + 0.5f) / (float)nbLignes) * Mathf.PI * 2f;

            Vector2 p = magnetPos + new Vector2(Mathf.Cos(t), Mathf.Sin(t)) * startRadius;

            int arrowIndex = 0;

            for (int s = 0; s < steps; s++)
            {
                _lines[i].SetPosition(s, new Vector3(p.x, p.y, 0f));

                Vector2 B = DipoleField(p, magnetPos, m, distanceMin);
                if (B.sqrMagnitude < 1e-10f)
                    break;

                float d = Vector2.Distance(p, magnetPos);
                bool tooClose = d < (distanceMin * 1.5f);

                if (!tooClose && s % arrowEvery == 0 && arrowIndex < arrowsPerLine)
                {
                    var tr = _arrowPools[i][arrowIndex];

                    tr.position = new Vector3(p.x, p.y, 0f);

                    float ang = Mathf.Atan2(B.y, B.x) * Mathf.Rad2Deg - 90f;
                    tr.rotation = Quaternion.Euler(0f, 0f, ang);

                    tr.localScale = Vector3.one * arrowScale;

                    arrowIndex++;
                }

                p += B.normalized * stepSize;
            }

            for (int k = arrowIndex; k < arrowsPerLine; k++)
                _arrowPools[i][k].position = new Vector3(9999f, 9999f, 0f);
        }
    }

    static Vector2 DipoleField(Vector2 point, Vector2 magnetPos, Vector2 m, float distanceMin)
    {
        Vector2 r = point - magnetPos;
        float dist = Mathf.Max(r.magnitude, distanceMin);
        Vector2 rHat = r / dist;

        return (3f * Vector2.Dot(m, rHat) * rHat - m) / (dist * dist * dist);
    }

    static Sprite CreateArrowSpriteOnce()
    {
        int w = 32, h = 32;
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;

        Color clear = new Color(0, 0, 0, 0);
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                tex.SetPixel(x, y, clear);

        int mid = w / 2;
        for (int y = 0; y < h; y++)
        {
            float k = y / (float)(h - 1);
            int half = Mathf.RoundToInt(k * (w * 0.35f));
            for (int x = mid - half; x <= mid + half; x++)
                tex.SetPixel(x, y, Color.white);
        }

        tex.Apply();

        const float pixelsPerUnit = 40f;

        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), pixelsPerUnit);
    }
}