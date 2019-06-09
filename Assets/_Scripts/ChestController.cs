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
    private bool ChestOpened = false;

    // Start is called before the first frame update
    void Start()
    {

        if(PlayerPrefs.GetInt("KeyCount",0)>0)
        {
          
            if (PlayerPrefs.GetInt("KeyCount", 0) > 1)
            {
                keyMultiplier.text = string.Format("x{0}", PlayerPrefs.GetInt("KeyCount", 0).ToString());
            }

            //else
            //    keyMultiplier.gameObject.SetActive(false);
        }
        
    }


    private void OnMouseDown()
    {
        if(!ChestOpened && GameManager.Instance.KeyCount>0)
        {
            ChestOpened = true;

            GameManager.Instance.OpenChest(transform.GetChild(0).GetChild(0).GetComponent<Collectable>().PowerCol);

        }
        
    }



}
