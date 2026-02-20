using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Runtime.CompilerServices;
public class ForceManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> listeBalles = new List<GameObject>();
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < listeBalles.Count; i++)
        {
            Vector3 forceR = Vector3.zero;
            for (int j = 0; j < listeBalles.Count; j++)
            {
                if (j != i)
                {
                    Vector3 direction = (listeBalles[j].transform.position - listeBalles[i].transform.position).normalized;
                    float distance = (listeBalles[j].transform.position - listeBalles[i].transform.position).magnitude;
                    float q1 = listeBalles[i].GetComponent<ForceAttraction>().getCharge();
                    float q2 = listeBalles[j].GetComponent<ForceAttraction>().getCharge();
                    forceR += -direction * q1 * q2 / (distance * distance);
                }
            }
            listeBalles[i].GetComponent<Rigidbody>().AddForce(forceR);
        }
    }
    public void AddBall(GameObject ball)
    {
        listeBalles.Add(ball);
    }
}
//Rigidbody rb = GetComponent<Rigidbody>();


//Vector3 direction = (otherSphere.transform.position - transform.position).normalized;
//float distance = (otherSphere.transform.position - transform.position).magnitude;
//rb.AddForce(-direction * charge * q2 / (distance * distance), ForceMode.Force);
