using MP.GameElements.InteractingSystem;
using MP.UI;
using Photon.Pun;
using UnityEngine;

namespace MP.StateMachine
{
    public class DrinkBehaviour : StateMachineBehaviourBase
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animator.GetComponent<PhotonView>().AmOwner)
            {
                Interactable item = animator.GetComponent<Interactor>().interactable;

                if (item is ItemUsable)
                {
                    ((ItemUsable)item).durability -= 50;
                    UIManager.instance.Get<UIWarningWindow>().Show("Drunk potion");
                }
            }
        }
    }
}
