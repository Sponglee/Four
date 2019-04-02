using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelManager : Singleton<LevelManager>
{

    public int cartCount=4;
    //Level Generator vars
    public GameObject levelPrefab;
    public GameObject bottomPrefab;
    public GameObject blankCartPrefab;
    public float spawnOffset = 35f;
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

    //public List<Transform> blanksLevelMove;

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


        ////Initialize blank list for levelMover
        //blanksLevelMove = new List<Transform>();


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


    public void LevelMove(int levelIndex)
    {
        ////Check if there's enough levels to move around
        //if (transform.childCount >2)
        //{
        //    LevelMoveProgress = true;
        //    Debug.Log("LEVELMOVE");
        //    //while (true)
        //    //{
        //    //yield return new WaitForSeconds(moveTime);
        //    List<int> rotLevels = new List<int>();
        //    int rotLevel;

        //    //Add first random level toa  list
        //    rotLevels.Add(Random.Range(0, transform.childCount - 1));
        //    //Get 5-1 different non-repeateable levels 

        //    for (int i = 0; i < Mathf.Round(transform.childCount / 3); i++)
        //    {
        //        //Repeat if number contains in the list
        //        do
        //        {
        //            rotLevel = Random.Range(0, transform.childCount - 1);
        //            Debug.Log(rotLevel + " : " + transform.childCount);

        //        }
        //        while (rotLevels.Contains(rotLevel));
        //        //if not - add it to the list
        //        rotLevels.Add(rotLevel);
        //    }


        //    //Debug.Log(rotLevels.Count);
        //    //Turn every Rot Level
        //    for (int i = 0; i < Mathf.Round(transform.childCount / 3); i++)
        //    {
        //        Debug.Log(rotLevels[i]);
        //        StartCoroutine(FollowRotate(rotLevels[i], transform.GetChild(rotLevels[i]).localEulerAngles.z));
        //    }

        //    rotLevels.Clear();
        //    //}
        //}
        //Debug.Log("LEVEL MOVE");
        StartCoroutine(FollowRotate(levelIndex, transform.GetChild(levelIndex).localEulerAngles.z));


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
        //number of turns
        int turnCount = Random.Range(1, 2);
        //float targetAngle = levelAngle + turnCount * 90f;

        //Remember every child of a level
        List<Transform> childsToMove = new List<Transform>();
        for (int i = 0; i < turnCount; i++)
        {
           
            foreach (Transform child in transform.GetChild(level).GetChild(0))
            {
                childsToMove.Add(child);
            }

          
        }

        //Move them around 
        foreach (Transform childToMove in childsToMove)
        {
            //Debug.Log("TURN " + childToMove.name);
            CartModelContoller tmp = childToMove.GetChild(0).GetComponent<CartModelContoller>();
            CinemachineDollyCart tmpCart = childToMove.GetComponent<CinemachineDollyCart>();

            tmp.Current++;
            tmpCart.m_Path = tmp.paths[tmp.Current];
            tmpCart.m_Position = 0;
            tmpCart.m_Speed = 8;

          

        }

        childsToMove.Clear();
        yield return new WaitForSeconds(0.8f);



        //*****************

        //Reset blank list
        //blanksLevelMove.Clear();


        //*****************
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
        float to = Mathf.Round(CurrentAngle / (360f/cartCount)) * (360f / cartCount);
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

    //Get reference to object hit by ray with tag
    public List<GameObject> ScanCarts(Transform origin, string obj)
    {
        //Debug.Log("TRUE GRAB SPAWN");
        List<GameObject> grabObjs = new List<GameObject>();

        ////Forward
        //Vector3 dir = origin.position + new Vector3(0, -3f, -3f);
        //GameObject tmp = GrabObjsRay(origin, dir, obj);
        //if (tmp != null)
        //    grabObjs.Add(tmp);


        ////Left
        //dir = origin.position + new Vector3(-3, -3f, 0f);
        //tmp = GrabObjsRay(origin, dir, obj);
        //if (tmp != null)
        //    grabObjs.Add(tmp);



        ////Back
        //dir = origin.position + new Vector3(0, -3f, 3f);
        //tmp = GrabObjsRay(origin, dir, obj);
        //if (tmp != null)
        //    grabObjs.Add(tmp);



        ////Right
        //dir = origin.position + new Vector3(3, -3f, 0f);
        //tmp = GrabObjsRay(origin, dir, obj);
        //if (tmp != null)
        //    grabObjs.Add(tmp);


        //Set all ray points 
        for (int i = 0; i < cartCount; i++)
        {
            int a = 360 / cartCount * i;
            Vector3 pos = RandomCircle(origin.position, 3.8f, a);
            GameObject tmp = GrabObjsRay(origin, pos, obj);
            if (tmp != null)
                grabObjs.Add(tmp);
        }






        return grabObjs;
    }

    //Grab cartCount colors for spawn
    public GameObject GrabObjsRay(Transform origin, Vector3 dir, string obj)
    {
       

        RaycastHit hit;
        Debug.DrawLine(dir, -Vector3.up * 100f + dir, Color.red, 10f);
        if (Physics.Raycast(dir, -Vector3.up, out hit))
        {
            if (hit.transform)
            {
                if (hit.transform.gameObject.CompareTag(obj))
                {
                    //Debug.Log("YEEET");
                    return hit.transform.gameObject;
                }
            }
        }
        return null;
    }


 

    //Build circle for spots
    public Vector3 RandomCircle(Vector3 center, float radius, int a)
    {
        //Debug.Log(a);
        float ang = a;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.y = center.y;
        return pos;
    }


}