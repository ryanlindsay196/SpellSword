using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : MonoBehaviour
{
    public bool isTargeted;
    [SerializeField]
    SkinnedMeshRenderer meshRenderer;
    [SerializeField]
    Material meshMat;
    float glowLevel;
    // Start is called before the first frame update
    void Start()
    {
        if(meshRenderer == null)
            meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if(meshMat == null && meshRenderer != null)
            meshMat = meshRenderer.material;
        isTargeted = false;
    }

    private void FixedUpdate()
    {
        if(!isTargeted)
        {
            //Debug.Log("Targeted false");
            ResetGlow();
        }
    }

    public void ResetGlow()
    {
        if(meshMat.name == "GlowMat (Instance)")
        {
            glowLevel = Mathf.Lerp(glowLevel, 0, 0.28f);
            //meshMat.SetFloat("_GlowIntensity", Mathf.Lerp(meshMat.GetFloat("_GlowIntensity"), 0, 0.28f));
            meshMat.SetFloat("_GlowIntensity", glowLevel);
            //Debug.Log("ResetGlow: " + meshMat.GetFloat("_GlowIntensity"));
            //Graphics.DrawMeshNow(GetComponent<Mesh>(), gameObject.transform.position, gameObject.transform.rotation);
        }
        isTargeted = false;
    }

    public void LerpGlow()
    {
        if (meshMat != null && meshMat.name == "GlowMat (Instance)" && isTargeted)
        {
            glowLevel = Mathf.Lerp(glowLevel, 30, 0.08f);
            //meshMat.SetFloat("_GlowIntensity", Mathf.Lerp(meshMat.GetFloat("_GlowIntensity"), 1, 0.08f));
            meshMat.SetFloat("_GlowIntensity", glowLevel);
            //Graphics.DrawMeshNow(meshRenderer.GetComponent<Mesh>(), gameObject.transform.position, gameObject.transform.rotation);
        }
    }
}
