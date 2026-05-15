using UnityEngine;
using UnityEngine.UI;

public class Pile : Composante
{
    [SerializeField] private float tension = 9f;
    [SerializeField] private Slider slider;
    private Slider sliderP;
    private bool modeSlider = false;
    [SerializeField] private Canvas canvas;

    private void Start()
    {
        sliderP = Instantiate(slider, canvas.transform);
        sliderP.transform.SetParent(canvas.transform, false);
        sliderP.value = tension;
    }
    private void Update()
    {
        if (modeSlider)
        {
            sliderP.gameObject.SetActive(true);
            sliderP.transform.position = Camera.main.WorldToScreenPoint(transform.position+new Vector3(0,0.7f,0));
            tension = sliderP.value;
        }
        else
        {
            sliderP.gameObject.SetActive(false);
        }
    }

    public override float Tension    => tension;
    public override float ValeurOhms => 0f;
 
    public void SetTension(float v) => tension = Mathf.Max(0f, v);
 
    private void OnValidate() => tension = Mathf.Max(0f, tension);

    public override void ToggleModeSlider(bool modeSlider) => this.modeSlider = modeSlider;
}