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
    private Transform player, gunContainer, fpsCam;

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
        fpsCam = GameObject.Find("CameraHolder").GetComponentInChildren<Camera>().transform;
        gunScript = this.gameObject.GetComponent<ProjectileGun>();
        rb = this.gameObject.GetComponent<Rigidbody>();
        coll = this.gameObject.GetComponent<MeshCollider>();
        player = GameObject.Find("Player").transform;
        gunContainer = GameObject.Find("GunHolder").transform;

        // Find UI children
        Transform uiCanvas = GameObject.Find("UI").transform;
        ammunitionDisplay = uiCanvas.Find("BulletDisplay").GetComponent<TextMeshProUGUI>();
        actionDisplay = uiCanvas.Find("ActionDisplay").GetComponent<TextMeshProUGUI>();
        backgroundAmmo = uiCanvas.Find("BackgroundDisplayBullets").GetComponent<Image>();
    }

    private void Start()
    {
        //Setup
        if (!equipped)
        {
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

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //Check if player is in range and "E" is pressed
        Vector3 distanceToPlayer = player.position - transform.position;
        if (!equipped && distanceToPlayer.magnitude <= pickUpRange && Input.GetKeyDown(KeyCode.E) && !slotFull) PickUp();

        //Drop if equipped and "Q" is pressed
        if (equipped && Input.GetKeyDown(KeyCode.Q)) Drop();
    }

    private void PickUp()
    {

        ammunitionDisplay.enabled = true;
        actionDisplay.enabled = true;
        backgroundAmmo.enabled = true;

        equipped = true;
        slotFull = true;

        //Make weapon a child of the camera and move it to default position
        transform.SetParent(gunContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;

        //Make Rigidbody kinematic and BoxCollider a trigger
        rb.isKinematic = true;
        coll.isTrigger = true;

        //Enable script
        gunScript.enabled = true;
    }

    public void Drop()
    {
        ammunitionDisplay.enabled = false;
        actionDisplay.enabled = false;
        backgroundAmmo.enabled = false;

        equipped = false;
        slotFull = false;

        //Set parent to null
        transform.SetParent(null);

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
