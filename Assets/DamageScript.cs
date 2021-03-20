using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageScript : MonoBehaviour
{
    public Material damagedMaterial;
    public Material fixedMaterial;
    private bool reported = false;
    public float severity;
    float latitude;
    float longitude;
    MeshRenderer mr;
    int damagedLayer;
    private void Awake() {
        mr = GetComponent<MeshRenderer>();
        damagedLayer = LayerMask.NameToLayer("HullDamage");
    }
    public bool IsReported() {
        return reported;
    }
    public void Damage() {
        mr.material = damagedMaterial;
        gameObject.layer = damagedLayer;
    }
    public void SetReported(bool rep) {
        reported = rep;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
