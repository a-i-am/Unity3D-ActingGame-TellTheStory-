using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueCharacter
{
    public string name;
    public Sprite icon;
}

[System.Serializable]
public class DialogueLine
{
    public DialogueCharacter character;
    [TextArea(3, 10)]
    public string line;
}

[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public float interactRange = 2f;
    public GameObject headUI;

    private Camera mainCamera;

    private void Start()
    {

        mainCamera = Camera.main;
        HideHeadUI();
    }

    public void TriggerDialogue()
    {
        DialogueManager.instance.StartDialogue(dialogue);
    }

    void Update()
    {
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
        foreach (Collider collider in colliderArray)
        {
            if (!DialogueManager.instance.isDialogueActive && collider.CompareTag("Player"))
            {
                SetHeadUI();
                if (Input.GetKeyDown(KeyCode.Z))
                    TriggerDialogue();
            }
            else
            {
                HideHeadUI();
            }
        }
    }

    private void SetHeadUI()
    {
        if (headUI != null && mainCamera != null)
        {
            headUI.SetActive(true);

            Vector3 screenPosition = mainCamera.WorldToScreenPoint(transform.position);


            if (screenPosition.z > 0)
            {

                headUI.transform.position = screenPosition + new Vector3(0f,180f,0f);
                headUI.SetActive(true);
            }
            else
            {

                headUI.SetActive(false);
            }
        }
    }
    private void HideHeadUI()
    {
        if (headUI)
            headUI.SetActive(false);

    }

    private void OnDrawGizmos()
    {

        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawSphere(transform.position, interactRange);
    }
}
