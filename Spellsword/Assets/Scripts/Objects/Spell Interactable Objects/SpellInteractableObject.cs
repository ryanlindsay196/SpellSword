using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellInteractableObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnActivated(Spell spellType)
    {

        Debug.Log("SpellInteractableObject::OnActivated(Spell)::This function doesn't exist in the " + this.GetType().FullName + " class");
    }
}
