using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.UI.Image;
using UnityEditor.PackageManager;
using Unity.VisualScripting;

/// Thanks for downloading my custom bullets/projectiles script! :D
/// Feel free to use it in any project you like!
/// 
/// The code is fully commented but if you still have any questions
/// don't hesitate to write a yt comment
/// or use the #coding-problems channel of my discord server
/// 
/// Dave
public class CustomBullet : MonoBehaviour
{
    //Assignables
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask whatIsEnemies;

    //Stats
    [Range(0f,1f)]
    public float bounciness;
    public bool useGravity;
    public Color paintColor;

    //Damage
    public int explosionDamage;
    public float explosionRange;
    public float explosionForce;

    //Lifetime
    public int maxCollisions;
    public float maxLifetime;
    public bool explodeOnTouch = true;

    int collisions;
    PhysicMaterial physics_mat;
    public DecalProjector dp;
    private Transform decalHolder;
    public int damage = 0;

    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        //When to explode:
        if (collisions > maxCollisions)
        {
            GetComponent<MeshRenderer>().enabled = false;
            Explode();
            Destroy(gameObject);
        }
            

        //Count down lifetime
        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0) Explode();
    }

    private void Explode()
    {
        // Instantiate explosion
        if (explosion != null) Instantiate(explosion, transform.position, Quaternion.identity);

        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);

        foreach (var enemy in enemies)
        {
            // Get the enemy's collider
            Collider enemyCollider = enemy.GetComponent<Collider>();
            if (enemyCollider == null) continue;
            
        }
        
        //decal.transform.rotation = Quaternion.LookRotation(-transform.forward, transform.up - transform.forward);

        // Add a little delay, just to make sure everything works fine
        Invoke("Delay", 0.05f);
    }




    private void Delay()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Don't count collisions with other bullets
        if (collision.collider.CompareTag("Bullet")) return;

        // Check if collision is with any child of the parent GameObject
        Transform parentTransform = transform.parent;
        if (parentTransform != null && collision.transform.IsChildOf(parentTransform)) return;

        // Count up collisions
        collisions++;


        // Explode if bullet hits an enemy directly and explodeOnTouch is activated
        if (collision.collider.CompareTag("Player") && explodeOnTouch)
        {
            //Explode();
            PlayerStatsHandler stats = collision.gameObject.GetComponent<PlayerStatsHandler>();
            if (stats != null)
            {
                stats.TakeDamage(damage);
            }
            Destroy(gameObject);
            return;
        }

        if (collision.collider.CompareTag("Breakable"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
            return;
        }


        // Get the collision point and normal
        ContactPoint contact = collision.GetContact(0);
        Vector3 hitNormal = contact.normal;

        // Define the distance you want the decal to be from the wall
        float distanceFromWall = 0.1f;

        // Calculate the position a bit further from the wall along the normal
        Vector3 decalPosition = contact.point + hitNormal * distanceFromWall;

        // Calculate rotation to align decal with the hit normal
        Quaternion decalRotation = Quaternion.LookRotation(-hitNormal);

        // Add slight rotation around the forward vector
        float rotationAngle = Random.Range(0f, 360f);
        Quaternion rotationOffset = Quaternion.AngleAxis(rotationAngle, Vector3.forward);
        decalRotation *= rotationOffset;

        // Instantiate color splash with position and rotation
        DecalProjector decal = Instantiate(dp, decalPosition, decalRotation);
        decal.GetComponent<DecalColorSetter>().splashColor = paintColor;

        decal.transform.parent = decalHolder;
        
    }





    private void Setup()
    {
        //Create new material
        Material mat = new Material(GetComponent<MeshRenderer>().material);
        mat.color = paintColor;
        GetComponent<MeshRenderer>().material = mat;

        //Create a new Physic material
        physics_mat = new PhysicMaterial();
        physics_mat.bounciness = bounciness;
        physics_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        physics_mat.bounceCombine = PhysicMaterialCombine.Maximum;
        //Assign material to collider
        GetComponent<SphereCollider>().material = physics_mat;

        //Set gravity
        rb.useGravity = useGravity;

        decalHolder = GameObject.Find("DecalHolder").transform;

    }

    /// Just to visualize the explosion range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
