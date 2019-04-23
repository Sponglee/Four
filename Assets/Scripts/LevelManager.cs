using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelManager : Singleton<LevelManager>
{
    //Number of carts
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
            level = value; 
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
            //Rotate tower to Current angle
            currentAngle = value % 360;
            transform.eulerAngles = new Vector3(0,currentAngle,0);
        }
    }

    //Reference to ball Controller
    public BallController ballRef;

    private void Start()
    {
        levelCount = PlayerPrefs.GetInt("LevelCount", 5);
      
        //Start Destruction of levels
        //StartCoroutine(LevelTimer());

        //Generate the level
        for (int i = 0; i < levelCount; i++)
        {
            GameObject tmpSpawn = Instantiate(levelPrefab, transform.position, transform.rotation, transform);
            tmpSpawn.transform.position += new Vector3(0, -spawnOffset - spawnOffsetStep * i, 0);
        }
        //Add a bottom level
        GameObject tmpBottomSpawn = Instantiate(bottomPrefab, transform.position, transform.rotation, transform);
        tmpBottomSpawn.transform.position += new Vector3(0, -spawnOffset - spawnOffsetStep * levelCount, 0);

    }

    
    // Update is called once per frame
    void Update()
    {

       
        if (Input.GetMouseButtonDown(0))
        {
            LevelMoveTrigger = true;
        }
        else if (Input.GetMouseButtonUp(0))
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
                        CurrentAngle = transform.eulerAngles.y;
                        //transform.localRotation = Quaternion.Euler(new Vector3(0, -CurrentAngle, 0));
                        StartCoroutine(StopRotate(followDuration,0,false));
                    }
                }
                //move level to the right
                else if (SwipeManager.Instance.IsSwiping(SwipeDirection.Right))
                {

                    LevelMoveTrigger = false;
                    if (!LevelMoveTrigger)
                    {
                        CurrentAngle = transform.eulerAngles.y;
                        //transform.localRotation = Quaternion.Euler(new Vector3(0, -CurrentAngle, 0));
                        StartCoroutine(StopRotate(followDuration,0,true));
                    }
                }
            }

        }
  
    }

    //Direction generation int
    public int randomDir;
    //Keep track of last CurrentLevel
    public int lastLevel = -1;


    //Move 5 levels on stickcart
    public void StartLevelMove(int level)
    {
        if(level != lastLevel)
        {
            lastLevel = level;
            for (int i = level+1; i < level + 5; i++)
            {
                randomDir = Random.Range(0, 2);
                
                if (randomDir == 1)
                {
                    LevelMove(i, false);
                }
                else
                {
                    LevelMove(i, true);
                }

            }
        }
        
    }

    public float raiseTowerTime = 0.25f;
    //Level Destruction sequence
    public IEnumerator LevelTimer()
    {
        int i = 0;
        yield return new WaitForSeconds(4f);
       
        while (true)
        {
            yield return new WaitForSeconds(raiseTowerTime);
            if (true/*ballRef != null && ballRef.CurrentLevel > 0*/)
            {
                Instantiate(cylinderPrefab, transform.GetChild(i).position + new Vector3(0, -5, -5), Quaternion.identity, LevelManager.Instance.EffectHolder);
                transform.GetChild(i).gameObject.SetActive(false);
                //Destroy(transform.GetChild(0).gameObject);
                RaiseTower();
                if(raiseTowerTime >= 0.25f)
                {
                    //raiseTowerTime -= 0.05f;
                }
                i++;
            }
        }
       
    }



    //Move Level direction - clockwise by default
    public void LevelMove(int levelIndex, bool direction = false)
    {
        if (!transform.GetChild(levelIndex).GetChild(0).CompareTag("Bottom"))
        {
            GameManager.Instance.ComboActive = false;
            //GameManager.Instance.Multiplier = 1;
            LevelMoveProgress = true;
            StartCoroutine(LevelMoveRotate(levelIndex, transform.GetChild(levelIndex).localEulerAngles.z, direction));
        }


    }


    public float moveTime = 5f;

    public float levelMoveSpeed = 60f;

    //Move level around  default - CLOCKWISE (LEFT)
    public IEnumerator LevelMoveRotate(int level, float levelAngle, bool righDirection = false)
    {
        //yield return new WaitForSeconds(0.1f);
        
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
        //yield return new WaitForSeconds(0.1f);
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

    // For tower movement *NOT USED*
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
           
        }
     
    }


    public float raiseDuration = 0.2f;

    //Destroy a single level
    public void RaiseTower()
    {

        foreach (Transform child in transform)
        {

            Vector3 to = transform.position;
            StartCoroutine(StopRaiseTower(child, to));

        }

    }


    //Raise tower enum
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

    //For pizzaz
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


  
    
    public float rotSpeed = 20;
    public float scrollSpeed = 2;
    public float rotResistance = 5000;


}