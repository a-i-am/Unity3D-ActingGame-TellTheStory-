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
    public float interactRange = 2f; // 상호작용 범위
    public GameObject headUI;

    private Camera mainCamera;

    private void Start()
    {
        // 메인 카메라를 캐싱
        mainCamera = Camera.main;
        HideHeadUI();
    }

    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogue);
    }

    void Update()
    {
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
        foreach (Collider collider in colliderArray)
        {
            if (!DialogueManager.Instance.isDialogueActive && collider.CompareTag("Player"))
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
            // 3D 월드 좌표를 스크린 좌표로 변환
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(transform.position);

            // 변환된 스크린 좌표가 카메라의 앞쪽에 있는지 확인
            if (screenPosition.z > 0)
            {
                // 스크린 좌표를 headUI의 위치로 설정
                headUI.transform.position = screenPosition + new Vector3(0f,180f,0f);
                headUI.SetActive(true); // UI 활성화
            }
            else
            {
                // 카메라 뒤에 있으면 비활성화
                headUI.SetActive(false);
            }
        }
    }
    private void HideHeadUI()
    {
        headUI.SetActive(false);

    }

    private void OnDrawGizmos()
    {
        // 상호작용 범위를 시각적으로 표시
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawSphere(transform.position, interactRange);
    }
}