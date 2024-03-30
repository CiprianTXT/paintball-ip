﻿
using UnityEngine;
using TMPro;
using System.Collections;

/// Thanks for downloading my projectile gun script! :D
/// Feel free to use it in any project you like!
/// 
/// The code is fully commented but if you still have any questions
/// don't hesitate to write a yt comment
/// or use the #coding-problems channel of my discord server
/// 
/// Dave
public class ProjectileGun : MonoBehaviour
{
    //bullet 
    public GameObject bullet;
    public int bulletDamage;

    //bullet force
    public float shootForce, upwardForce;

    //Gun stats
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap, noOfMagazines;
    public bool allowButtonHold;

    int bulletsLeft, bulletsShot;

    //Recoil
    private Rigidbody playerRb;
    public float recoilForce;

    //bools
    bool shooting, readyToShoot, reloading;

    //Reference
    private static Camera fpsCam;
    public Transform attackPoint;

    //Graphics
    public GameObject muzzleFlash;
    private TextMeshProUGUI ammunitionDisplay;
    private TextMeshProUGUI actionDisplay;

    //bug fixing :D
    public bool allowInvoke = true;
    private Material[] materials;

    private void Awake()
    {
        //make sure magazine is full

        bulletsLeft = magazineSize;
        readyToShoot = true;

        // Find fpsCam
        fpsCam = GameObject.Find("CameraHolder").GetComponentInChildren<Camera>();

        // Find UI children
        Transform uiCanvas = GameObject.Find("UI").transform;
        ammunitionDisplay = uiCanvas.Find("BulletDisplay").GetComponent<TextMeshProUGUI>();
        actionDisplay = uiCanvas.Find("ActionDisplay").GetComponent<TextMeshProUGUI>();

        //Find player RB
        playerRb = GameObject.Find("Player").GetComponent<Rigidbody>();

        //Extract player materials
        materials = playerRb.transform.Find("PlayerModel").GetComponent<MeshRenderer>().materials;
    }

    private void Update()
    {

        //Set ammo display, if it exists :D
        actionDisplay.color = Color.white;
        if (ammunitionDisplay != null)
        {
            if (bulletsLeft / bulletsPerTap == 0 && reloading == false)
            {
                actionDisplay.color = Color.red;
                if (noOfMagazines > 0)
                    actionDisplay.SetText("Reload\n(" + noOfMagazines + " available)");
                else
                    actionDisplay.SetText("Empty");

            }
            else if (reloading == true)
            {
                actionDisplay.SetText(" Reloading...");
            }
            else
            {
                actionDisplay.SetText("");
            }
            ammunitionDisplay.SetText(bulletsLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);
        }

        MyInput();
        transform.eulerAngles = fpsCam.transform.eulerAngles;
    }



    private void MyInput()
    {
        //Check if allowed to hold down button and take corresponding input
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        //Reloading 
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();
        //Reload automatically when trying to shoot without ammo
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0) Reload();

        //Shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            //Set bullets shot to 0
            bulletsShot = 0;

            Shoot();
        }

    }

    private void Shoot()
    {
        readyToShoot = false;

        //Find the exact hit position using a raycast
        Ray ray = fpsCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        //check if ray hits something
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75); //Just a point far away from the player

        //Calculate direction from attackPoint to targetPoint
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        //Calculate spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calculate new direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0); //Just add spread to last direction

        //Instantiate bullet/projectile
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity, playerRb.transform); //store instantiated bullet in currentBullet
        //Rotate bullet to shoot direction
        currentBullet.transform.forward = directionWithSpread.normalized;

        currentBullet.GetComponent<CustomBullet>().damage = bulletDamage;


        //Color the bullet accordingly


        Color col = new Color(0, 0, 0);
        bool ok = false;
        foreach (Material mat in materials)
        {
            if (mat.name == "suit_material (Instance)")
            {
                col = mat.color;
                ok = true;
                break;
            }
        }

        if (!ok)
        {
            Debug.Log("error finding col");
        }
        else
        {
            currentBullet.GetComponent<CustomBullet>().paintColor = col;
        }
       

        //Add forces to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);

        //Instantiate muzzle flash, if you have one
        if (muzzleFlash != null)
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

        bulletsLeft--;
        bulletsShot++;

        //Invoke resetShot function (if not already invoked), with your timeBetweenShooting
        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;

            //Add recoil to player (should only be called once)
            playerRb.AddForce(-directionWithSpread.normalized * recoilForce, ForceMode.Impulse);
        }

        //if more than one bulletsPerTap make sure to repeat shoot function
        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);
    }
    private void ResetShot()
    {
        //Allow shooting and invoking again
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        if (noOfMagazines > 0)
        {
            reloading = true;
            noOfMagazines -= 1;
            Invoke("ReloadFinished", reloadTime); //Invoke ReloadFinished function with your reloadTime as delay
        }
        
    }
    private void ReloadFinished()
    {
        //Fill magazine
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
