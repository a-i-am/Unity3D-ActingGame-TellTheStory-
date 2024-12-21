//=============================================================================
//  ActorBillboard
//  by Mariusz Skowroński from Healthbar Games (http://healthbargames.pl)
//  This class represents billboard - actor's visual part that is always
//  orientated to face current camera.
//=============================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EightDirectionalSpriteSystem
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(SpriteRenderer))]
    public class ActorBillboard : MonoBehaviour
    {
        public Transform actorTransform;

        public delegate void BeforeRenderBillboardEvent();

        public BeforeRenderBillboardEvent beforeRenderBillboardEvent;

        public bool IsPlaying
        {
            get { return isPlaying; }
        }

        public bool IsPaused
        {
            get { return isPaused; }
        }

        public ActorAnimation CurrentAnimation
        {
            get { return currentAnimation; }
        }

        private Transform myTransform;
        private SpriteRenderer spriteRenderer;
        private Vector3 actorForwardVector = Vector3.forward;
        private ActorAnimation currentAnimation = null;
        private int currentFrameIndex = 0;
        private float frameChangeDelay = 1.0f;
        private bool isPlaying = false;
        private bool isPaused = false;
        private int playDirection = 1;

        public int animFrameDirection = 0;



        public void SetActorForwardVector(Vector3 actorForward)
        {
            actorForwardVector = actorForward;
        }

        public void PlayAnimation(ActorAnimation animation)
        {
            currentAnimation = animation;
            PlayAnimation();
        }

        public void PlayAnimation()
        {
            if (currentAnimation != null)
            {
                currentFrameIndex = 0;
                playDirection = 1;
                isPlaying = true;
                isPaused = false;
                frameChangeDelay = 1.0f / currentAnimation.Speed;
            }
        }

        public void PauseAnimation()
        {
            if (isPlaying)
            {
                isPaused = true;
            }
        }

        public void ResumeAnimation()
        {
            if (isPlaying)
            {
                isPaused = false;
            }
        }

        public void StopAnimation()
        {
            isPlaying = false;
            isPaused = false;
            currentFrameIndex = 0;
            playDirection = 1;
        }

        private void Awake()
        {
            myTransform = GetComponent<Transform>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer == null)
            {
                Debug.LogError("SpriteRenderer component is missing from ActorBillboard GameObject!");
            }
        }

        private void Update()
        {
            if (actorTransform != null)
            {
                actorForwardVector = actorTransform.forward;
            }

            if (isPlaying == false || isPaused == true)
                return;

            frameChangeDelay -= Time.deltaTime;
            if (frameChangeDelay > 0.0f)
                return;

            if (playDirection > 0)
            {
                currentFrameIndex++;
                if (currentFrameIndex >= currentAnimation.FrameCount)
                {
                    if (currentAnimation.Mode == ActorAnimation.AnimMode.Once)
                    {
                        currentFrameIndex = currentAnimation.FrameCount - 1;
                    }
                    else if (currentAnimation.Mode == ActorAnimation.AnimMode.PingPong)
                    {
                        currentFrameIndex = (currentAnimation.FrameCount << 1) - 2 - currentFrameIndex;
                        playDirection = -playDirection;
                    }
                    else
                    {
                        currentFrameIndex -= currentAnimation.FrameCount;
                    }
                }
            }
            else if (playDirection < 0)
            {
                currentFrameIndex--;
                if (currentFrameIndex < 0)
                {
                    if (currentAnimation.Mode == ActorAnimation.AnimMode.Once)
                    {
                        currentFrameIndex = 0;
                    }
                    else if (currentAnimation.Mode == ActorAnimation.AnimMode.PingPong)
                    {
                        currentFrameIndex = -currentFrameIndex;
                        playDirection = -playDirection;
                    }
                    else
                    {
                        currentFrameIndex += currentAnimation.FrameCount;
                    }
                }
            }

            frameChangeDelay += 1.0f / currentAnimation.Speed;
        }

        private void OnWillRenderObject()
        {
            if (currentAnimation == null)
            {
                Debug.LogWarning("No animation set for ActorBillboard.");
                return;
            }

            if (spriteRenderer == null)
            {
                Debug.LogWarning("SpriteRenderer is not assigned.");
                return;
            }

            if (beforeRenderBillboardEvent != null)
                beforeRenderBillboardEvent();

            // 처리된 방향키 입력을 추가하여 8방향 + 대각선 계산

            if (!DialogueManager.Instance.isDialogueActive)
                animFrameDirection = 0;

            if (currentAnimation.AnimType == ActorAnimation.AnimDirType.EightDirections)
            {
                // 입력된 방향에 따라 애니메이션 프레임을 결정
                if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftArrow))
                    animFrameDirection = 3; // Up-Left -> 3번 (대각선 방향)
                else if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow))
                    animFrameDirection = 5; // Up-Right -> 5번 (대각선 방향)
                else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow))
                    animFrameDirection = 1; // Down-Left -> 1번 (대각선 방향)
                else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow))
                    animFrameDirection = 7; // Down-Right -> 7번 (대각선 방향)
                else if (Input.GetKey(KeyCode.UpArrow))
                    animFrameDirection = 4; // Up -> 4번 (위 방향)
                else if (Input.GetKey(KeyCode.DownArrow))
                    animFrameDirection = 0; // Down -> 0번 (아래 방향)
                else if (Input.GetKey(KeyCode.LeftArrow))
                    animFrameDirection = 2; // Left -> 2번 (왼쪽 방향)
                else if (Input.GetKey(KeyCode.RightArrow))
                    animFrameDirection = 6; // Right -> 6번 (오른쪽 방향)
            }

            Sprite sprite = currentAnimation.GetSprite(currentFrameIndex, animFrameDirection);
            spriteRenderer.sprite = sprite;
        }

    }
}
