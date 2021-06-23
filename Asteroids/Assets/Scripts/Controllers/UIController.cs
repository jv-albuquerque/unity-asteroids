using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void UpdatePoints(int points)
    {
        text.text = points.ToString();
    }
}
