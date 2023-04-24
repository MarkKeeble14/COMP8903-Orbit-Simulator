using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public struct DialogueSnippet
{
    public string Text;
    public float Duration;
    public Speaker Speaker;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager _Instance { get; private set; }

    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float afterSnippetWaitTime = 1f;
    [SerializeField] private List<DialogueSnippet> onStartDialogue;
    [SerializeField] private GameObject dialogueContainer;

    [SerializeField] private bool forcePlayOnStartDialogue;

    [Header("Audio")]
    [SerializeField] private RandomClipAudioClipContainer onPlayDialogueSnippet;

    private string playOnStartDialogueKey = "PlayOnStartDialogue";

    private bool dialogueInterrupted => Input.GetMouseButtonDown(0);
    private Coroutine activeDialogue;

    [ContextMenu("SetPlayDialogue")]
    public void SetPlayDialogue()
    {
        PlayerPrefs.SetInt(playOnStartDialogueKey, 1);
    }

    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(gameObject);
        }
        _Instance = this;
    }

    public void TryPlayOnStartDialogue()
    {
        if (!PlayerPrefs.HasKey(playOnStartDialogueKey) || forcePlayOnStartDialogue)
        {
            PlayDialogue(onStartDialogue);
            PlayerPrefs.SetInt(playOnStartDialogueKey, 0);
        }
        else
        {
            if (PlayerPrefs.GetInt(playOnStartDialogueKey) == 1)
            {
                PlayDialogue(onStartDialogue);
                PlayerPrefs.SetInt(playOnStartDialogueKey, 0);
            }
        }
    }

    public void PlayDialogue(List<DialogueSnippet> snippets)
    {
        if (activeDialogue != null) StopCoroutine(activeDialogue);
        activeDialogue = StartCoroutine(ExecuteDialogue(snippets));
    }

    // Needs to be called as a coroutine (which it is).
    // Will play the snippets in turn to their full length, and finish when they are all done.
    private IEnumerator ExecuteDialogue(List<DialogueSnippet> snippets)
    {
        Debug.Log("Executing Dialogue");

        dialogueContainer.SetActive(true);
        foreach (DialogueSnippet ds in snippets)
        {
            onPlayDialogueSnippet.PlayOneShot();

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

                if (dialogueInterrupted)
                {
                    text.text = "<color=#" + ColorUtility.ToHtmlStringRGBA(ds.Speaker.Color) + ">" +
                        ds.Speaker.Name + ": </color>" + ds.Text;
                    i = ds.Text.Length;
                }
                else
                {
                    float afterCharTimer = 0;
                    while (afterCharTimer < timeBetweenChars)
                    {
                        afterCharTimer += Time.unscaledDeltaTime;

                        if (dialogueInterrupted)
                        {
                            text.text = "<color=#" + ColorUtility.ToHtmlStringRGBA(ds.Speaker.Color) + ">" +
                                ds.Speaker.Name + ": </color>" + ds.Text;
                            i = ds.Text.Length;
                        }

                        yield return null;
                    }
                }
            }

            yield return new WaitUntil(() => !dialogueInterrupted);

            // Wait the required time as per snippet description
            float afterSnippetTimer = 0;
            while (afterSnippetTimer < afterSnippetWaitTime)
            {
                if (dialogueInterrupted)
                {
                    afterSnippetTimer = afterSnippetWaitTime;
                }

                afterSnippetTimer += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        //Clear subtitles after done talking.
        text.text = "";
        dialogueContainer.SetActive(false);
    }
}
