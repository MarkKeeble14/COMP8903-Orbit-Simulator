using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlanetNameDisplay : MonoBehaviour
{
    [SerializeField] private string prefix;
    [SerializeField] private string suffix;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private string StringContent => prefix + GameManager._Instance.CurrentPlanet + suffix;

    // Update is called once per frame
    void Update()
    {
        Set();
    }

    public void Set()
    {
        text.text = StringContent;
    }
}
