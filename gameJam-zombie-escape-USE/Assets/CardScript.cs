using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardScript : MonoBehaviour
{
    private float lifetime;
    void Update()
    {
        lifetime += Time.deltaTime;
        if (lifetime > 5)
        {
            lifetime = 0;
            gameObject.SetActive(false);
            
        }
    }
}
