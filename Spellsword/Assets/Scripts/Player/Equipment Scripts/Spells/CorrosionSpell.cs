using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrosionSpell : Spell
{
    [SerializeField]
    LayerMask floorLayerMask;

    [SerializeField]
    float spellLifeTime;
    
    AudioSource audioSource;
    [SerializeField]
    AudioClip startupAudioClip, hitAudioClip;

    [SerializeField]
    ParticleSystem[] corrosiveExplosionElements;
    // Start is called before the first frame update
    void Start()
    {
        RaycastHit hit;
        audioSource = GetComponent<AudioSource>();

        for (int i = 0; i < corrosiveExplosionElements.Length; i++)
        {
            corrosiveExplosionElements[i].Play();
        }

        if (Physics.Raycast(transform.position + Camera.main.transform.forward * 3, Vector3.down, out hit, 15, floorLayerMask))
        {
            transform.position = hit.point;// + new Vector3(0, 2, 0);
            transform.rotation = Quaternion.identity;

            return;
        }
        //for(int i = 0; i < corrosiveExplosionElements.Length; i++)
        //{
        //    corrosiveExplosionElements[i].Stop();
        //}

    }

    // Update is called once per frame
    void Update()
    {
        spellLifeTime -= Time.deltaTime;
        if (spellLifeTime <= 0)
            Destroy(gameObject);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        Debug.Log("Corrosion hit enemy");
        audioSource.clip = hitAudioClip;
        audioSource.Play();
    }
}
