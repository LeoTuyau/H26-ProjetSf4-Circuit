using UnityEngine;

public class Anchor : MonoBehaviour
{
    Renderer rend;
    [SerializeField] Material YellowMaterial;
    [SerializeField] Material GreenMaterial;
    bool Selected;
    [SerializeField] GameObject Attache;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Anchor(GameObject Attache)
    {
        this.Attache = Attache;
    }
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
            Selected = true;
        }
        else
        {
            Debug.Log("yellow");
            rend.sharedMaterial = YellowMaterial;
            Selected = false;
        }
    }
    public bool GetSelect()
    {
        return this.Selected;
    }
    public GameObject GetAttache()
    {
        return Attache;
    }
    public void SetAttache(GameObject Attache)
    {
        this.Attache = Attache;
    }
}
