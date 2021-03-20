using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageScript : MonoBehaviour
{
    private bool reported = false;
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
