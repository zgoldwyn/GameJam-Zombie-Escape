using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsScript : MonoBehaviour{
    [SerializeField] public GameObject card;

    [SerializeField] public float shootForce, upwardForce;

    [SerializeField] public float timeBetweenShooting, spread, timeBetweenShots;
    [SerializeField] bool shooting, readyToShoot, allowInvoke;
    [SerializeField] public Camera fpsCam;
    [SerializeField] public Transform attackPoint;

    private void Awake(){

    }

    private void Update(){
        MyInput();
    }

    private void MyInput(){
        shooting = Input.GetKey(KeyCode.Mouse0);

        if (shooting){
            Shoot();
        }
    }

    private void Shoot(){
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;

        if(Physics.Raycast(ray, out hit)){
            targetPoint = hit.point;
        }
        else{
            targetPoint = ray.GetPoint(75);
        }

        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        GameObject curentCard = Instantiate(card, attackPoint.position, Quaternion.identity);

        curentCard.transform.forward = directionWithoutSpread.normalized;

        curentCard.GetComponent<Rigidbody>().AddForce(directionWithoutSpread.normalized * shootForce, ForceMode.Impulse);
        curentCard.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);

        if (allowInvoke){
                Invoke("ResetShot", timeBetweenShooting);
                allowInvoke = false;
        }


    }

    private void ResetShot(){
        readyToShoot = true;
        allowInvoke = true;
    }
}