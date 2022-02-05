using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
 

public class HeaderInfo : MonoBehaviourPun
{
    public TextMeshProUGUI nameText;
    public Image bar;
    private float maxValue;

    //initialize function when the parent spawns
    public void Initialized(string text, int maxVal)
    {
        nameText.text = text;
        maxValue = maxVal;
        bar.fillAmount = 1.0f;
    }

    //update health bar

    [PunRPC]
    void UpdateHealthBar(int value)
    {
        bar.fillAmount = (float)value / maxValue;
    }
}
