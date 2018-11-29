using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager> {

    //Level Generator vars
    public GameObject levelPrefab;
    public int spawnOffset = 0;


    //Input vars
    public float currentAngleSpeed = 0f;
    public Vector3 startPosition;
    public float maxRotateSpeed = 30f;
    public int rotateSpeed;
    public List<float> speedHistory;
    public float minSwipeDistX = 50f;
    public bool RotationProgress = false;
    public float followDuration;


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
                spawnOffset += 5;
        }

        speedHistory = new List<float>();
    }

    // Update is called once per frame
    void Update () {



        if (Input.GetMouseButtonUp(0))
        {
            //Finish rotation to even 90 degree slot
            StartCoroutine(FollowRotate(followDuration));
        }


        UpdateInput();
        currentAngleSpeed = Mathf.Lerp(currentAngleSpeed, 0f, 5f * Time.deltaTime);
        CurrentAngle += currentAngleSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(new Vector3(90f, 0f, CurrentAngle));


    }

    IEnumerator FollowRotate(float duration = 0.2f, float angle = 0)
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
                RotationProgress = false;
                
                break;
            }
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
    float speed = (Mathf.Sign(Input.mousePosition.x - startPosition.x) * moveXPercent) * rotateSpeed;

        if (true/*!GameManager.Instance.gameOver*/)
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
            //else if(Input.GetMouseButtonUp(0) && (moveX<minSwipeDistX))
            //{
            //    //To avoid interruptions
            //    RotationProgress = false;
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
        
            Debug.Log(child.transform.GetSiblingIndex() + "x FROM: " + from + " TO: " + to);
           
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

}

   


