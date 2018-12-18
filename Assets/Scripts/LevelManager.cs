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
                StartCoroutine(StopLevelRotate(level, followDuration));
             

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
        //LevelCurrentAngles = new Stack<LevelAnglePtr>();


        for (int i = 0; i < 10; i++)
        {
                GameObject tmpSpawn = Instantiate(levelPrefab, transform);
                tmpSpawn.transform.position += new Vector3(0, -spawnOffset, 0);
                spawnOffset += 5;
        }
        GameObject tmpBottomSpawn = Instantiate(bottomPrefab, transform);
        tmpBottomSpawn.transform.position += new Vector3(0, -spawnOffset, 0);
        
        speedHistory = new List<float>();
    }

    // Update is called once per frame
    void Update () {

        //
        if (Input.GetMouseButtonDown(0))
        {
            if(level>=0)
                LevelCurrentAngle = transform.GetChild(level).localEulerAngles.z;
        }

        if (Input.GetMouseButtonUp(0))
        {
            //Finish rotation to even 90 degree slot
            StartCoroutine(StopRotate(followDuration));
          
            StartCoroutine(StopLevelRotate(level,followDuration));
            //LevelCurrentAngle = transform.GetChild(level).eulerAngles.y;
        }
       

        UpdateInput();
        currentAngleSpeed = Mathf.Lerp(currentAngleSpeed, 0f, 5f * Time.deltaTime);
        CurrentAngle += currentAngleSpeed * Time.deltaTime;

        LevelCurrentAngle -= currentAngleSpeed * Time.deltaTime;
        if (/*Input.GetMouseButton(0) &&*/ levelStop)
        {
           
            if (level >= 0)
            {
                
                transform.GetChild(level).localRotation = Quaternion.Euler(new Vector3(0, 0f, LevelCurrentAngle));
            }
        }


        transform.localRotation = Quaternion.Euler(new Vector3(90f, 0f, CurrentAngle));

      


       
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

    //public IEnumerator FollowRotate(int level, float levelAngle)
    //{
    //    float tempAngle = 0; 
    //    while (tempAngle<=levelAngle)
    //    {
    //        Debug.Log(tempAngle + " + " + levelAngle);
    //        tempAngle += 160 * Time.deltaTime;
    //        transform.GetChild(level).localRotation = Quaternion.Euler(new Vector3(0f, 0f, tempAngle));
    //        yield return null;
    //    }
    //    //StartCoroutine(StopRotate(tempAngle));
       
    //}

    //for whole tower finish
    IEnumerator StopRotate(float duration = 0.2f, float angle = 0)
    {


        
        float from = CurrentAngle;
        float to = Mathf.Round(CurrentAngle / 90f)*90f;
        //Debug.Log("FROM: " + from + " TO: " + to);
        //Quaternion to = from * Quaternion.Euler(0f, 0, angle);

        //smooth lerp rotation loop
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            CurrentAngle = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.fixedDeltaTime;
            if (Mathf.Abs(CurrentAngle - to) <= 3f)
            {
                currentAngleSpeed = 0;

                //Delay rotation bool to avoid extra spawn
                StartCoroutine(StopRotationProgress());
                break;
            }
            yield return null;
        }
        //if(Mathf.Abs(CurrentAngle - to)<=0.3f)
        //{
        //    CurrentAngle = to;
        //    if (Level >= 0)
        //    {
                
        //        LevelCurrentAngle = transform.GetChild(level).eulerAngles.y;
        //    }
        //}
       

    }
    //for current level finish
    IEnumerator StopLevelRotate(int tempLevel = -2, float duration = 0.2f, float angle = 0)
    {

        float tempLevelAngle = LevelCurrentAngle;
        float from = tempLevelAngle;
        float to = Mathf.Round(LevelCurrentAngle / 90f) * 90f;
        //Debug.Log("FROM: " + from + " TO: " + to);
        //Quaternion to = from * Quaternion.Euler(0f, 0, angle);

        //smooth lerp rotation loop
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            tempLevelAngle = Mathf.Lerp(from, to, elapsed / duration);

            elapsed += Time.fixedDeltaTime;
            if (Mathf.Abs(tempLevelAngle - to) <= 3f)
            {
                currentAngleSpeed = 0;

                //Delay rotation bool to avoid extra spawn
                StartCoroutine(StopRotationProgress());
                break;
            }
            if (tempLevel >= 0)
            {
                transform.GetChild(tempLevel).localRotation = Quaternion.Euler(new Vector3(0, 0f, tempLevelAngle));
            }
            //Debug.Log(tempLevelAngle + " ::: " + to);

            yield return null;
        }

        //if (Mathf.Abs(tempLevelAngle - to) <= 10f)
        //{
        //    if (tempLevel >= 0)
        //    {
        //        transform.GetChild(tempLevel).localRotation = Quaternion.Euler(new Vector3(0, 0f, to));
        //    }
        //}

        levelStop = false;
    }

    //For finishing levels that are not current
    IEnumerator StopLevelTempRotate(int tempLevel = -2, float duration = 0.2f, float angle = 0)
    {
            
            float tempLevelAngle = LevelCurrentAngle;
            float from = tempLevelAngle;
            float to = Mathf.Round(LevelCurrentAngle / 90f) * 90f;
            //Debug.Log("FROM: " + from + " TO: " + to);
            //Quaternion to = from * Quaternion.Euler(0f, 0, angle);

            //smooth lerp rotation loop
            float elapsed = 0.0f;
            while (elapsed < duration)
            {
                tempLevelAngle = Mathf.Lerp(from, to, elapsed / duration);
               
                elapsed += Time.fixedDeltaTime;
                if (Mathf.Abs(tempLevelAngle - to) <= 3f)
                {
                    currentAngleSpeed = 0;

                    //Delay rotation bool to avoid extra spawn
                    StartCoroutine(StopRotationProgress());
                    break;
                }
                if (tempLevel >= 0)
                {
                    transform.GetChild(tempLevel).localRotation = Quaternion.Euler(new Vector3(0, 0f, tempLevelAngle));
                }
                //Debug.Log(tempLevelAngle + " ::: " + to);
                
                yield return null;
            }
      
        if (Mathf.Abs(tempLevelAngle - to) <= 10f)
        {
            if (tempLevel >= 0)
            {
                transform.GetChild(tempLevel).localRotation = Quaternion.Euler(new Vector3(0, 0f, to));
            }
        }
       
        levelStop = false;
    }

    //Delay rotation bool
    private IEnumerator StopRotationProgress()
    {
        yield return new WaitForFixedUpdate();
        RotationProgress = false;
    }


    private void UpdateInput()
    {
    //
    Vector3 moveVector = new Vector3(Input.mousePosition.x, 0f, 0f) - new Vector3(startPosition.x, 0f, 0f);
    float moveX = Mathf.Clamp(moveVector.magnitude, 0f, this.maxRotateSpeed);
    float screenWidth = ((float)Screen.width);
    float moveXPercent = moveX / screenWidth;
    float speed = (Mathf.Sign(Input.mousePosition.x - startPosition.x) * moveXPercent) * rotateSpeed;

        if (!SpawnInProgress)
        {
            if (Input.GetMouseButtonDown(0))
            {
                
                speedHistory.Clear();
                currentAngleSpeed = 0f;
                startPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
               

                if(SwipeManager.Instance.Direction != SwipeDirection.None)
                {
                    //To avoid interruptions
                    RotationProgress = true;
                }
                
               
                currentAngleSpeed = 0f;

                if (moveXPercent > minSwipeDistX)
                {
                    

                    speedHistory.Add(speed);
                }
                else
                {
                    speedHistory.Add(0f);
                }
                if (speedHistory.Count > 4)
                {
                    speedHistory.RemoveAt(0);
                }
                CurrentAngle += speed;
                if(levelStop)
                {
                    LevelCurrentAngle -= speed;
                }
                currentAngleSpeed = speed;
                startPosition = Input.mousePosition;
                if (currentAngleSpeed <= 0.02f)
                {
                    currentAngleSpeed = 0;
                    
                    //currentAngle = Mathf.Round(CurrentAngle / 90f) * 90f;
                }

            }
            else if (Input.GetMouseButtonUp(0) && (moveX > minSwipeDistX))
            {
                //
                float speedX = 0f;
                for (int i = 0; i < speedHistory.Count; i++)
                {
                    speedX += speedHistory[i];
                }
                currentAngleSpeed = 6f * speedX;
                startPosition = Input.mousePosition;
               
            }
            //else if (Input.GetMouseButtonUp(0) && (moveX < minSwipeDistX))
            //{
            //    if (SwipeManager.Instance.Direction == SwipeDirection.Down)
            //    {
            //        level++;
            //    }
            //    else if (SwipeManager.Instance.Direction == SwipeDirection.Up)
            //    {
            //        level--;
            //    }
            //}
        }
        //if(currentAngleSpeed == 0)
        //{
            
        //}
    }

    public float raiseDuration = 0.2f;

    public void RaiseTower()
    {
        
        foreach (Transform child in transform)
        {
           
            Vector3 to = transform.position;
            StartCoroutine(StopRaiseTower(child, to));
           
        }

    }



    public IEnumerator StopRaiseTower(Transform child, Vector3 toDesto, float duration = 0.2f)
    {

        yield return new WaitForEndOfFrame();
        if (child != null)
        {
            Vector3 from = child.position;
            Vector3 to = toDesto + new Vector3(0, -5 * child.GetSiblingIndex(), 0); ;
        

            //smooth lerp rotation loop
            float elapsed = 0.0f;
        
            //Debug.Log(child.transform.GetSiblingIndex() + "x FROM: " + from + " TO: " + to);
           
            while (elapsed < duration)
            {
                if (child != null)
                {
                    child.position = Vector3.Lerp(from, to, elapsed / duration);
                    elapsed += Time.fixedDeltaTime;
                    yield return null;

                }
                else break;

            }
        }
        else
            yield break;

       
    }


    public IEnumerator TiDi(float timeDelay)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(timeDelay);
        Time.timeScale = 1;
    }
}

   


