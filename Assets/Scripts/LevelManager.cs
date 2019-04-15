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
        }
    }


    private void Start()
    {
        levelCount = PlayerPrefs.GetInt("LevelCount", 5);
        //LevelCurrentAngles = new Stack<LevelAnglePtr>();


        ////Initialize blank list for levelMover
        //blanksLevelMove = new List<Transform>();


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

    //Move Level direction - clockwise by default
    public void LevelMove(int levelIndex, bool direction = false)
    {
       if(!transform.GetChild(levelIndex).GetChild(0).CompareTag("Bottom"))
        {
            LevelMoveProgress = true;
            StartCoroutine(FollowRotate(levelIndex, transform.GetChild(levelIndex).localEulerAngles.z, direction));
        }


    }


    public float moveTime = 5f;
    //public IEnumerator LevelMover()
    //{



    //}
    public GameObject selectedCart;

    // Update is called once per frame
    void Update()
    {

        //
        if (Input.GetMouseButtonDown(0))
        {
            selectedCart = GrabRayObj("Cart");

            if (selectedCart != null)
            {
                //Debug.Log("RAY " + rayObj.name);
                if (selectedCart.CompareTag("Cart") || selectedCart.CompareTag("Steel"))
                {

                    LevelMoveTrigger = true;
                    
                    
                    //CurrentAngle = rayObj.transform.parent.parent.parent.parent.eulerAngles.y- transform.eulerAngles.y;
                    Level = selectedCart.transform.parent.parent.parent.parent.GetSiblingIndex();
                }
                
            }
            else
            {
                CurrentAngle = transform.eulerAngles.y;
                
                Debug.Log("TOWER RAY " +  CurrentAngle);
                Level = -2;
                
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
           
            //If cart was pressed
            if(LevelMoveTrigger)
            {
                //Move level left
                if(SwipeManager.Instance.IsSwiping(SwipeDirection.Left))
                {
                    LevelMove(Level, false);
                    LevelMoveTrigger = false;
                }
                //move level right
                else if(SwipeManager.Instance.IsSwiping(SwipeDirection.Right))
                {
                    LevelMove(Level, true);
                    LevelMoveTrigger = false;
                }
                //Drop pressed cart down
                else if(SwipeManager.Instance.IsSwiping(SwipeDirection.Down))
                {

                    SpawnManager.Instance.DropSpawn(selectedCart);
                    LevelMoveTrigger = false;

                    
                }
            }
            else if(DragInProgress)
            {
                initialMove = Vector3.zero;
                DragInProgress = false;
            }
            else
            {
                Debug.Log("LLLLL");
                //Finish rotation to even 90 degree slot
                //LevelMoveTrigger = false;
            }
            StartCoroutine(StopRotate(followDuration));
        }

             

      
        if(!LevelMoveTrigger)
        {
            UpdateInput();

            currentAngleSpeed = Mathf.Lerp(currentAngleSpeed, 0f, 5f * Time.deltaTime);
            CurrentAngle += currentAngleSpeed * Time.deltaTime;
            transform.localRotation = Quaternion.Euler(new Vector3(0, CurrentAngle, 0));
        }

        //if(Level != -2)
        //    transform.GetChild(Level).localRotation = Quaternion.Euler(new Vector3(0, CurrentAngle,0 ));
        //else
      
           


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

    //    StartCoroutine(FollowRotate(level, levelCurrentAngle));

    //}



    public float levelMoveSpeed = 120f;

    //Move level around  default - CLOCKWISE (LEFT)
    public IEnumerator FollowRotate(int level, float levelAngle, bool righDirection = false)
    {
        yield return new WaitForSeconds(0.2f);
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
                StartCoroutine(StopCircLerp(tmp.transform.parent, tmp.transform.parent.parent, 10f, true));
            }
            else
            {
                //change currents, set parents 
                tmp.Current++;
                tmp.transform.parent.SetParent(null);
                tmp.transform.parent.SetParent(transform.GetChild(level).GetChild(0).GetChild(tmp.Current));
                //Start turning sequence to the left
                StartCoroutine(StopCircLerp(tmp.transform.parent, tmp.transform.parent.parent, 10f));
            }
          
            

           
        }

        childsToMove.Clear();
       

        yield return new WaitForSeconds(0.8f);

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
            ////remember last rotation
            //if (level == -2)
            //    lastCurrentLevel = to;
        }


    }
    ////for current level finish
    //IEnumerator StopLevelRotate(float levelCur, int tempLevel = -2, float duration = 0.2f, float angle = 0)
    //{

    //    float tempLevelAngle = levelCur;
    //    float from = tempLevelAngle;
    //    float to = Mathf.Round(levelCur / 90f) * 90f;
    //    //Debug.Log("FROM: " + from + " TO: " + to);
    //    //Quaternion to = from * Quaternion.Euler(0f, 0, angle);

    //    //smooth lerp rotation loop
    //    float elapsed = 0.0f;
    //    while (elapsed < duration)
    //    {
    //        tempLevelAngle = Mathf.Lerp(from, to, elapsed / duration);

    //        elapsed += Time.fixedDeltaTime;
    //        if (Mathf.Abs(tempLevelAngle - to) <= 3f)
    //        {
    //            //currentAngleSpeed = 0;

    //            //Delay rotation bool to avoid extra spawn
    //            StartCoroutine(StopRotationProgress());
    //            break;
    //        }
    //        if (tempLevel >= 0)
    //        {
    //            transform.GetChild(tempLevel).localRotation = Quaternion.Euler(new Vector3(0, 0f, tempLevelAngle));
    //        }
    //        //Debug.Log(tempLevelAngle + " ::: " + to);

    //        yield return null;
    //    }

    //    if (Mathf.Abs(tempLevelAngle - to) <= 10f)
    //    {
    //        if (tempLevel >= 0)
    //        {
    //            transform.GetChild(tempLevel).localRotation = Quaternion.Euler(new Vector3(0, 0f, to));
    //        }
    //    }

    //    //LevelMoveProgress = false;
    //    levelStop = false;
    //}

    //Delay rotation bool
    private IEnumerator StopRotationProgress()
    {
        yield return new WaitForFixedUpdate();
        RotationProgress = false;
    }


    private void UpdateInput(GameObject target = null)
    {
        
        Vector3 moveVector = new Vector3(Input.mousePosition.x, 0f, 0f) - new Vector3(startPosition.x, 0f, 0f);
        float moveX = Mathf.Clamp(moveVector.magnitude, 0f, this.maxRotateSpeed);
        float screenWidth = ((float)Screen.width);
        float moveXPercent = moveX / screenWidth;
        //Debug.Log("% " + moveXPercent);
        float speed = 0;
        //Rotation resistance
        if (moveXPercent > minSwipeDistX)
        {

            speed = (Mathf.Sign(Input.mousePosition.x - startPosition.x) * moveXPercent) * rotateSpeed;
        }


      
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

                //Inertia speed decrease
                if (moveXPercent > 50/*moveXPrecent*/)
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
                CurrentAngle -= speed;
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
                    speedX -= speedHistory[i];
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


    public float rotSpeed = 1f;

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
    Vector3 initialMove = Vector3.zero;
    public bool DragInProgress = false;
    void OnMouseDrag()
    {
       
        if (!LevelMoveTrigger)
        {

            float distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

            if (initialMove == Vector3.zero)
            {
                initialMove = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));
                DragInProgress = true;
            }

            Vector3 pos_move = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));

            float pos_moveY = Mathf.Clamp((pos_move.y - initialMove.y) / 10f + transform.position.y, -10f /*+ spawnOffset * levelCount*/, 20f + spawnOffsetStep * levelCount);

            transform.position = new Vector3(transform.position.x, pos_moveY , transform.position.z);

        }

    }

    

}