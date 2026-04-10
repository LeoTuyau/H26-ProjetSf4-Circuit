using UnityEngine;
using UnityEngine.UI;

public class BouttonFil : MonoBehaviour
{
    [SerializeField] Sprite fil;
    [SerializeField] Sprite filSelected;

    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
    }

    public void ToggleColor()
    {
        if (image.sprite == fil)
        {
            image.sprite = filSelected;
        }
        else
        {
            image.sprite = fil;
        }
    }
    public void SetSelected(bool sel)
    {
        if (sel)
        {
            image.sprite = filSelected;
        }
        else
        {
            image.sprite = fil;
        }
    }
}
