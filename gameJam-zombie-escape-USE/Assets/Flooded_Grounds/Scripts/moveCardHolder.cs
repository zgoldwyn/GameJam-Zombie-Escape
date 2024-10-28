using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveCardHolder : MonoBehaviour
{
    // Start is called before the first frame update
   public Transform cardHolderPosition;

   private void Update()
   {
    transform.position = cardHolderPosition.position;
   }
   
}
