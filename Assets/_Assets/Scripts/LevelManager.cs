using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct LevelAnglePtr
{
    public float ptrAngle;
    public int ptrLevel;
}

public class LevelManager : Singleton<LevelManager> {

    //Level Generator vars
    public GameObject levelPrefab;
    public GameObject bottomPrefab;
    public GameObject blankCartPrefab;
    public int spawnOffset = 0;

    [SerializeField]
    private int level;

    public int Level
    {
        get
        {
            return level;
        }

        set
        {
            //set a level with angle to rotate later
            //LevelAnglePtr tmp;
            //tmp.ptrAngle = 0;
            //tmp.ptrLevel = level;
            //LevelCurrentAngles.Push(tmp);
            if(level != value)
            {
                //StartCoroutine(StopLevelRotate(level, followDuration));
             

                level = value;
                //if (level >= 0)
                //    LevelCurrentAngle = transform.GetChild(level).localEulerAngles.z;
            }


            
        }
    }

    public bool levelStop = false;
    //Effects
    public GameObject hitPrefab;
    public GameObject threePrefab;
    public GameObject cylinderPrefab;


    public Transform EffectHolder;
    public Transform spawn;
    public CharacterModelController character;


    //Input vars
    public float currentAngleSpeed = 0f;
    public Vector3 startPosition;
    public float maxRotateSpeed = 30f;
    public List<float> speedHistory;
    public float minSwipeDistX = 50f;
    public bool RotationProgress = false;
    public bool SpawnInProgress = false;
    public float followDuration;

    //Running parameters
    public float runSpeed = 360f;
    public float speedInertioa = 100f;
    public float stopInertia = 50f;
    public float jumpInertia = 10f;
    public float collisionInertia =0.3f;

    //[SerializeField]
    //private Stack<LevelAnglePtr> LevelCurrentAngles;
    ////current level ptr
    //LevelAnglePtr tempLevelAngle;

    [SerializeField]
    private float levelCurrentAngle;
    public float LevelCurrentAngle
    {
        get
        {
            return levelCurrentAngle;
        }

        set
        {
            
            levelCurrentAngle = value%360;
        }
    }
    [SerializeField]
    private float currentAngle;
    public float CurrentAngle
    {
        get
        {
            return currentAngle;
        }

        set
        {
            currentAngle = value%360;
        }
    }



    //RUNNER
    private void Start()
    {
       

        for (int i = 0; i < 10; i++)
        {
                GameObject tmpSpawn = Instantiate(levelPrefab, transform);
                tmpSpawn.transform.position += new Vector3(0, -spawnOffset, 0);
                spawnOffset += 20;
        }
        GameObject tmpBottomSpawn = Instantiate(bottomPrefab, transform);
        tmpBottomSpawn.transform.position += new Vector3(0, -spawnOffset, 0);
        
        speedHistory = new List<float>();
    }


