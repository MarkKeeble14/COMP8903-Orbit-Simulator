using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StateTypeDisplay : MonoBehaviour
{
    public enum StateType
    {
        ENABLED,
        DISABLED
    }

    [System.Serializable]
    public struct StateDisplayData
    {
        public string Text;
        public Color Color;
    }

    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private SerializableDictionary<StateType, StateDisplayData> stateDictionary = new SerializableDictionary<StateType, StateDisplayData>();

    private void Set(StateDisplayData data)
    {
        image.color = data.Color;
        text.text = data.Text;
    }

    public void On()
    {
        Set(stateDictionary[StateType.ENABLED]);
    }

    public void Off()
    {
        Set(stateDictionary[StateType.DISABLED]);
    }
}
