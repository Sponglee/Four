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
    public GameObject cartPrefab;
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
    public float minSwipeDistX = 0.02f;
    public bool RotationProgress = false;
    public bool LevelMoveProgress = false;

    public bool LevelMoveTrigger = false;

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
            transform.eulerAngles = new Vector3(0,currentAngle,0);
        }
    }


    public BallController ballRef;

    private void Start()
    {
        levelCount = PlayerPrefs.GetInt("LevelCount", 5);
        //LevelCurrentAngles = new Stack<LevelAnglePtr>();
       

        ////Initialize blank list for levelMover
        //blanksLevelMove = new List<Transform>();

        //StartCoroutine(LevelTimer());

        for (int i = 0; i < levelCount; i++)
        {
            GameObject tmpSpawn = Instantiate(levelPrefab, transform.position, transform.rotation, transform);
            tmpSpawn.transform.position += new Vector3(0, -spawnOffset - spawnOffsetStep * i, 0);
           /* spawnOffset += spawnOffsetStep*/;
        }
        GameObject tmpBottomSpawn = Instantiate(bottomPrefab, transform.position, transform.rotation, transform);
        tmpBottomSpawn.transform.position += new Vector3(0, -spawnOffset - spawnOffsetStep * levelCount, 0);

        speedHistory = new List<float>();

        

    }

    

    public GameObject selectedCart;

    // Update is called once per frame
    void Update()
    {

        //
        if (Input.GetMouseButtonDown(0))
        {
            LevelMoveTrigger = true;
            //Level = 


            //selectedCart = GrabRayObj("Cart");

            //if (selectedCart != null)
            //{
            //    //Debug.Log("RAY " + rayObj.name);
            //    if (selectedCart.CompareTag("Cart") || selectedCart.CompareTag("Steel"))
            //    {

            //        


            //        //CurrentAngle = rayObj.transform.parent.parent.parent.parent.eulerAngles.y- transform.eulerAngles.y;
            //        Level = selectedCart.transform.parent.parent.parent.parent.GetSiblingIndex();
            //    }

            //}
            //else
            //{
            //    CurrentAngle = transform.eulerAngles.y;

            //    Debug.Log("TOWER RAY " +  CurrentAngle);
            //    Level = -2;

            //}
        }

        if (Input.GetMouseButtonUp(0))
        {

            //If cart was pressed
            if (LevelMoveTrigger)
            {
                //Move level left
                if (SwipeManager.Instance.IsSwiping(SwipeDirection.Left))
                {

                    LevelMoveTrigger = false;
                    if (!LevelMoveTrigger)
                    {
                        //UpdateInput();


                        CurrentAngle = transform.eulerAngles.y;
                        //transform.localRotation = Quaternion.Euler(new Vector3(0, -CurrentAngle, 0));
                        StartCoroutine(StopRotate(followDuration,0,false));
                    }
                }
                //move level right
                else if (SwipeManager.Instance.IsSwiping(SwipeDirection.Right))
                {

                    LevelMoveTrigger = false;
                    if (!LevelMoveTrigger)
                    {
                        //UpdateInput();


                        CurrentAngle = transform.eulerAngles.y;
                        //transform.localRotation = Quaternion.Euler(new Vector3(0, -CurrentAngle, 0));
                        StartCoroutine(StopRotate(followDuration,0,true));
                    }
                }
                //Drop pressed cart down
                else if (SwipeManager.Instance.IsSwiping(SwipeDirection.Down))
                {

                    SpawnManager.Instance.DropSpawn(selectedCart);
                    transform.GetChild(Level).GetChild(0).GetComponent<CartManager>().CheckCarts();
                    LevelMoveTrigger = false;
                }




            }

            //if(Level != -2)
            //    transform.GetChild(Level).localRotation = Quaternion.Euler(new Vector3(0, CurrentAngle,0 ));
            //else


        }
  
    }
    int randomDir;

    public void StartLevelMove(int level)
    {
        for (int i = level - 1; i < level + 5; i++)
        {
            randomDir = Random.Range(0, 2);

            //if (i == level - 1)
            //{
            //    Debug.Log(SwipeManager.Instance.Direction);
            //    if (SwipeManager.Instance.IsSwiping(SwipeDirection.Right))
            //    {
            //        LevelMove(i, false);
            //    }
            //    else if (SwipeManager.Instance.IsSwiping(SwipeDirection.Left))
            //    {
            //        LevelMove(i, true);
            //    }
            //    else
            //    {
            //        continue;
            //    }
            //}
         /*   else*/ if (randomDir == 1)
            {
                LevelMove(i, false);
            }
            else
            {
                LevelMove(i, true);
            }

        }
    }


    public IEnumerator LevelTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            if(ballRef != null && ballRef.CurrentLevel >0)
                StartLevelMove(ballRef.CurrentLevel);
        }
       
    }
    //Move Level direction - clockwise by default
    public void LevelMove(int levelIndex, bool direction = false)
    {
        if (!transform.GetChild(levelIndex).GetChild(0).CompareTag("Bottom"))
        {
            GameManager.Instance.ComboActive = false;
            GameManager.Instance.Multiplier = 1;
            LevelMoveProgress = true;
            StartCoroutine(LevelMoveRotate(levelIndex, transform.GetChild(levelIndex).localEulerAngles.z, direction));
        }


    }


    public float moveTime = 5f;

    public float levelMoveSpeed = 60f;

    //Move level around  default - CLOCKWISE (LEFT)
    public IEnumerator LevelMoveRotate(int level, float levelAngle, bool righDirection = false)
    {
        yield return new WaitForSeconds(0.1f);
        //Debug.Log(">>FOLLOW ROTATE ");
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
                if(child.childCount != 0)
                    childsToMove.Add(child.GetChild(0));
                //Debug.Log(">" + child.parent.name);
            }

          
        }

        //Move them around  default - CLOCKWISE (LEFT)
        foreach (Transform childToMove in childsToMove)
        {
            CartModelContoller tmp = childToMove.GetChild(0).GetComponent<CartModelContoller>();

            //Switch parents of carts and move (right or left)
            if(righDirection)
            {
                //change currents, set parents 
                tmp.Current--;
                tmp.transform.parent.SetParent(null);
                tmp.transform.parent.SetParent(transform.GetChild(level).GetChild(0).GetChild(tmp.Current));
                //Start turning sequence to the right
                StartCoroutine(StopCircLerp(tmp.transform.parent, tmp.transform.parent.parent, levelMoveSpeed, true));
            }
            else
            {
                //change currents, set parents 
                tmp.Current++;
                tmp.transform.parent.SetParent(null);
                tmp.transform.parent.SetParent(transform.GetChild(level).GetChild(0).GetChild(tmp.Current));
                //Start turning sequence to the left
                StartCoroutine(StopCircLerp(tmp.transform.parent, tmp.transform.parent.parent, levelMoveSpeed));
            }
          
            

           
        }

        childsToMove.Clear();
       

        yield return new WaitForSeconds(0.4f);

        //*****************
        LevelMoveProgress = false;
        yield return null;

    }

    //Rotate a level to next position clockwise (right == false) or ccw (right == true)
    public IEnumerator StopCircLerp(Transform cart, Transform dest, float fFraction, bool right = false)
    {
        yield return new WaitForSeconds(0.1f);
        float debugTime = Time.time;
        float rotAngle=0; /*= dest.localRotation.y - cart.localRotation.y;*/



            //Debug.Log(cart.parent.name + " >> " + cart.localRotation.y*Mathf.Rad2Deg + " :::: " + dest.rotation.y*Mathf.Rad2Deg);


        if(cart.localRotation.y <0)
        {
            while (cart.localRotation.y <= 0)
            {
                //Debug.Log(tempAngle + " + " + levelAngle
                rotAngle += fFraction * Time.deltaTime;
                if(right)
                {
                    cart.localRotation = cart.localRotation * Quaternion.Euler(0f, -rotAngle, 0);
                }
                else
                {
                    cart.localRotation = cart.localRotation * Quaternion.Euler(0f, rotAngle, 0);
                }
                yield return null;
            }
            cart.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if(cart.localRotation.y>0)
        {
            while (cart.localRotation.y >= 0)
            {
                //Debug.Log(tempAngle + " + " + levelAngle
                rotAngle += fFraction * Time.deltaTime;

                if (right)
                {
                    cart.localRotation = cart.localRotation * Quaternion.Euler(0f, -rotAngle, 0);
                }
                else
                {
                    cart.localRotation = cart.localRotation * Quaternion.Euler(0f, rotAngle, 0);
                }
                yield return null;
            }
            cart.localRotation = Quaternion.Euler(0,0,0);
        }
        else
        {
            yield return null;
        }

        debugTime = Time.time - debugTime;
        //Debug.Log("<<<<<< " + debugTime);
    }


    //for whole tower finish
    IEnumerator StopRotate(float duration = 0.2f, float angle = 0, bool rightDirection = false)
    {


        float from = CurrentAngle;
        float to;

        if (rightDirection)
        {
            to = Mathf.Round((CurrentAngle + (360f / cartCount) )/ (360f / cartCount)) * (360f / cartCount);
        }
        else
        {
            to = Mathf.Round((CurrentAngle - (360f / cartCount)) / (360f / cartCount)) * (360f / cartCount);
        }

        //Debug.Log("STOP " + from + " :: " + to);

        Debug.Log("FROM: " + from + " TO: " + to);
        //Quaternion to = from * Quaternion.Euler(0f, 0, angle);

        //smooth lerp rotation loop
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            CurrentAngle = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.fixedDeltaTime;


            //Finish lerp 
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
            ////remember last rotation
            //if (level == -2)
            //    lastCurrentLevel = to;
        }


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

        //Refresh slider progress
        GameManager.Instance.LevelProgress = 1- (float)(transform.childCount-1)/levelCount;
     
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
   
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray, 500.0f);


       

        if (hits.Length >1)
        {
            RaycastHit hit = hits[1];
            if (hit.transform)
            {
                Debug.DrawRay(ray.origin, ray.direction * 500f, Color.red, 10f);
                //Debug.Log(hit.transform.name);
                if (hit.transform.gameObject.CompareTag(obj))
                {
                    return hit.transform.gameObject;
                }
                //return hit.transform.gameObject;
            }
        }
        return null;

    }


        //}

    //Get reference to object hit by ray with tag
    public List<GameObject> ScanCarts(Transform origin, string obj)
    {
        //Debug.Log("TRUE GRAB SPAWN");
        List<GameObject> grabObjs = new List<GameObject>();


        //Set all ray points 
        for (int i = 0; i < cartCount; i++)
        {
            float a = 360 / cartCount * i - CurrentAngle;
            Vector3 pos = RandomCircle(origin.position, 2.5f, a);
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
        Debug.DrawLine(dir, -Vector3.up * 100f + dir, Color.red, 3f);
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
    public Vector3 RandomCircle(Vector3 center, float radius, float a)
    {
        //Debug.Log(a);
        float ang = a;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.y = center.y;
        return pos;
    }


   

    ////Rotate planet
    //void OnMouseDrag()
    //{



    //    float rotY = Input.GetAxis("Mouse Y");

    //    if (Input.touchCount > 0)
    //    {
    //        rotY = Input.touches[0].deltaPosition.y;

    //    }

    //    Debug.Log("REEE " + rotY + " : " + Input.GetAxis("Mouse Y") + " = " + rotSpeed);

    //    //if (rotX > rotResistance)
    //    //{
    //    //    cameraHolder.transform.Rotate(Vector3.up, rotX, Space.Self);
    //    //}


    //    //Scroll camera and elevator
    //    if (Mathf.Abs(rotY) > 3)
    //        transform.position += new Vector3(0,  rotY * rotSpeed / 10000, 0);
    //    //elevatorHolder.transform.position += new Vector3(0, -rotX / 120f, 0);


    //}

    public float rotSpeed = 20;
    public float scrollSpeed = 2;
    public float rotResistance = 5000;

    ////Scroll towerf planet
    //void OnMouseDrag()
    //{
    //    if (!LevelMoveTrigger)
    //    {

    //        float rotX = Input.GetAxis("Mouse X") * rotSpeed * Mathf.Deg2Rad;
    //        float rotY = Input.GetAxis("Mouse Y") * scrollSpeed;
    //        if (Input.touchCount > 0)
    //        {
    //            rotX = Input.touches[0].deltaPosition.x;
    //            rotY = Input.touches[0].deltaPosition.y;
    //        }

    //        //Debug.Log("REEE " + rotX + " : " + Input.GetAxis("Mouse Y") + " = " + scrollSpeed);

    //        //if (rotX > rotResistance)
    //        //{
    //        //    transform.Rotate(Vector3.up, rotX, Space.Self);
    //        //}

    //        //Scroll camera and elevator
    //        transform.position += new Vector3(0, rotY / 10f, 0);
    //        //transform.position += new Vector3(0, -rotY / 120f, 0);
    //        Debug.Log(rotY);
    //    }
    //}


}