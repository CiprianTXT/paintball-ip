using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

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

    PhotonView view;
    int my_id;

    private void Awake()
    {
        //Set objects
      
        
    }

    private void Start()
    {

        view = transform.GetComponent<PhotonView>();

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
            //player = GameObject.Find("Player").transform;


            int player_id = -1;
            Transform playerHolder = GameObject.Find("PlayerHolder").transform;
            int playerNo = (int)playerHolder.childCount;
            for (int i = 0; i < playerNo; i++)
            {
                if (playerHolder.GetChild(i).GetComponent<PhotonView>().IsMine)
                {
                    player_id = playerHolder.GetChild(i).GetComponent<PhotonView>().ViewID;
                    break;
                }
            }
            my_id = player_id;

            player = PhotonView.Find(player_id + 1).gameObject.transform;
            Debug.Log($"i should be {player} with {player_id}");



            Vector3 distanceToPlayer = player.position - transform.position;
            
            //player = PhotonNetwork.LocalPlayer.UserId;

            if (distanceToPlayer.magnitude <= pickUpRange)
            {
                // Get the gun closest to where the player was looking
                RaycastHit hit;
                fpsCam = player.parent.transform.Find("CameraHolder").GetComponentInChildren<Camera>().transform;
                Ray ray = fpsCam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                //Debug.Log($"aaaa {droppedGunContainer}  {gunContainer}");

                droppedGunContainer = PhotonView.Find(2).transform;

                gunContainer = PhotonView.Find(player_id + 2).transform;

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
                        int id = player.GetComponent<PhotonView>().ViewID/1000;
                        Debug.Log($"my id is {id}");
                        // Pick up the closest gun
                        view.RPC("PickUpWeapon", RpcTarget.All, closestGun.GetComponent<PhotonView>().ViewID, id);
                        //PickUpWeapon(closestGun, id);
                    }
                }
            }
            
        }

        // Drop if equipped and "Q" is pressed
        if (equipped && Input.GetKeyDown(KeyCode.Q))
        {
            //Drop();
            view.RPC("Drop", RpcTarget.All, my_id / 1000);
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


    [PunRPC]
    public void PickUpWeapon(int gun_id, int sender_id)
    {
        GameObject gun = PhotonView.Find(gun_id).gameObject;

        equipped = true;

        //Transform model = PhotonView.Find((int)data[0] * 1000 + 2).gameObject.transform.Find("PlayerModel");
        gunContainer = PhotonView.Find(sender_id * 1000 + 3).gameObject.transform;

        //Make weapon a child of the camera and move it to default position
        gun.transform.SetParent(gunContainer);
        gun.transform.localPosition = Vector3.zero;
        gun.transform.localRotation = Quaternion.Euler(Vector3.zero);
        gun.transform.localScale = Vector3.one;

        //Make Rigidbody kinematic and BoxCollider a trigger
        gun.transform.GetComponent<Rigidbody>().isKinematic = true;
        gun.transform.GetComponent<MeshCollider>().isTrigger = true;

        Transform sender = PhotonView.Find(sender_id * 1000 + 2).transform;


        Debug.Log($"{my_id} and was sent by {sender_id}");

        if (my_id / 1000 == sender_id)
        {

            ammunitionDisplay.enabled = true;
            actionDisplay.enabled = true;
            backgroundAmmo.enabled = true;

            
            slotFull = true;

            //Enable script
            Debug.Log("it should work");
            gunScript.enabled = true;
            playerRb = sender.GetComponent<Rigidbody>();
        }
        
    }

    [PunRPC]
    public void Drop(int player_id)
    {
        if (player_id == my_id / 1000)
        {
            ammunitionDisplay.enabled = false;
            actionDisplay.enabled = false;
            backgroundAmmo.enabled = false;

            slotFull = false;
        }
        

        equipped = false;
        

        //Set parent to droppedGunContainer
        transform.SetParent(droppedGunContainer);

        //Make Rigidbody not kinematic and BoxCollider normal
        rb.isKinematic = false;
        coll.isTrigger = false;

        Transform player = PhotonView.Find(player_id * 1000 + 2).transform;

        //Gun carries momentum of player
        rb.velocity = player.GetComponent<Rigidbody>().velocity;

        //AddForce
        rb.AddForce(player.Find("PlayerModel").transform.forward * dropForwardForce, ForceMode.Impulse);
        rb.AddForce(player.Find("PlayerModel").transform.forward * dropUpwardForce, ForceMode.Impulse);
        //Add random rotation
        float random = Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(random, random, random) * 10);

        //Disable script
        gunScript.enabled = false;
    }


}
