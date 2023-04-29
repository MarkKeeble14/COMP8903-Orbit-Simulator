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
    public DialogueType Type;

    public enum DialogueType
    {
        RADIO_CHATTER
    }
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
    [SerializeField] private bool useDialogueDurations;
    [SerializeField] private float defaultDurationBetweenChars;

    [Header("Radio Chatter")]
    [SerializeField] private int maxLengthShortClip = 25;
    [SerializeField] private int maxLengthMidClip = 100;

    [Header("Audio")]
    [SerializeField] private RandomClipAudioClipContainer shortRadioChatters;
    [SerializeField] private RandomClipAudioClipContainer midRadioChatters;
    [SerializeField] private RandomClipAudioClipContainer longRadioChatters;

    private string playOnStartDialogueKey = "PlayOnStartDialogue";

    private bool dialogueInterruptedAction => Input.GetMouseButtonDown(0) && Time.deltaTime != 0;
    private bool dialogueInterruptedActionHard => Input.GetKeyDown(KeyCode.Backspace) && Time.deltaTime != 0;
    private Coroutine activeDialogue;

    [ContextMenu("SetPlayDialogue")]
    public void SetPlayDialogue()
    {
        PlayerPrefs.SetInt(playOnStartDialogueKey, 1);
        PlayerPrefs.Save();
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
            PlayerPrefs.Save();
        }
        else
        {
            if (PlayerPrefs.GetInt(playOnStartDialogueKey) == 1)
            {
                PlayDialogue(onStartDialogue);
                PlayerPrefs.SetInt(playOnStartDialogueKey, 0);
                PlayerPrefs.Save();
            }
        }
    }

    public void PlayDialogue(List<DialogueSnippet> snippets)
    {
        if (activeDialogue != null) StopCoroutine(activeDialogue);
        activeDialogue = StartCoroutine(ExecuteDialogue(snippets));
    }

    private void PlayRadioChatterDialogueNoise(int textLength)
    {
        if (textLength < maxLengthShortClip)
        {
            shortRadioChatters.PlayOneShot();
        }
        else if (textLength < maxLengthMidClip)
        {
            midRadioChatters.PlayOneShot();
        }
        else
        {
            longRadioChatters.PlayOneShot();
        }
    }

    // Needs to be called as a coroutine (which it is).
    // Will play the snippets in turn to their full length, and finish when they are all done.
    private IEnumerator ExecuteDialogue(List<DialogueSnippet> snippets)
    {
        // Debug.Log("Executing Dialogue");

        dialogueContainer.SetActive(true);
        foreach (DialogueSnippet ds in snippets)
        {
            switch (ds.Type)
            {
                case DialogueSnippet.DialogueType.RADIO_CHATTER:
                    PlayRadioChatterDialogueNoise(ds.Text.Length);
                    break;
            }

            float timeBetweenChars = defaultDurationBetweenChars;
            if (useDialogueDurations)
                timeBetweenChars = ds.Duration / ds.Text.Length;
            text.text = "<color=#" + ColorUtility.ToHtmlStringRGBA(ds.Speaker.Color) + ">" +
                ds.Speaker.Name + ": </color>";
            image.sprite = ds.Speaker.Icon;
            image.color = ds.Speaker.Color;
            bool interrupted = false;
            int index = 0;

            for (int i = 0; i < ds.Text.Length; i++)
            {
                char c = ds.Text[i];
                index++;
                text.text += c;

                if (dialogueInterruptedAction)
                {
                    interrupted = true;
                }

                if (dialogueInterruptedActionHard)
                {
                    ResetDialogueBox();
                    yield break;
                }

                if (interrupted)
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

                        if (dialogueInterruptedAction)
                        {
                            text.text = "<color=#" + ColorUtility.ToHtmlStringRGBA(ds.Speaker.Color) + ">" +
                                ds.Speaker.Name + ": </color>" + ds.Text;
                            i = ds.Text.Length;
                        }

                        yield return null;
                    }
                }
            }

            interrupted = false;

            yield return new WaitUntil(() => !dialogueInterruptedAction);

            if (useDialogueDurations)
            {
                // Wait the required time as per snippet description
                float afterSnippetTimer = 0;
                while (afterSnippetTimer < afterSnippetWaitTime)
                {
                    if (dialogueInterruptedAction)
                    {
                        afterSnippetTimer = afterSnippetWaitTime;
                    }

                    if (dialogueInterruptedActionHard)
                    {
                        ResetDialogueBox();
                        yield break;
                    }

                    afterSnippetTimer += Time.unscaledDeltaTime;
                    yield return null;
                }
            }
            else
            {
                while (!interrupted)
                {
                    if (dialogueInterruptedAction)
                    {
                        interrupted = true;
                    }

                    if (dialogueInterruptedActionHard)
                    {
                        ResetDialogueBox();
                        yield break;
                    }
                    yield return null;
                }
            }
        }

        // Clear subtitles after done talking.
        ResetDialogueBox();
    }

    private void ResetDialogueBox()
    {
        text.text = "";
        dialogueContainer.SetActive(false);
    }
}
