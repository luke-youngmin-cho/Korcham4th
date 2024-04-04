using Photon.Pun;
using MP.GameElements.StatSystem;
using System;

namespace MP.GameElements.InteractingSystem
{
    public class Potion : ItemUsable
    {
        public override void Use()
        {
            if (ClientCharacterController.TryGetLocal(out ClientCharacterController controller))
            {
                controller.ChangeState(Characters.State.Drink);
            }
        }

        [PunRPC]
        protected override void GrabClientRpc(int clientID)
        {
            base.GrabClientRpc(clientID);
        }

        [PunRPC]
        protected override void UngrabClientRpc()
        {
            base.UngrabClientRpc();
        }
    }
}
