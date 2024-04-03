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

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            animator.SetBool(HASH_IS_DIRTY, false);
        }
    }
}