using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageScript : MonoBehaviour
{
    public Material damagedMaterial;
    public Material fixedMaterial;
    private bool reported = false;
    public float severity;
    public float latitude;
    public float longitude;
    MeshRenderer mr;
    MeshCollider collider;
    public bool damaged = false;
    int damagedLayer;
    int fixedLayer;
    private void Awake() {
        collider = GetComponent<MeshCollider>();
        if (collider.bounds.center.y < -0.1) {
            gameObject.SetActive(false);
        }
        mr = GetComponent<MeshRenderer>();
        damagedLayer = LayerMask.NameToLayer("HullDamage");
        fixedLayer = LayerMask.NameToLayer("Default");
    }
    public bool IsReported() {
        return reported;
    }
    public void Damage() {
        damaged = true;
        mr.material = damagedMaterial;
        gameObject.layer = damagedLayer;
    }
    public void UnDamage(){
        damaged = false;
        mr.material = fixedMaterial;
        gameObject.layer = fixedLayer;
        reported = false;
    }
    public void SetReported(bool rep) {
        reported = rep;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}