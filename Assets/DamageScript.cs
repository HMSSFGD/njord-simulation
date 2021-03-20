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
    int defaultLayer;
    public DamageScript(Material _damagedMaterial, Material _fixedMaterial, float _severity, float _latitude, float _longitude, MeshRenderer _mr, bool _enabled){
        damagedMaterial = _damagedMaterial;
        fixedMaterial = _fixedMaterial;
        severity = _severity;
        latitude = _latitude;
        longitude = _longitude;
        mr = _mr;
        damagedLayer = LayerMask.NameToLayer("HullDamage");
        defaultLayer = LayerMask.NameToLayer("Default");
        SetEnabled(_enabled);
    }

    public void SetEnabled(bool enabled){
        //mr.gameObject.SetActive(enabled);
        if (enabled){
            mr.material = damagedMaterial;
            mr.gameObject.layer = damagedLayer;
        } else {
            mr.material = fixedMaterial;
            mr.gameObject.layer = defaultLayer;
        }
    }
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
}
