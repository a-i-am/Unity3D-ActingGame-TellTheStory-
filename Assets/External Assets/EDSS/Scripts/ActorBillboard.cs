//=============================================================================
//  ActorBillboard
//  by Mariusz Skowroński from Healthbar Games (http://healthbargames.pl)
//  This class represents billboard - actor's visual part that is always
//  orientated to face current camera.
//=============================================================================

//=============================================================================
//  리팩토링 (Bit Flag  + WASD/Key Input 지원)
//=============================================================================

using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
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

        [System.Flags]
        private enum InputFlags
        {
            None = 0,
            Up = 1 << 0,
            Down = 1 << 1,
            Left = 1 << 2,
            Right = 1 << 3
        }

        public bool IsPlaying => isPlaying;
        public bool IsPaused => isPaused;
        public ActorAnimation CurrentAnimation => currentAnimation;


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

        private static readonly Direction[] directionTable = new Direction[16]
{
            Direction.Down,      // 0000
            Direction.Up,        // 0001
            Direction.Down,      // 0010
            Direction.Up,        // 0011
            Direction.Left,      // 0100
            Direction.UpLeft,    // 0101
            Direction.Left,      // 0110
            Direction.UpLeft,    // 0111
            Direction.Right,     // 1000
            Direction.UpRight,   // 1001
            Direction.DownRight, // 1010
            Direction.UpRight,   // 1011
            Direction.Right,     // 1100
            Direction.UpRight,   // 1101
            Direction.DownRight, // 1110
            Direction.UpRight    // 1111
};
        private static readonly ProfilerMarker k_UpdateSpriteMarker = new ProfilerMarker("UpdateSprite");
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
            if (isPlaying) isPaused = true;
        }

        public void ResumeAnimation()
        {
            if (isPlaying) isPaused = false;
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

            if (!isPlaying || isPaused)
                return;

            frameChangeDelay -= Time.deltaTime;
            if (frameChangeDelay > 0.0f)
                return;

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
            if (currentAnimation.AnimType == ActorAnimation.AnimDirType.EightDirections && !isDialogueActive)
            {
                InputFlags flags = InputFlags.None;

                if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) flags |= InputFlags.Up;
                if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) flags |= InputFlags.Down;
                if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) flags |= InputFlags.Left;
                if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) flags |= InputFlags.Right;

                animDir = directionTable[(int)flags];
            }

            animFrameDirection = (int)animDir;
            spriteRenderer.sprite = currentAnimation.GetSprite(currentFrameIndex, animFrameDirection);
        }

    }






}