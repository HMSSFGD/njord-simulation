using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class DamageManager : MonoBehaviour
{
    // Start is called before the first frame update
    List<DamageScript> children;
    public GameObject domeParent;
    public float damageChance = 0.1f;
    public float severityMin = 0.0f;
    public float severityMax = 1.0f;
    public float centreLat = 54.5f;
    public float centreLong = 45.5f;
    float sphereRadius = 15f;
    float decayTimerMax;
    float decayTimer;
    public float decayTimerMinRange = 25.0f;
    public float decayTimerMaxRange = 35.0f;
    Vector3 sphereCentre;
    void Start()
    {
        decayTimer = Random.Range(decayTimerMinRange, decayTimerMaxRange);;
        sphereCentre = transform.position;
        children = domeParent.GetComponentsInChildren<DamageScript>().OfType<DamageScript>().ToList(); //yes
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

    void DecayRandom(){
        int index = Random.Range(0, children.Count);
        int startIndex = index;
        while (children[index].damaged){
            index = (index+1)%children.Count;
            Debug.Log("Searching for undamaged index.");
            if (index == startIndex){
                Debug.Log("Could not find undamaged panel.");
                return;
            }
        }
        children[index].Damage();
    }

    // Update is called once per frame
    void Update()
    {
        decayTimer -= Time.deltaTime;
        if (decayTimer < 0){
            DecayRandom();
            decayTimerMax = Random.Range(decayTimerMinRange, decayTimerMaxRange);
            decayTimer = decayTimerMax;
        }
    }
}