using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public float spwanTime = 3f;
    public float curTime;
   
    public GameObject makeWeapon;

    
    
    void Update()
    {
      
        if (curTime >= spwanTime)
        {
            Instantiate(makeWeapon);
            curTime = 0;

        }
        curTime += Time.deltaTime;
    }

 
}
