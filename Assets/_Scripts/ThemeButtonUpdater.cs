using UnityEngine;
using UnityEngine.UI;

public class ThemeButtonUpdater : MonoBehaviour
{

    public int itemCost;

    // Use this for initialization
    void Start()
    {
        UpdateThemeButton();
    }

    public void UpdateThemeButton(int lastSkin = -1)
    {
        if ((GameManager.Instance.SkinAvailability & 1 << transform.GetSiblingIndex()) == 1 << transform.GetSiblingIndex())
        {
            //gameObject.transform.GetChild(0).GetComponentInChildren<Text>().text = "SELECT";
            gameObject.transform.GetChild(0).GetComponent<Image>().color = Color.white;
            transform.GetChild(0).GetComponent<UnityEngine.UI.Outline>().effectColor = Color.white;

            if(PlayerPrefs.GetInt("Skin",0) == transform.GetSiblingIndex())
            {
                //gameObject.transform.GetChild(0).GetComponent<Image>().color = Color.black;
                transform.GetChild(0).GetComponent<UnityEngine.UI.Outline>().effectColor = Color.black;
                if(lastSkin != -1)
                    transform.parent.GetChild(lastSkin).GetChild(0).GetComponent<UnityEngine.UI.Outline>().effectColor = Color.white;
            }


        }
        else
        {
            transform.GetChild(0).GetComponent<Image>().color -= new Color(0f,0f,0f,0.5f);
            gameObject.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
            //gameObject.transform.GetChild(0).GetComponentInChildren<Text>().text = itemCost.ToString();
            //gameObject.transform.GetChild(0).GetComponentInChildren<Text>().color = Color.white;
            //gameObject.transform.GetChild(0).GetChild(1).GetComponentInChildren<Text>().color = Color.white;
            //gameObject.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);

        }

        //if (PlayerPrefs.GetInt("Theme", 0) == gameObject.transform.GetSiblingIndex())
        //{
        //    gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
        //}
        //else
        //{
        //    gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().color = new Color32(0, 0, 0, 100);

        //}
    }
}



