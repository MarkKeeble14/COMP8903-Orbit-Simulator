using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public struct Speaker
{
    public string Name;
    public Sprite Icon;
    public Color Color;
}

[System.Serializable]
public struct DialogueSnippet
{
    public string Text;
    public Speaker Speaker;
    public float Duration;
}

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float afterSnippetWaitTime = 1f;
    [SerializeField] private List<DialogueSnippet> onStartDialogue;
    [SerializeField] private GameObject dialogueContainer;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("OnStartDialoguePlayed"))
        {
            StartCoroutine(ExecuteDialogue(onStartDialogue));
            PlayerPrefs.SetInt("OnStartDialoguePlayed", 1);
        }
    }

    // Needs to be called as a coroutine (which it is).
    // Will play the snippets in turn to their full length, and finish when they are all done.
    private IEnumerator ExecuteDialogue(List<DialogueSnippet> snippets)
    {
        dialogueContainer.SetActive(true);
        foreach (DialogueSnippet ds in snippets)
        {
            float timeBetweenChars = ds.Duration / ds.Text.Length;
            text.text = "<color=#" + ColorUtility.ToHtmlStringRGBA(ds.Speaker.Color) + ">" +
                ds.Speaker.Name + ": </color>";
            image.sprite = ds.Speaker.Icon;
            image.color = ds.Speaker.Color;

            int index = 0;
            for (int i = 0; i < ds.Text.Length; i++)
            {
                char c = ds.Text[i];
                index++;
                text.text += c;
                yield return new WaitForSeconds(timeBetweenChars);

                if (Input.GetMouseButtonDown(0))
                {
                    text.text = "<color=#" + ColorUtility.ToHtmlStringRGBA(ds.Speaker.Color) + ">" +
                ds.Speaker.Name + ": </color>" + ds.Text;
                    break;
                }
            }

            //Wait the required time as per snippet description -
            //useful for cutting lines off or something like that.
            yield return new WaitForSeconds(afterSnippetWaitTime);
        }

        //Clear subtitles after done talking.
        text.text = "";
        dialogueContainer.SetActive(false);
    }
}
