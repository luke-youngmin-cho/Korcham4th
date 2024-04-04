using MP.GameElements.StatSystem;
using System.Collections.Generic;
using UnityEngine;

namespace MP.GameElements.InteractingSystem
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Item : Interactable
    {
        public Dictionary<StatType, Stat> stats = new Dictionary<StatType, Stat>();
        new protected Rigidbody rigidbody;

        protected override void Awake()
        {
            base.Awake();
            rigidbody = GetComponent<Rigidbody>();
        }
    }
}