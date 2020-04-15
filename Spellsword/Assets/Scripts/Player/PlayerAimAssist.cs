using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimAssist : MonoBehaviour
{
    [SerializeField]
    List<Targetable> targetsInRange;
    public List<Targetable> TargetsInRange
    {
        get { return targetsInRange; }
    }

    [SerializeField]
    float maxAngleForAutoAim;
    public float MaxAngleForAutoAim
    {
        get { return maxAngleForAutoAim; }
    }

    // Start is called before the first frame update
    void Start()
    {
        targetsInRange = new List<Targetable>();

        if(maxAngleForAutoAim == 0)
        {
            maxAngleForAutoAim = 45;
            Debug.Log("PlayerAimAssist::Start()::maxAngleForAutoAim set to default of 45 because it wasn't set in the editor/prefab");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector3();
    }

    public void RemoveNullTargets()
    {
        for(int i = 0; i < targetsInRange.Count; i++)
        {
            if(targetsInRange[i] == null)
            {
                targetsInRange.RemoveAt(i);
                i--;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Targetable>() != null)
        {
            Debug.Log("AimAssist collided with " + collision.gameObject.name);
            targetsInRange.Add(collision.gameObject.GetComponent<Targetable>());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Targetable>() != null)
        {
            Debug.Log("AimAssist triggered with " + other.gameObject.name);
            targetsInRange.Add(other.gameObject.GetComponent<Targetable>());
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        for(int i = 0; i < targetsInRange.Count; i++)
        {
            if(targetsInRange[i].gameObject == collision.gameObject)
            {
                targetsInRange[i].ResetGlow();
                targetsInRange.RemoveAt(i);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        for (int i = 0; i < targetsInRange.Count; i++)
        {
            if (targetsInRange[i] == null)
            {
                targetsInRange.RemoveAt(i);
                i--;
            }

            if (targetsInRange[i].gameObject == other.gameObject)
            {
                targetsInRange[i].ResetGlow();
                targetsInRange.RemoveAt(i);
                i--;//Maybe dud, but keep in because reasons
                break;
            }
        }
    }
}
