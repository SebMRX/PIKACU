using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public int maxNumberOfShots = 3;

    [SerializeField] private float secondsToWaitBeforeDeathCheck = 3f;
    [SerializeField] private GameObject restartScreenObject;
    [SerializeField] private SlingShotHandler SlingShotHandler;

    private int usedNumberOfShots;

    private IconHandler iconhandler;

    private List<Baddie> _baddies = new List<Baddie>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        iconhandler = GameObject.FindObjectOfType<IconHandler>();

        Baddie[] baddies = FindObjectsOfType<Baddie>();
        for (int i = 0; i < baddies.Length; i++)
        {
            _baddies.Add(baddies[i]);
        }
        
    
    }

    public void UseShot()
    {
        usedNumberOfShots++;
        iconhandler.UseShot(usedNumberOfShots);

        CheckForLastShot();
    }

    public bool HasEnoughShots()
    {
        if(usedNumberOfShots < maxNumberOfShots)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CheckForLastShot()
    {
        if(usedNumberOfShots == maxNumberOfShots)
        {
            StartCoroutine(CheckAfterWaitTime());
        }
    }

    private IEnumerator CheckAfterWaitTime()
    {
        yield return new WaitForSeconds(secondsToWaitBeforeDeathCheck);

        if(_baddies.Count == 0)
        {
            //win
            WinGame();
        }

        else
        {
            //lose
            RestartGame();
        }
    }

    public  void RemoveBaddie(Baddie baddie)
    {
        _baddies.Remove(baddie);
        CheckForAllDeadBaddies();
    }

    private void CheckForAllDeadBaddies()
    {
        if (_baddies.Count == 0)
        {
            //win
            WinGame();
        }
    }

    #region Win/Lose

    private void WinGame()
    {
        restartScreenObject.SetActive(true);
        SlingShotHandler.enabled = false;
    }

    public void RestartGame()
    {
        DOTween.Clear(true);
        SceneManager.LoadScene(0);
    }


    #endregion
}
