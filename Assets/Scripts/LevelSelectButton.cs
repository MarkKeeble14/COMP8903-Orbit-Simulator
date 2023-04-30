using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] private PlanetLevel planetLevel;

    [SerializeField] private Button button;
    [SerializeField] private Image foreground;
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI starCollectedText;

    [SerializeField] private Color foregroundColor;
    [SerializeField] private Color backgroundColor;
    [SerializeField] private Color textColor;
    [SerializeField] private Color highlightedColor;
    [SerializeField] private Color pressedColor;

    [SerializeField] private Color starCollectedColor = Color.yellow;
    [SerializeField] private Color starUncollectedColor = Color.black;

    private void Awake()
    {
        if (PlayerPrefs.HasKey(planetLevel.ToString()))
        {
            // Debug.Log("Has Key: " + planetLevel.ToString() + ": " + PlayerPrefs.GetInt(planetLevel.ToString()));
            if (PlayerPrefs.GetInt(planetLevel.ToString()) != 1)
            {
                // Debug.Log("Not Completed: " + planetLevel.ToString());
                gameObject.SetActive(false);
                return;
            }
            else
            {
                // Debug.Log("Has Completed: " + planetLevel.ToString());
            }
        }
        else
        {
            // Debug.Log("No Key: " + planetLevel.ToString());
            gameObject.SetActive(false);
            return;
        }

        text.text = GameManager.ParsePlanetLevelToSceneString(planetLevel);
        text.color = textColor;

        foreground.color = foregroundColor;
        background.color = backgroundColor;

        ColorBlock colorBlock = button.colors;
        colorBlock.highlightedColor = highlightedColor;
        colorBlock.pressedColor = pressedColor;
        button.colors = colorBlock;

        starCollectedText.text = "Star " + (UIManager.GetHasPickedUpStarOnLevel(planetLevel) ? "Collected" : "Not Collected");
        starCollectedText.color = (UIManager.GetHasPickedUpStarOnLevel(planetLevel) ? starCollectedColor : starUncollectedColor);
    }

    public void OnClick()
    {
        UIManager._Instance.GoToLevel(planetLevel);
    }
}
