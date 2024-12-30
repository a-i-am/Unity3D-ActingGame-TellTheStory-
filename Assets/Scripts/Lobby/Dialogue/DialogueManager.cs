using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.TextCore.Text;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public Image charachterIcon;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;
    public GameObject dialogueTemplate;
    public GameObject episodeWindow;

    private Queue<DialogueLine> lines;

    public bool isDialogueActive = false;
    public float typingSpeed = 0.2f;

    public Animator animator;

    private void Start()
    {
        if (Instance == null)
            Instance = this;

        lines = new Queue<DialogueLine>(); // 큐 초기화
        
        dialogueTemplate.SetActive(false);
        
    }

    public void StartDialogue(Dialogue dialogue)
    {
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

        StopAllCoroutines();

        switch (currentLine.line)
        {
            case "나도 무도회에 가고싶어...":
                SoundManager.instance.PlayNPC0_Dialogue(0);
                break;
            case "내 이야길 들어볼래?":
                SoundManager.instance.PlayNPC0_Dialogue(1);
                break;

        }

        StartCoroutine(TypeSentence(currentLine));
    }

    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        dialogueArea.text = "";
        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            dialogueArea.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        animator.Play("Hide");

        episodeWindow.SetActive(true);
    }
}
