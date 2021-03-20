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
        foreach (DamageScript kid in children){
            kid.gameObject.SetActive(Random.Range(0f, 1f) < damageChance);
            Debug.Log("Damage enabled? " + kid.gameObject.activeSelf);
            kid.severity = Random.Range(severityMin, severityMax);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
