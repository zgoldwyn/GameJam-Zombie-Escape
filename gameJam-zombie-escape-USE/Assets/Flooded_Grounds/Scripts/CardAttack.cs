using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsScript : MonoBehaviour{
    [SerializeField] public GameObject[] cardArr;

    [SerializeField] public float shootForce, upwardForce;

    [SerializeField] public float timeBetweenShooting, spread, timeBetweenShots;
    bool shooting, readyToShoot, allowInvoke, hasShot;
    [SerializeField] public Camera fpsCam;
    [SerializeField] public Transform attackPoint;

    public GameObject currentCard;

    private void Awake(){

    }

    private void Update(){
        MyInput();
    }

    private void MyInput(){
        shooting = Input.GetKey(KeyCode.Mouse0);
        
        if (shooting && !hasShot){
            Shoot();
            hasShot = true;
        }

        if (!shooting){
            hasShot = false;
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
        for(int n=0; n < cardArr.Length; n++){
            if(!cardArr[n].activeInHierarchy){
                cardArr[n].SetActive(true);
                cardArr[n].transform.position = new Vector3(attackPoint.position.x + .1f, attackPoint.position.y, attackPoint.position.z + .1f);
                currentCard = cardArr[n];
                break;
            }
        }

        currentCard.transform.forward = directionWithoutSpread.normalized;

        currentCard.GetComponent<Rigidbody>().AddForce(directionWithoutSpread.normalized * shootForce, ForceMode.Impulse);
        currentCard.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);

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