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
    void Start()
    {
        children = GetComponentsInChildren<DamageScript>().OfType<DamageScript>().ToList(); //yes
        Debug.Log(children.Count);
        foreach (DamageScript kid in children){
            bool damaged = Random.Range(0f, 1f) < damageChance;
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
