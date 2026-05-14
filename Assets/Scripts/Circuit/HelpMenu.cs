using UnityEngine;

public class HelpMenu : MonoBehaviour
{
    private bool Active;
    [SerializeField] private float smooth = 50;
    [SerializeField] private Vector3 pointA = new Vector3(-170, -217, 0);
    [SerializeField] private Vector3 pointB = new Vector3(0, 0, 0);
    private bool goA = false;
    private bool goB = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Active = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (goA)
        {
            transform.position = Vector3.Lerp(
                transform.position, pointA,
                1 - Mathf.Exp(-smooth * Time.deltaTime));
            if (Vector3.Distance(transform.position, pointA) < 0.1f)
            {
                transform.position = pointA;
                goA = false;
            }
        }
        else if (goB)
        {
            transform.position = Vector3.Lerp(
                transform.position, pointB,
                1 - Mathf.Exp(-smooth * Time.deltaTime));
            if (Vector3.Distance(transform.position, pointB) < 0.1f)
            {
                transform.position = pointB;
                goB = false;
            }
        }
    }

    public void Toggle()
    {
        if (Active)
        {
            goA = true;
            goB = false;
            Active = false;
        }
        else
        {
            goB = true;
            goA = false;
            Active = true;
        }
    }
}
