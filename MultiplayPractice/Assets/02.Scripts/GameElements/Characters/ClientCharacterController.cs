using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MP.GameElements
{
    [RequireComponent(typeof(PhotonView), typeof(PhotonTransformView))]
    public class ClientCharacterController : MonoBehaviour
    {
        public Vector3 velocity { get; set; }
        private PhotonView _view;

        private void Awake()
        {
            _view = GetComponent<PhotonView>();
        }

        private void Update()
        {
            if (_view.IsMine)
            {
                velocity = new Vector3(Input.GetAxis("Vertical"), 0f, Input.GetAxis("Horizontal"));
            }
        }

        private void FixedUpdate()
        {
            if (_view.IsMine)
            {
                transform.position += velocity * Time.fixedDeltaTime;
            }
        }
    }
}