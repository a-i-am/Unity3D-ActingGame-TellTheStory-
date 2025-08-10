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
        private enum Direction
        {
            Down = 0,
            DownLeft = 1,
            Left = 2,
            UpLeft = 3,
            Up = 4,
            UpRight = 5,
            Right = 6,
            DownRight = 7
        }

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

            bool isDialogueActive = DialogueManager.instance != null && DialogueManager.instance.isDialogueActive;

            beforeRenderBillboardEvent?.Invoke();

            Direction animDir = Direction.Down;

            // 8방향 처리 + 대화 중이 아닐 때만 입력 반영
            if (currentAnimation.AnimType == ActorAnimation.AnimDirType.EightDirections && !isDialogueActive)
            {
                bool up = Input.GetKey(KeyCode.UpArrow);
                bool down = Input.GetKey(KeyCode.DownArrow);
                bool left = Input.GetKey(KeyCode.LeftArrow);
                bool right = Input.GetKey(KeyCode.RightArrow);

                if (up && left) animDir = Direction.UpLeft;
                else if (up && right) animDir = Direction.UpRight;
                else if (down && left) animDir = Direction.DownLeft;
                else if (down && right) animDir = Direction.DownRight;
                else if (up) animDir = Direction.Up;
                else if (left) animDir = Direction.Left;
                else if (right) animDir = Direction.Right;
                // 기본값 Down은 그대로 유지
            }

            spriteRenderer.sprite = currentAnimation.GetSprite(currentFrameIndex, (int)animDir);
        }

    }
}
