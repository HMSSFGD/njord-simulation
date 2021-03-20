using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class DamageManager : MonoBehaviour
{
    // Start is called before the first frame update
    List<DamageScript> children;
    public float damageChance = 0.1f;
    public float severityMin = 0.0f;
    public float severityMax = 1.0f;
    public float centreLat = 54.5f;
    public float centreLong = 45.5f;
    float sphereRadius = 15f;
    Vector3 sphereCentre;
    void Start()
    {
        sphereCentre = transform.position;
        children = GetComponentsInChildren<DamageScript>().OfType<DamageScript>().ToList(); //yes
        Debug.Log(children.Count);
        foreach (DamageScript kid in children){
            bool damaged = Random.Range(0f, 1f) < damageChance;
            Vector3 location = kid.gameObject.GetComponent<MeshCollider>().bounds.center;
            float lati = centreLat + (location.x-sphereCentre.x)/10;
            float longi = centreLong + (location.z-sphereCentre.z)/10;
            kid.latitude = lati;
            kid.longitude = longi;
            if (damaged) {
                Debug.Log("damaged");
                kid.Damage(); 
                kid.severity = Random.Range(severityMin, severityMax);
            }
            Debug.Log("Damage enabled? " + kid.gameObject.activeSelf);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}