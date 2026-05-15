using System;
using UnityEngine;
using UnityEngine.UI;
public class Resistance : Composante
{
    [SerializeField] private float valeur = 100f; // Ohms
    [SerializeField] private Slider slider;
    private Slider sliderR;
    private bool modeS = false;
    [SerializeField] private Canvas canvas;

    private void Start()
    {
        sliderR = Instantiate(slider, canvas.transform);
        sliderR.transform.SetParent(canvas.transform, false);
        sliderR.value = valeur;
    }
    private void Update()
    {
        Debug.Log(modeS);
        if (modeS)
        {
            sliderR.gameObject.SetActive(true);
            if (transform.rotation.eulerAngles.z == 90 && transform.rotation.eulerAngles.z == -90)
                sliderR.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1f, 0));
            else
                sliderR.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 0.6f, 0));
            valeur = sliderR.value; 
        }
        else
        {
            sliderR.gameObject.SetActive(false);
        }
    }

    public override float ValeurOhms => valeur;
    public override float Tension    => valeur * Courant; // V = R × I
 
    public void SetValeur(float v) => valeur = Mathf.Max(0.001f, v);
 
    private void OnValidate() => valeur = Mathf.Max(0.001f, valeur);

    public override void ToggleModeSlider(bool modeSlider)
    {
        modeS = modeSlider;
        Debug.Log(modeS);
    }
}