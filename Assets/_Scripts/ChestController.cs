using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class ChestController : MonoBehaviour
{
    public GameObject key;
    public TextMeshProUGUI keyMultiplier;
    public Animator chestAnim;

    [SerializeField]
    private bool chestOpenedBool = false;
    public bool ChestOpenedBool
    {
        get
        {
            return chestOpenedBool;
        }

        set
        {
            chestOpenedBool = value;
        }
    }

  

    [SerializeField]
    private bool canSkip = false;
    public bool CanSkip
    {
        get
        {
            return canSkip;
        }

        set
        {
            canSkip = value;
        }
    }


    [SerializeField]
    private bool skipIgnore = true;
    public bool SkipIgnore
    {
        get
        {
            return skipIgnore;
        }

        set
        {
            skipIgnore = value;
        }
    }




    public GameObject chestPowerUpPref;





    // Start is called before the first frame update
    void Start()
    {

        //if(PlayerPrefs.GetInt("KeyCount",0)>0)
        //{

        //    if (PlayerPrefs.GetInt("KeyCount", 0) > 1)
        //    {
        //        keyMultiplier.text = string.Format("x{0}", PlayerPrefs.GetInt("KeyCount", 0).ToString());
        //    }

        //    //else
        //    //    keyMultiplier.gameObject.SetActive(false);
        //}
        CheckKeys();
    }

    public void CheckKeys()
    {
        if (PlayerPrefs.GetInt("KeyCount", 0) > 0)
        {
            Debug.Log("ONENABLE" + PlayerPrefs.GetInt("KeyCount", 0));
            //if (PlayerPrefs.GetInt("KeyCount", 0) > 1)
            //{
                keyMultiplier.gameObject.SetActive(true);
                keyMultiplier.text = string.Format("x{0}", PlayerPrefs.GetInt("KeyCount", 0).ToString());
            //}
            //else
            //{
            //    keyMultiplier.gameObject.SetActive(false);
            //}


        }
        else
        {
            Debug.Log("NO KEY");
            key.SetActive(false);

        }
    }

  


    private void OnMouseDown()
    {
        if (!ChestOpenedBool && !CanSkip && GameManager.Instance.KeyCount>0)
        {
            ChestOpenedBool = true;
            //ChestOpened = true;
            OpenChest();

        }
        //else if (ChestOpenedBool && GameManager.Instance.KeyCount > 0)
        //{
        //    GameManager.Instance.ResetAnims();
        //    ////chestAnim.SetBool("Open", false);

        //    chestAnim.SetTrigger("SkipChest");
        //    ChestOpenedBool = false;
        //    //CanSkip = false;
        //    //chestAnim.SetTrigger("OpenChest");

        //}

    }


    //Open chest
    private void OpenChest()
    {
        

        //tmpChest.ChestOpened = true;
        StartCoroutine(StopOpenChest());
    }

   
    public IEnumerator StopOpenChest()
    {
        GameManager.Instance.KeyCount--;
        GameObject tmpGlow = Instantiate(chestPowerUpPref, transform.GetChild(0));
        int pow = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Collectable>().PowerCol;
        chestAnim.SetTrigger("ChestOpen");
        chestAnim.Play("PwrUp");

        transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Collectable>().RandomizeCollectable();
        GameManager.Instance.GrabCollectable(pow);

        yield return null;
        //Debug.Log(PlayerPrefs.GetInt("KeyCount", 0));

        yield return new WaitForSeconds(3f);
        Destroy(gameObject);    

    }
}
