using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vines : MonoBehaviour
{
    [SerializeField]
    Material vineMaterial;

    bool isBurning;
    // Start is called before the first frame update
    void Start()
    {
        //Isburning vec1
        vineMaterial.SetFloat("_Vector1_D9BC0F3A", 0);

        //BurnLevel vec1
        vineMaterial.SetFloat("_Vector1_CFE875CF", 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(isBurning)
        {
            //Isburning vec1
            vineMaterial.SetFloat("_Vector1_D9BC0F3A", 1);

            //BurnLevel vec1
            vineMaterial.SetFloat("_Vector1_CFE875CF", Mathf.Min(vineMaterial.GetFloat("_Vector1_CFE875CF") + 0.004f, 1));
        }
        if(vineMaterial.GetFloat("_Vector1_CFE875CF") >= 0.38 && GetComponent<Torch>().FireActive)
        {
            Debug.Log("Vines::Update()::Finished burning, disabled renderer and collider");
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;


            //Isburning vec1
            vineMaterial.SetFloat("_Vector1_D9BC0F3A", 0);

            //BurnLevel vec1
            vineMaterial.SetFloat("_Vector1_CFE875CF", 0);

            enabled = false;
            Destroy(gameObject);
        }
    }

    public void StartBurning()
    {
        isBurning = true;
        //if (GetComponent<Torch>().FireActive)
        {
        }
    }
}
