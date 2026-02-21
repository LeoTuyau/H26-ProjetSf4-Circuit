using TMPro;
using UnityEngine;

public class ForceAttraction : MonoBehaviour

{
    [SerializeField] private TextMeshPro myText;
    [SerializeField] float charge = 5f;
    [SerializeField] Material redMaterial;
    [SerializeField] Material blueMaterial;
    private Renderer rend;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rend = GetComponent<Renderer>();

    }

    // Update is called once per frame
    void Update()
    {
        if (charge < 0)
        {
            rend.material = redMaterial;
        }
        else
        {
            rend.material = blueMaterial;
        }
    }
    public void setCharge(float charge)
    {
        this.charge = charge;
        myText.text = charge.ToString("F1") + "C";
    }
    public float getCharge()
    {
        return charge;
    }
}
