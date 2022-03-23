using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager instance;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    [SerializeField] TextMeshProUGUI creditsLabel;
    public void SetCreditsLabel(int _credits)
    {
        creditsLabel.text = _credits.ToString();
    }

}
