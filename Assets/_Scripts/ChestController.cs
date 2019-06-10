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
    private bool newChestReady = false;
    public bool NewChestReady
    {
        get
        {
            return newChestReady;
        }

        set
        {
            newChestReady = value;
        }
    }


    [SerializeField]
    private bool chestOpened = false;

    public bool ChestOpened
    {
        get
        {
            return chestOpened;
        }

        set
        {
            Debug.Log(">>>><<<<");

            if(value == true && chestOpened == false)
            {
                chestAnim.SetTrigger("NewChest");
            }
            else if(value == false && chestOpened == true)
            {
                chestAnim.SetTrigger("NewChest");
            }
            chestOpened = value;
        }
    }

    

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
        if (/*!chestAnim.GetBool("Open") */ !ChestOpened && GameManager.Instance.KeyCount>0)
        {

           
            GameManager.Instance.OpenChest(transform.GetChild(0).GetChild(0).GetComponent<Collectable>().PowerCol);

        }
        else if (/*chestAnim.GetBool("Open")*/ ChestOpened && GameManager.Instance.KeyCount > 0)
        {
            //chestAnim.SetBool("Open", false);
            ChestOpened = false;
            chestAnim.SetTrigger("SkipChest");

        }

    }



}
