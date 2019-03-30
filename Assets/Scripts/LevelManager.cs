using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelManager : Singleton<LevelManager>
{

    //Level Generator vars
    public GameObject levelPrefab;
    public GameObject bottomPrefab;
    public GameObject blankCartPrefab;
    public float spawnOffset = 15f;
    public float spawnOffsetStep = 5f;

    [SerializeField]
    private int level = -2;
    public int Level
    {
        get
        {
            return level;
        }

        set
        {

            // if spawn moved down - rotate levels
            //if(level < value)

            level = value;
            //if (value == -2)
            //CurrentAngle = lastCurrentLevel;
            //else
            //    CurrentAngle = transform.GetChild(level).localEulerAngles.z;
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
    public bool LevelMoveProgress = false;
    public bool SpawnInProgress = false;
    public float followDuration;

    public int levelCount = 10;


    //[SerializeField]
    //private Stack<LevelAnglePtr> LevelCurrentAngles;
    ////current level ptr
    //LevelAnglePtr tempLevelAngle;

    public float lastCurrentLevel;
    public float lastLevelCurrentLevel;

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
            currentAngle = value % 360;
        }
    }


    private void Start()
    {
        //LevelCurrentAngles = new Stack<LevelAnglePtr>();


        for (int i = 0; i < levelCount; i++)
        {
            GameObject tmpSpawn = Instantiate(levelPrefab, transform);
            tmpSpawn.transform.position += new Vector3(0, -spawnOffset - spawnOffsetStep * i, 0);
           /* spawnOffset += spawnOffsetStep*/;
        }
        GameObject tmpBottomSpawn = Instantiate(bottomPrefab, transform);
        tmpBottomSpawn.transform.position += new Vector3(0, -spawnOffset - spawnOffsetStep * levelCount, 0);

        speedHistory = new List<float>();

        

    }


    public void LevelMove()
    {
        //Check if there's enough levels to move around
        if (transform.childCount >2)
        {
            LevelMoveProgress = true;
            Debug.Log("LEVELMOVE");
            //while (true)
            //{
            //yield return new WaitForSeconds(moveTime);
            List<int> rotLevels = new List<int>();
            int rotLevel;

            //Add first random level toa  list
            rotLevels.Add(Random.Range(0, transform.childCount - 1));
            //Get 5-1 different non-repeateable levels 
     
            for (int i = 0; i < Mathf.Round(transform.childCount / 3); i++)
            {
                //Repeat if number contains in the list
                do
                {
                    rotLevel = Random.Range(0, transform.childCount - 1);
                    Debug.Log(rotLevel + " : " + transform.childCount);

                }
                while (rotLevels.Contains(rotLevel));
                //if not - add it to the list
                rotLevels.Add(rotLevel);
            }


            //Debug.Log(rotLevels.Count);
            //Turn every Rot Level
            for (int i = 0; i < Mathf.Round(transform.childCount / 3); i++)
            {
                Debug.Log(rotLevels[i]);
                StartCoroutine(FollowRotate(rotLevels[i], transform.GetChild(rotLevels[i]).localEulerAngles.z));
            }

            rotLevels.Clear();
            //}
        }


    }


    public float moveTime = 5f;
    //public IEnumerator LevelMover()
    //{
       


    //}


    // Update is called once per frame
    void Update()
    {

        //
        if (Input.GetMouseButtonDown(0))
        {
            //GameObject rayObj = GrabRayObj("Cart");

            //if (rayObj != null)
            //{
            //    Level = rayObj.GetComponent<CartModelContoller>().CurrentLevel;
            //}
            //else
            //Level = -2;

        }

        if (Input.GetMouseButtonUp(0))
        {
            //Finish rotation to even 90 degree slot
            StartCoroutine(StopRotate(followDuration));
        }

        UpdateInput();

        currentAngleSpeed = Mathf.Lerp(currentAngleSpeed, 0f, 5f * Time.deltaTime);
        CurrentAngle += currentAngleSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(new Vector3(90f, 0f, CurrentAngle));



    }

    //public IEnumerator Mover(Transform target, float y)
    //{
    //    currentAngleSpeed = Mathf.Lerp(currentAngleSpeed, 0f, 5f * Time.deltaTime);
    //    CurrentAngle += currentAngleSpeed * Time.deltaTime;
    //    target.localRotation = Quaternion.Euler(new Vector3(y, 0f, CurrentAngle));

    //    float from = CurrentAngle;
    //    float to = Mathf.Round(CurrentAngle / 90f) * 90f;

    //    float elapsed = 0.0f;
    //    while (elapsed < duration)
    //    {
    //        CurrentAngle = Mathf.Lerp(from, to, elapsed / duration);
    //        elapsed += Time.fixedDeltaTime;
    //        if (Mathf.Abs(CurrentAngle - to) <= 3f)
    //        {
    //            currentAngleSpeed = 0;

    //            //Delay rotation bool to avoid extra spawn
    //            StartCoroutine(StopRotationProgress());
    //            break;
    //        }
    //        yield return null;
    //    }
    //}

    //public void LevelRotate(int level, int direction, float angle)
    //{
    //    if (direction == 1)
    //    {
    //        levelStop = true;
    //        //levelCurrentAngle -= 90f;
    //        levelCurrentAngle = Mathf.Round(levelCurrentAngle / 90f) * 90f;
    //    }
    //    else if (direction == -1)
    //    {
    //        levelStop = true;
    //        //levelCurrentAngle += 90f;
    //        levelCurrentAngle = Mathf.Round(levelCurrentAngle / 90f) * 90f;
    //    }

    //    //StartCoroutine(FollowRotate(level, levelCurrentAngle));

    //}
    public float levelMoveSpeed = 120f;

    public IEnumerator FollowRotate(int level, float levelAngle)
    {
        //Debug.Log("FOLLOWING");
        float tempAngle = levelAngle;
        int turnCount = Random.Range(0, 2);
        //float targetAngle = levelAngle + turnCount * 90f;


        for (int i = 0; i < turnCount; i++)
        {
            //Debug.Log("TURN " + i);
            foreach (Transform child in transform.GetChild(level).GetChild(0))
            {
                if (child.childCount != 0)
                {
                    CartModelContoller tmp = child.GetChild(0).GetComponent<CartModelContoller>();
                    CinemachineDollyCart tmpCart = child.GetComponent<CinemachineDollyCart>();
                    

                    if (turnCount == 0)
                    {

                        break;
                    }
                    else
                    {
                        tmp.Current++;
                        tmpCart.m_Path = tmp.paths[tmp.Current];
                        tmpCart.m_Position = 0;
                        tmpCart.m_Speed = 8;
                    }
                }

            }

            yield return new WaitForSeconds(0.8f);
        }

        LevelMoveProgress = false;
        yield return null;

        //while (tempAngle <= targetAngle)
        //{
        //    //Debug.Log(tempAngle + " + " + levelAngle);
        //    tempAngle += levelMoveSpeed * Time.deltaTime;
        //    transform.GetChild(level).localRotation = Quaternion.Euler(new Vector3(0f, 0f, tempAngle));
        //    yield return null;
        //}
        //StartCoroutine(StopLevelRotate(tempAngle, level));

    }

    //for whole tower finish
    IEnumerator StopRotate(float duration = 0.2f, float angle = 0)
    {



        float from = CurrentAngle;
        float to = Mathf.Round(CurrentAngle / 90f) * 90f;
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
        if (Mathf.Abs(CurrentAngle - to) <= 5f)
        {
            CurrentAngle = to;
            //remember last rotation
            if (level == -2)
                lastCurrentLevel = to;
        }


    }
    //for current level finish
    IEnumerator StopLevelRotate(float levelCur, int tempLevel = -2, float duration = 0.2f, float angle = 0)
    {

        float tempLevelAngle = levelCur;
        float from = tempLevelAngle;
        float to = Mathf.Round(levelCur / 90f) * 90f;
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
                //currentAngleSpeed = 0;

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

        //LevelMoveProgress = false;
        levelStop = false;
    }

    ////For finishing levels that are not current
    //IEnumerator StopLevelTempRotate(int tempLevel = -2, float duration = 0.2f, float angle = 0)
    //{

    //        float tempLevelAngle = LevelCurrentAngle;
    //        float from = tempLevelAngle;
    //        float to = Mathf.Round(LevelCurrentAngle / 90f) * 90f;
    //        //Debug.Log("FROM: " + from + " TO: " + to);
    //        //Quaternion to = from * Quaternion.Euler(0f, 0, angle);

    //        //smooth lerp rotation loop
    //        float elapsed = 0.0f;
    //        while (elapsed < duration)
    //        {
    //            tempLevelAngle = Mathf.Lerp(from, to, elapsed / duration);

    //            elapsed += Time.fixedDeltaTime;
    //            if (Mathf.Abs(tempLevelAngle - to) <= 3f)
    //            {
    //                currentAngleSpeed = 0;

    //                //Delay rotation bool to avoid extra spawn
    //                StartCoroutine(StopRotationProgress());
    //                break;
    //            }
    //            if (tempLevel >= 0)
    //            {
    //                transform.GetChild(tempLevel).localRotation = Quaternion.Euler(new Vector3(0, 0f, tempLevelAngle));
    //            }
    //            //Debug.Log(tempLevelAngle + " ::: " + to);

    //            yield return null;
    //        }

    //    if (Mathf.Abs(tempLevelAngle - to) <= 10f)
    //    {
    //        if (tempLevel >= 0)
    //        {
    //            transform.GetChild(tempLevel).localRotation = Quaternion.Euler(new Vector3(0, 0f, to));
    //        }
    //    }

    //    levelStop = false;
    //}

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

        if (true /*!SpawnInProgress*/ )
        {
            if (Input.GetMouseButtonDown(0))
            {

                speedHistory.Clear();
                currentAngleSpeed = 0f;
                startPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0) /*&& !LevelMoveProgress*/)
            {


                if (SwipeManager.Instance.Direction != SwipeDirection.None)
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
                //if(levelStop)
                //{
                //    LevelCurrentAngle -= speed;
                //}
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
            Vector3 to = toDesto + new Vector3(0,  - spawnOffset -spawnOffsetStep * child.GetSiblingIndex(), 0); ;


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


    //Get reference to object hit by ray with tag
    private GameObject GrabRayObj(string obj)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 500.0f))
        {
            if (hit.transform)
            {
                Debug.DrawRay(ray.origin, ray.direction * 500f, Color.red, 10f);
                //Debug.Log(hit.transform.name);
                if (hit.transform.gameObject.CompareTag(obj))
                {
                    return hit.transform.gameObject;
                }
                return hit.transform.gameObject;
            }
        }
        return null;




    }
}