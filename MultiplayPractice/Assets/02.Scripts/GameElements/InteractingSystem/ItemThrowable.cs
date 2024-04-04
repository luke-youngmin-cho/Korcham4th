using UnityEngine;
using Photon.Pun;

namespace MP.GameElements.InteractingSystem
{
    public abstract class ItemThrowable : ItemGrabbable, IOneHandThrowable
    {
        public override Interactions interactions => base.interactions | Interactions.Throw;
        public int clientWhoThrownThis = NOBODY;
        public void Throw()
        {
            if (ClientCharacterController.TryGetLocal(out ClientCharacterController controller))
            {
                Ungrab();
                rigidbody.position = controller.transform.position + Vector3.up;
                rigidbody.AddForce(controller.transform.forward * 10.0f);
            }
        }
    }
}
