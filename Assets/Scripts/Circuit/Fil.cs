using UnityEngine;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine.UIElements;

public class Fil : MonoBehaviour
{
    bool sens; //true : a->b, false : b->a
    Vector3 posA;
    Vector3 posB;
    GameObject ObjetA;
    GameObject ObjetB;
    float anchorOffsetA;
    float anchorOffsetB;

    void Start()
    {

    }

    void Update()
    {
        if (ObjetA != null && ObjetB != null)
        {
            Debug.Log("yerp");
            Vector3 posAnchorA = ObjetA.transform.position + transform.right * anchorOffsetA;
            Vector3 posAnchorB = ObjetB.transform.position + transform.right * anchorOffsetB;

            transform.position = (posAnchorA + posAnchorB) / 2;
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(posAnchorA - posAnchorB, new Vector3(0, 0, 1)));
            transform.localScale = new Vector3(0, (posAnchorA - posAnchorB).magnitude, 0);
        }
    }
    public void SetAttaches(GameObject ObjetA, GameObject ObjetB)
    {
        Debug.Log("yo");
        this.ObjetA = ObjetA;
        this.ObjetA = ObjetB;
    }
    public void SetAnchorOffsets(GameObject AnchorA, GameObject AnchorB)
    {
        anchorOffsetA = AnchorA.GetComponent<Anchor>().GetOffset();
        anchorOffsetB = AnchorB.GetComponent<Anchor>().GetOffset();
    }
}
