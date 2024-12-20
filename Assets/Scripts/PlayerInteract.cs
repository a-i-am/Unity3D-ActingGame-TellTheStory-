using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour
{
    public float interactRange = 2f; // 상호작용 범위

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
            foreach (Collider collider in colliderArray)
            {
            }
        }
    }

    private void OnDrawGizmos()
    {
        // 상호작용 범위를 시각적으로 표시
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawSphere(transform.position, interactRange);
    }
}
