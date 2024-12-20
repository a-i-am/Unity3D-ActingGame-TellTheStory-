using UnityEngine;
using UnityEditor;
using System.Collections;

namespace EightDirectionalSpriteSystem
{
    [ExecuteInEditMode]
    public class DemoActor : MonoBehaviour
    {
        public enum State { NONE, IDLE, WALKING };

        public ActorBillboard actorBillboard;

        public ActorAnimation idleAnim;
        public ActorAnimation walkAnim;

        private Transform myTransform;
        private ActorAnimation currentAnimation = null;
        private State currentState = State.NONE;

        void Awake()
        {
            myTransform = GetComponent<Transform>();
        }

        void Start()
        {
            SetCurrentState(State.IDLE);
        }

        private void OnEnable()
        {
            SetCurrentState(State.IDLE);
        }

        private void OnValidate()
        {
            if (actorBillboard != null && actorBillboard.CurrentAnimation == null)
                SetCurrentState(currentState);
        }

        void Update()
        {
            if (actorBillboard != null)
            {
                actorBillboard.SetActorForwardVector(myTransform.forward);
            }

            // Horizontal 또는 Vertical Input에 따라 상태 전환
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            bool isMoving = horizontalInput != 0 || verticalInput != 0;
            State newState = isMoving ? State.WALKING : State.IDLE;

            // 상태가 변경된 경우에만 SetCurrentState 호출
            if (currentState != newState)
            {
                SetCurrentState(newState);
            }
        }

        private void SetCurrentState(State newState)
        {
            // 상태가 변경된 경우에만 애니메이션을 초기화
            if (currentState != newState)
            {
                currentState = newState;

                switch (currentState)
                {
                    case State.WALKING:
                        currentAnimation = walkAnim;
                        break;

                    default:
                        currentAnimation = idleAnim;
                        break;
                }

                if (actorBillboard != null)
                {
                    // 상태 변경 시 애니메이션 초기화 및 재생
                    actorBillboard.StopAnimation();
                    actorBillboard.PlayAnimation(currentAnimation);
                }
            }
        }
    }
}
