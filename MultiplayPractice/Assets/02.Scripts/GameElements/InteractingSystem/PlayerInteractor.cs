using UnityEngine;

namespace MP.GameElements.InteractingSystem
{
    public class PlayerInteractor : Interactor, IOneHandGrabber
    {
        [field:SerializeField] public Transform hand { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            interactableMask = 1 << LayerMask.NameToLayer("Interactable") | 1 << LayerMask.NameToLayer("ItemTrigger");
        }
    }
}
