using UnityEngine;

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

    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        //When to explode:
        if (collisions > maxCollisions)
        {
            Destroy(gameObject);
            Explode();
        }
            

        //Count down lifetime
        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0) Explode();
    }

    private void Explode()
    {

        //Instantiate explosion
        if (explosion != null) Instantiate(explosion, transform.position, Quaternion.identity);


        //Check for enemies 
        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);
        for (int i = 0; i < enemies.Length; i++)
        {
            //Get component of enemy and call Take Damage

            //Just an example!

            PlayerStatsHandler statScript = enemies[i].GetComponentInParent<PlayerStatsHandler>();
            if(statScript && statScript.gameObject.transform != gameObject.transform.parent)
            {
                statScript.TakeDamage(explosionDamage);
                
            }
                

            //Add explosion force (if enemy has a rigidbody)
            //if (enemies[i].GetComponent<Rigidbody>())
                //enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRange);
        }

        //Add a little delay, just to make sure everything works fine
        Invoke("Delay", 0.05f);
    }
    private void Delay()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Don't count collisions with other bullets
        if (collision.collider.CompareTag("Bullet")) return;

        // Check if collision is with any child of the parent GameObject
        Transform parentTransform = transform.parent;
        if (parentTransform != null && collision.transform.IsChildOf(parentTransform)) return;
        
        //Count up collisions
        collisions++;

        //Explode if bullet hits an enemy directly and explodeOnTouch is activated
        if (collision.collider.CompareTag("Enemy") && explodeOnTouch) Explode();

        if (collision.collider.CompareTag("Breakable"))
        {
            Destroy(collision.gameObject);

            Destroy(gameObject);
        }
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
    }

    /// Just to visualize the explosion range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
