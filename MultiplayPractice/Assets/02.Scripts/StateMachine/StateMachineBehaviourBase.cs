using MP.GameElements.Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MP.StateMachine
{
    public abstract class StateMachineBehaviourBase : StateMachineBehaviour
    {
        private readonly int HASH_IS_DIRTY = Animator.StringToHash("isDirty");
        private readonly int HASH_STATE = Animator.StringToHash("state");
        protected List<KeyValuePair<Func<bool>, Action<Animator>>> transitions = new List<KeyValuePair<Func<bool>, Action<Animator>>>();

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            animator.SetBool(HASH_IS_DIRTY, false);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            if (animator.IsInTransition(layerIndex) == false)
            {
                foreach (var transition in transitions)
                {
                    if (transition.Key.Invoke())
                    {
                        transition.Value.Invoke(animator);
                        break;
                    }    
                }
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            animator.SetInteger(HASH_STATE, (int)State.Move);
        }

        protected void ChangeState(Animator animator, State newState)
        {
            animator.SetInteger(HASH_STATE, (int)newState);
            animator.SetBool(HASH_IS_DIRTY, true);
        }
    }
}