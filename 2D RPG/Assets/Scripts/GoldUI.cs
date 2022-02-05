using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GoldUI : MonoBehaviour
{

    public TextMeshProUGUI goldText;

    //instance
    public static GoldUI instance;

    private void Awake()
    {
        instance=this;
    }

    public void UpdateGoldText(int gold)
    {
        goldText.text = "Fold: " + gold;
    }

}
