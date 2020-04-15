using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSpout : SpellInteractableObject 
{
    [SerializeField]
    GameObject icePrefab;

    GameObject currentIcePrefab;

    int freezeLevel;

    bool isFrozen;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public override void OnActivated(Spell spellType)
    {
        if (spellType.GetComponent<IceSpell>() != null)
        {
            if (freezeLevel >= 8)
            {
                currentIcePrefab = Instantiate(icePrefab, transform.position, transform.rotation);
                Debug.Log("WaterSpout::OnActivated(Spell)::WaterSpout activated");
            }
            else
            {
                freezeLevel++;
                Debug.Log("WaterSpout::OnActivated(Spell)::Waterspout is freezing");
            }
        }
    }
}
