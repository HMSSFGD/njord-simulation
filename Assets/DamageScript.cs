using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageScript : MonoBehaviour
{
    private bool reported = false;
    public float severity;
    float latitude;
    float longitude;
    void Start()
    {
        
    }
    public bool IsReported() {
        return reported;
    }
    public void SetReported(bool rep) {
        reported = rep;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
