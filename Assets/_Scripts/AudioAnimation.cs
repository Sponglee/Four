using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAnimation : MonoBehaviour
{
 



    public void CheckmarkSound()
    {
        AudioManager.Instance.PlaySound("MenuSmash");
    }
}
