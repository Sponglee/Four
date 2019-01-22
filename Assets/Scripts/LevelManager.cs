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
                if (level >= 0)
                    LevelCurrentAngle = transform.GetChild(level).localEulerAngles.z;
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

    //Input vars
    public float currentAngleSpeed = 0f;
    public Vector3 startPosition;
    public float maxRotateSpeed = 30f;
    public int rotateSpeed;
    public List<float> speedHistory;
    public float minSwipeDistX = 50f;
    public bool RotationProgress = false;
    public bool SpawnInProgress = false;
    public float followDuration;

    //Running parameters
    float stopInertia = 50f;
    float jumpInertia = 10f;

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

    // Update is called once per frame
    void Update () {

        //
        if(Input.GetMouseButtonDown(0))
        {
            StopAllCoroutines();
        }


        if (Input.GetMouseButton(0) )
        {
            if(level>=0)
                LevelCurrentAngle = transform.GetChild(level).localEulerAngles.z;


            currentAngleSpeed = Mathf.Lerp(currentAngleSpeed, 0f, 5f * Time.deltaTime);
            CurrentAngle -= currentAngleSpeed * Time.deltaTime;
            transform.localRotation = Quaternion.Euler(new Vector3(90f, 0f, CurrentAngle));
        }
        

        if (Input.GetMouseButtonUp(0) && SwipeManager.Instance.Direction == SwipeDirection.None)
        {
            //Finish rotation to full stop
            StartCoroutine(StopRotate(currentAngleSpeed, stopInertia));
        }
        else if (Input.GetMouseButtonUp(0) && SwipeManager.Instance.Direction == SwipeDirection.Up)
        {
            Rigidbody rb = spawn.GetChild(0).GetChild(0).GetComponent<Rigidbody>();
            rb.AddForce(0, 16f, 0);
            StartCoroutine(StopRotate(currentAngleSpeed, jumpInertia));

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

        while(speed>0)
        {
            currentAngleSpeed = Mathf.Lerp(speed, 0f, 5f * Time.deltaTime);
            CurrentAngle -= speed * Time.deltaTime;
            transform.localRotation = Quaternion.Euler(new Vector3(90f, 0f, CurrentAngle));
            speed -= inertia;
            Debug.Log(speed);
            yield return null;
        }
    }
   

    private void UpdateInput()
    {
    //
    Vector3 moveVector = new Vector3(Input.mousePosition.x, 0f, 0f) - new Vector3(startPosition.x, 0f, 0f);
    float moveX = Mathf.Clamp(moveVector.magnitude, 0f, this.maxRotateSpeed);
    float screenWidth = ((float)Screen.width);
    float moveXPercent = moveX / screenWidth;
    float speed = /*(Mathf.Sign(Input.mousePosition.x - startPosition.x) * moveXPercent) **/ rotateSpeed;
        currentAngleSpeed = speed;
    }

   

    public IEnumerator TiDi(float timeDelay)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(timeDelay);
        Time.timeScale = 1;
    }
}

   


