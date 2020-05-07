using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarsMelt : SpellInteractableObject
{
    bool isMelting;
    [SerializeField]
    Material barMat;

    [SerializeField]
    GameObject parent;

    public override void OnActivated(Spell spellType)
    {
        if (spellType.GetComponent<CorrosionSpell>() != null)
        {
            Debug.Log("BarsMelt::OnSpellActivated(Spell)::Melting");
            isMelting = true;
        }
    }

void Update()
{
    if (isMelting)
    {
        //Isburning vec1
        barMat.SetFloat("_Vector1_D9BC0F3A", 1);

        //BurnLevel vec1
        barMat.SetFloat("_Vector1_CFE875CF", Mathf.Min(barMat.GetFloat("_Vector1_CFE875CF") + 0.004f, 1));
    }
    if (barMat.GetFloat("_Vector1_CFE875CF") >= 0.38 && isMelting)
    {
        Debug.Log("BarsMelt::Update()::Finished melting, disabled renderer and collider");
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;


        //Isburning vec1
        barMat.SetFloat("_Vector1_D9BC0F3A", 0);

        //BurnLevel vec1
        barMat.SetFloat("_Vector1_CFE875CF", 0);

        enabled = false;
        Destroy(parent);
    }
}
}
