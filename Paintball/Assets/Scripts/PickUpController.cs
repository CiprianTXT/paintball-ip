using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickUpController : MonoBehaviour
{
    private ProjectileGun gunScript;
    private Rigidbody rb;
    private MeshCollider coll;
    private Transform player, gunContainer;
    public Transform fpsCam;
    public Rigidbody playerRb;
    private Transform droppedGunContainer;

    private TextMeshProUGUI ammunitionDisplay;
    private TextMeshProUGUI actionDisplay;
    private Image backgroundAmmo;

    public float pickUpRange = 4;
    public float dropForwardForce = 7, dropUpwardForce = 3;

    public bool equipped;
    public static bool slotFull;

    private void Awake()
    {
        //Set objects
      
        
    }

    private void Start()
    {

        gunScript = gameObject.GetComponent<ProjectileGun>();
        rb = gameObject.GetComponent<Rigidbody>();
        coll = gameObject.GetComponent<MeshCollider>();

        // Find UI children
        Transform uiCanvas = GameObject.Find("UI").transform;
        ammunitionDisplay = uiCanvas.Find("BulletDisplay").GetComponent<TextMeshProUGUI>();
        actionDisplay = uiCanvas.Find("ActionDisplay").GetComponent<TextMeshProUGUI>();
        backgroundAmmo = uiCanvas.Find("BackgroundDisplayBullets").GetComponent<Image>();

        //Setup
        if (!equipped)
        {
            //Debug.Log($"gunscript is {gunScript} and is {gunScript.enabled}");
            gunScript.enabled = false;
            rb.isKinematic = false;
            coll.isTrigger = false;
        }
        if (equipped)
        {
            gunScript.enabled = true;
            rb.isKinematic = true;
            coll.isTrigger = true;
            slotFull = true;
        }
        

    }

    private void Update()
    {

        /*if (player.GetComponent<PlayerStatsHandler>().IsDead())
        {
            return;
        }*/

        //Check if player is in range and "E" is pressed
        if (!equipped && Input.GetKeyDown(KeyCode.E) && !slotFull)
        {
            player = GameObject.Find("Player").transform;
            Vector3 distanceToPlayer = player.position - transform.position;

            if (distanceToPlayer.magnitude <= pickUpRange)
            {
                // Get the gun closest to where the player was looking
                RaycastHit hit;
                fpsCam = GameObject.Find("CameraHolder").GetComponentInChildren<Camera>().transform;
                Ray ray = fpsCam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                //Debug.Log($"aaaa {droppedGunContainer}  {gunContainer}");
                if(droppedGunContainer == null)
                    droppedGunContainer = GameObject.Find("DroppedGunHolder").transform;
                if(gunContainer == null)
                    gunContainer = GameObject.Find("GunHolder").transform;
                //Debug.Log($"bbbb {droppedGunContainer}  {gunContainer}");
                //Debug.Log("aalalalalalalalala");
                if (Physics.Raycast(ray, out hit))
                {
                    // Check if the object hit is in the droppedGunContainer
                    Vector3 hitPoint = hit.point;
                    //Debug.Log($"{droppedGunContainer}");
                    //Debug.Log($"{ hit.point}");
                    GameObject closestGun = GetClosestGunToHit(droppedGunContainer.transform, hit.point);
                    if (closestGun != null)
                    {
                        // Pick up the closest gun
                        PickUpWeapon(closestGun);
                    }
                }
            }
            
        }

        // Drop if equipped and "Q" is pressed
        if (equipped && Input.GetKeyDown(KeyCode.Q))
        {
            Drop();
        }
    }

    private GameObject GetClosestGunToHit(Transform container, Vector3 hitPos)
    {
        GameObject closestGun = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform child in container)
        {
            float distance = Vector3.Distance(hitPos, child.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestGun = child.gameObject;
            }
        }

        if (closestGun && gameObject.name == closestGun.name)
        {
            return closestGun;
        }

        return null;
    }


    private void PickUpWeapon(GameObject gun)
    {

        ammunitionDisplay.enabled = true;
        actionDisplay.enabled = true;
        backgroundAmmo.enabled = true;

        equipped = true;
        slotFull = true;

        //Make weapon a child of the camera and move it to default position
        gun.transform.SetParent(gunContainer);
        gun.transform.localPosition = Vector3.zero;
        gun.transform.localRotation = Quaternion.Euler(Vector3.zero);
        gun.transform.localScale = Vector3.one;

        //Make Rigidbody kinematic and BoxCollider a trigger
        gun.transform.GetComponent<Rigidbody>().isKinematic = true;
        gun.transform.GetComponent<MeshCollider>().isTrigger = true;

        //Enable script
        gunScript.enabled = true;

        
        playerRb = player.GetComponent<Rigidbody>();
    }

    public void Drop()
    {
        ammunitionDisplay.enabled = false;
        actionDisplay.enabled = false;
        backgroundAmmo.enabled = false;

        equipped = false;
        slotFull = false;

        //Set parent to droppedGunContainer
        transform.SetParent(droppedGunContainer);

        //Make Rigidbody not kinematic and BoxCollider normal
        rb.isKinematic = false;
        coll.isTrigger = false;

        //Gun carries momentum of player
        rb.velocity = player.GetComponent<Rigidbody>().velocity;

        //AddForce
        rb.AddForce(fpsCam.forward * dropForwardForce, ForceMode.Impulse);
        rb.AddForce(fpsCam.up * dropUpwardForce, ForceMode.Impulse);
        //Add random rotation
        float random = Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(random, random, random) * 10);

        //Disable script
        gunScript.enabled = false;
    }


}
