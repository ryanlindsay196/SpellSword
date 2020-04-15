using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAbilities : MonoBehaviour
{

    public LayerMask collidingLayer;
    public GameObject vfxMarkerPrefab;

    public float vfxOffset;

    private bool aiming = false;
    private GameObject vfxMarker;
    private GameObject effectToSpawn;

    public List<GameObject> VFX;

    // Start is called before the first frame update
    void Start()
    {
        vfxMarker = Instantiate(vfxMarkerPrefab) as GameObject;
        vfxMarker.SetActive(false);

        if(VFX.Count > 0)
        {
        effectToSpawn = VFX[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
       if(Input.GetMouseButtonDown(1))
        {
            aiming = true;
            vfxMarker.SetActive(true);
        }

       if(Input.GetMouseButtonUp(1))
        {
            aiming = false;
            vfxMarker.SetActive(false);
        }

       if(Input.GetKeyDown(KeyCode.C))
        {
            if(aiming)
            {
                GameObject vfx = Instantiate(effectToSpawn, vfxMarker.transform.position, Quaternion.identity) as GameObject;
                Destroy(vfx, 5);
            }
        }

       if(aiming)
        {
            RaycastHit hit;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            if(Physics.Raycast(ray, out hit, Mathf.Infinity, collidingLayer))
            {
                vfxMarker.SetActive(true);
                vfxMarker.transform.position = hit.point;
            }
            else
            {
                vfxMarker.SetActive(false);
            }
        }
    }
}
