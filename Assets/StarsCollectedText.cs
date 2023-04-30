using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StarsCollectedText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private void Start()
    {
        text.text = UIManager.GetTotalStarsCollectedKey() + " / " + UIManager._Instance.GetNumberOfLevels() + " Stars Collected";
    }
}
