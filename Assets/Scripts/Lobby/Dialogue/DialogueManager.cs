using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.TextCore.Text;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [SerializeField] Image charachterIcon;
    [SerializeField] TextMeshProUGUI characterName;
    [SerializeField] TextMeshProUGUI dialogueArea;
    [SerializeField] GameObject dialogueTemplate;
    [SerializeField] GameObject episodeWindow;
    [SerializeField] Animator animator;

    private Queue<DialogueLine> lines;
    private Coroutine typingCoroutine; // 코루틴 핸들 추가
    private float typingSpeed = 0.2f;

    public bool isDialogueActive = false;
   
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        instance = null; // 씬 시작 전에 항상 초기화
    }
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (instance == null)
            instance = this;

        dialogueTemplate.SetActive(false);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        // 대화 시작 시 큐 초기화 및 크기 설정
        lines = new Queue<DialogueLine>(dialogue.dialogueLines);

        dialogueTemplate.SetActive(true);
        isDialogueActive = true;

        animator.Play("Show");

        lines.Clear();

        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }

        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }
        SoundManager.instance.PlayButton(0);

        DialogueLine currentLine = lines.Dequeue();

        charachterIcon.sprite = currentLine.character.icon;
        characterName.text = currentLine.character.name;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        switch (currentLine.line)
        {
            case "나도 무도회에 가고싶어...":
                SoundManager.instance.PlayNPC0_Dialogue(0);
                break;
            case "내 이야길 들어볼래?":
                SoundManager.instance.PlayNPC0_Dialogue(1);
                break;

        }

        typingCoroutine = StartCoroutine(TypeSentence(currentLine));
    }

    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        dialogueArea.text = "";
        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            dialogueArea.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        animator.Play("Hide");

        episodeWindow.SetActive(true);
    }
}
