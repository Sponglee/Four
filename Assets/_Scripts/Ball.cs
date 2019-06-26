using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class Ball : MonoBehaviour
{
    [SerializeField]
    public /*static*/ bool Move = false;
    [SerializeField] float jumpStrength = 100;
    [SerializeField] float gravityForce = 10;

    LevelManager level;
    Rigidbody rb;
    float nextBallPosToJump;
    int skippedCounter = 0;
    float vel;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        level = FindObjectOfType<LevelManager>();

        nextBallPosToJump = -level.spawnOffset /*+ GetComponent<SphereCollider>().bounds.size.y / 2*/ + level.spawnOffsetStep/2;

        Debug.Log(nextBallPosToJump);
    }

    void Update()
    {
        //Debug.DrawLine(transform.position, transform.position + Vector3.down * cylinder.distanceBtwCircles / 2);
    }

    void FixedUpdate()
    {
        if (!Move)
            return;

        vel = -gravityForce * Time.deltaTime;

        float overlap = nextBallPosToJump - (transform.position.y + vel);
        if (overlap >= 0)
        {
            transform.Translate(Vector3.up * (vel + overlap));
            CheckCollision();
        }
        transform.Translate(Vector3.up * vel);
    }

    void CheckCollision()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, level.spawnOffsetStep/ 2,
            LayerMask.GetMask("Circles")))
        {
            if (hit.collider.CompareTag("Cart"))
            {
                if (skippedCounter >= 2)
                {
                    //// TODO: Apply good-looking break force.
                    //if (hit.collider.transform.parent.CompareTag("Cylinder Object"))
                    //{
                    //    Destroy(hit.collider.gameObject);
                    //}
                    //else
                    //{
                    //    Destroy(hit.collider.transform.parent.gameObject);
                    //}
                }

                skippedCounter = 0;
                Jump();
                Debug.Log("Good.");
            }
            //else if (hit.collider.CompareTag("Bad"))
            //{
            //    Debug.Log("END GAME.");
            //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            //}
            //else if (hit.collider.CompareTag("Finish"))
            //{
            //    Debug.Log("YOU WON.");
            //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            //}
            else
            {
                Debug.LogWarning("COLLIDED WITH UNKNOWN OBJECT.");
            }
        }
        else
        {
            ++skippedCounter;
            nextBallPosToJump -= level.spawnOffsetStep;
        }
    }

    void Jump()
    {
        Debug.Log("Jump");
        vel = jumpStrength;
    }

    void Stop()
    {
        Move = false;
        vel = 0;
    }
}