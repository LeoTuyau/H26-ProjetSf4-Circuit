using UnityEngine;

public class Anchor : MonoBehaviour
{
    Renderer rend;
    [SerializeField] Material YellowMaterial;
    [SerializeField] Material GreenMaterial;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material = YellowMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ToggleSelect()
    {
        if (rend.sharedMaterial == YellowMaterial)
        {
            Debug.Log("green");
            rend.sharedMaterial = GreenMaterial;
        }
        else
        {
            Debug.Log("yellow");
            rend.sharedMaterial = YellowMaterial;
        }
    }
}
