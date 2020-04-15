using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : SpellInteractableObject
{
    [SerializeField]
    ParticleSystem fireParticle;

    bool fireActive;
    public bool FireActive
    {
        get { return fireActive; }
    }
    // Start is called before the first frame update
    void Start()
    {
        if(GetComponent<Vines>() == null)
            fireParticle.Pause();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnActivated(Spell spellType)
    {
        if (GetComponent<Vines>() == null)
        {
            if (spellType.GetComponent<Fireball>() != null)
            {
                Debug.Log("Torch::OnActivated(Spell)::Torch activated");
                fireActive = true;
                fireParticle.Play();
            }
        }
        else
        {
            Debug.Log("Torch::OnSpellActivated(Spell)::Burn vines");
            fireActive = true;
            GetComponent<Vines>().StartBurning();
        }
    }
}
