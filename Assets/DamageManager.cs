using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// public class DamagePt : MonoBehaviour{
//     public Material damagedMaterial;
//     public Material fixedMaterial;
//     private bool reported = false;
//     public float severity;
//     float latitude;
//     float longitude;
//     MeshRenderer mr;
//     int damagedLayer;
//     int defaultLayer;
//     public DamagePt(Material _damagedMaterial, Material _fixedMaterial, float _severity, float _latitude, float _longitude, MeshRenderer _mr, bool _enabled){
//         damagedMaterial = _damagedMaterial;
//         fixedMaterial = _fixedMaterial;
//         severity = _severity;
//         latitude = _latitude;
//         longitude = _longitude;
//         mr = _mr;
//         damagedLayer = LayerMask.NameToLayer("HullDamage");
//         defaultLayer = LayerMask.NameToLayer("Default");
//         SetEnabled(_enabled);
//     }

//     public void SetEnabled(bool enabled){
//         //mr.gameObject.SetActive(enabled);
//         if (enabled){
//             mr.material = damagedMaterial;
//             mr.gameObject.layer = damagedLayer;
//         } else {
//             mr.material = fixedMaterial;
//             mr.gameObject.layer = defaultLayer;
//         }
//     }
//     private void Awake() {
//         mr = GetComponent<MeshRenderer>();
//         damagedLayer = LayerMask.NameToLayer("HullDamage");
//     }
//     public bool IsReported() {
//         return reported;
//     }
//     public void Damage() {
//         mr.material = damagedMaterial;
//         gameObject.layer = damagedLayer;
//     }
//     public void SetReported(bool rep) {
//         reported = rep;
//     }
// }
public class DamageManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Material damagedMaterial;
    public Material fixedMaterial;
    List<GameObject> children = new List<GameObject>();
    public float damageChance = 0.1f;
    public float severityMin = 0.0f;
    public float severityMax = 1.0f;
    void Start()
    {
        MeshRenderer[] childObjects = GetComponentsInChildren<MeshRenderer>();
        //children = GetComponentsInChildren<GameObject>().OfType<GameObject>().ToList(); //yes
        Debug.Log(children.Count);
        foreach (MeshRenderer renderer in childObjects){
            
            bool damaged = Random.Range(0f, 1f) < damageChance;
            float severity = 0f;
            float latitude = 0f;
            float longitude = 0f;
            if (damaged) {
                Debug.Log("damaged");
                severity = Random.Range(severityMin, severityMax);
            }
            DamageScript pt = new DamageScript(damagedMaterial, fixedMaterial, severity, latitude, longitude, renderer, damaged);
            Debug.Log("Damage enabled? " + damaged);
        }
        // children = GetComponentsInChildren<DamageScript>().OfType<DamageScript>().ToList(); //yes
        // Debug.Log(children.Count);
        // foreach (DamageScript kid in children){
        //     bool damaged = Random.Range(0f, 1f) < damageChance;
        //     if (damaged) {
        //         Debug.Log("damaged");
        //         kid.Damage(); 
        //         kid.severity = Random.Range(severityMin, severityMax);
        //     }
        //     Debug.Log("Damage enabled? " + kid.gameObject.activeSelf);
        // }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
