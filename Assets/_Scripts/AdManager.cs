
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : Singleton<AdManager>
{
    public static AdManager managerInstance;
    public int levelsToAd = 1;



    private void Start()
    {
        //Initialize ads
        Advertisement.Initialize("3467289");

        DontDestroyOnLoad(gameObject);

        //Check for doubles
        if (managerInstance == null)
        {
            managerInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public int adCount = 0;

    public void PlayAd(bool forceAd= false)
    {
        if (forceAd || (Advertisement.IsReady() && adCount >= levelsToAd))
        {

            Advertisement.Show();
            adCount = 0;
        }
        else
        {
            adCount++;
        }

    }


    private void HandleAdResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Failed:
                break;
            case ShowResult.Skipped:
                break;
            case ShowResult.Finished:
                break;
            default:
                break;
        }
    }
}