    private float currentScroll;
    public float visualSpeedScalar = 1f;
    // Update is called once per frame
    void Update () {

        //
        if(Input.GetMouseButtonDown(0))
        {
            //currentScroll = 0;
            StopAllCoroutines();

            character.StartRunning();
        }


        if (Input.GetMouseButton(0) )
        {
            if(level>=0)
                LevelCurrentAngle = transform.GetChild(level).localEulerAngles.z;


            currentAngleSpeed = Mathf.Lerp(currentAngleSpeed, 0f, 5f * Time.deltaTime);
            CurrentAngle -= currentAngleSpeed * Time.deltaTime;
            transform.localRotation = Quaternion.Euler(new Vector3(90f, 0f, CurrentAngle));

            //// Scroll texture to fake it moving
            //currentScroll = currentScroll + Time.deltaTime * currentAngleSpeed * visualSpeedScalar;
            //transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Renderer>().material.mainTextureOffset = new Vector2(currentScroll, 0 );
            
        }
        

        if (Input.GetMouseButtonUp(0) && SwipeManager.Instance.Direction == SwipeDirection.None)
        {
            //Finish rotation to full stop
            StartCoroutine(StopRotate(currentAngleSpeed, stopInertia));
            character.StopRunning();
            
        }
        //Jump logic
        else if (Input.GetMouseButtonUp(0) && SwipeManager.Instance.Direction == SwipeDirection.Up)
        {
            Rigidbody rb = spawn.GetChild(0).GetChild(0).GetComponent<Rigidbody>();
            rb.AddForce(0, 16f, 0);
            StartCoroutine(StopRotate(currentAngleSpeed, jumpInertia));
            character.Jump();
            character.StopRunning();
        }
        //Slide logic
        else if (Input.GetMouseButtonUp(0) && SwipeManager.Instance.Direction == SwipeDirection.Down)
        {
            Rigidbody rb = spawn.GetChild(0).GetChild(0).GetComponent<Rigidbody>();
            rb.AddForce(0, -16f, 0);
            StartCoroutine(StopRotate(currentAngleSpeed, jumpInertia));
            character.Slide();
            character.StopRunning();
        }

        UpdateInput(); 
    }

    public void LevelRotate(int level, int direction)
    {
        if (direction == 1)
        {
            levelStop = true;
            //levelCurrentAngle -= 90f;
            //levelCurrentAngle = Mathf.Round(levelCurrentAngle / 90f) * 90f;
        }
        else if (direction == -1)
        {
            levelStop = true;
            //levelCurrentAngle += 90f;
            //levelCurrentAngle = Mathf.Round(levelCurrentAngle / 90f) * 90f;
        }

        //StartCoroutine(FollowRotate(level, levelCurrentAngle));

    }
    
    //for whole tower finish
    IEnumerator StopRotate(float speed, float inertia)
    {
        //currentScroll = 0;
        while (speed>0)
        {
            currentAngleSpeed = Mathf.Lerp(speed, 0f, 5f * Time.deltaTime);
            CurrentAngle -= speed * Time.deltaTime;
            transform.localRotation = Quaternion.Euler(new Vector3(90f, 0f, CurrentAngle));
            speed -= inertia;
            //Debug.Log(speed);

            //// Scroll texture to fake it moving
            //currentScroll = currentScroll + Time.deltaTime * currentAngleSpeed * visualSpeedScalar;
            //transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Renderer>().material.mainTextureOffset = new Vector2(currentScroll, 0);
            yield return null;
        }
    }
   
    
    //public void DropSpeed()
    //{
    //    runSpeed = 180;
        
    //}


    //Catch up behaviour when collided with obstaclew
    public IEnumerator StopCollision()
    {
        float originSpeed = runSpeed;
        runSpeed /= 2;
      
       //Bring spawn back to center
        while (runSpeed < originSpeed)
        {
            runSpeed += stopInertia * Time.deltaTime;

            yield return null;
        }

        runSpeed = originSpeed;
        ////Reset it if catch up "overshoots"
        //if (tmpChild.localPosition.z >= 0)
        //    tmpChild.localPosition = new Vector3(tmpChild.localPosition.x, tmpChild.localPosition.y, 0);
    }
    
    private void UpdateInput()
    {
    //
    Vector3 moveVector = new Vector3(Input.mousePosition.x, 0f, 0f) - new Vector3(startPosition.x, 0f, 0f);
    float moveX = Mathf.Clamp(moveVector.magnitude, 0f, this.maxRotateSpeed);
    float screenWidth = ((float)Screen.width);
    float moveXPercent = moveX / screenWidth;
    float speed = /*(Mathf.Sign(Input.mousePosition.x - startPosition.x) * moveXPercent) **/ runSpeed;
        currentAngleSpeed = speed;
    }

   

    public IEnumerator TiDi(float timeDelay)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(timeDelay);
        Time.timeScale = 1;
    }


}

   


