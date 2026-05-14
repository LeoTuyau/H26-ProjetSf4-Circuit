using UnityEngine;
using TMPro;
 
/// <summary>
/// Tooltip qui suit la souris et affiche les infos d'une composante.
/// Placez ce script sur un GameObject UI Canvas avec un TMP_Text enfant.
/// </summary>
public class BoiteInfo : MonoBehaviour
{
    public static BoiteInfo Instance;
 
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text texte;
    private Vector2 offset = new Vector2(170,-80);

    private RectTransform rectTransform;
 
    void Awake()
    {
        Instance = this;
        rectTransform = GetComponent<RectTransform>();
        panel.SetActive(false);
    }
 
    void Update()
    {
        if (panel.activeSelf)
        {
            // Suit la souris
            rectTransform.position = Input.mousePosition + (Vector3)offset;
        }
    }
 
    public void Afficher(string contenu)
    {
        texte.text = contenu;
        panel.SetActive(true);
    }
 
    public void Cacher()
    {
        panel.SetActive(false);
    }
}