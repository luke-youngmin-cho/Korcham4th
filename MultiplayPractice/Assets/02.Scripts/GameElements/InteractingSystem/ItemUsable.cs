using Photon.Pun;
using MP.GameElements.StatSystem;
using System;

namespace MP.GameElements.InteractingSystem
{
    public abstract class ItemUsable : ItemThrowable, IUsable
    {
        public override Interactions interactions => base.interactions | Interactions.Use;
        public int durability
        {
            get => _durability;
            set
            {
                if (value > stats[StatType.Durability].valueModified)
                    return;

                _durability = value;
                onDurabilityChanged?.Invoke(value);

                if (value <= 0)
                {
                    DestroySelf();
                }
            }
        }
        private int _durability;

        public event Action<int> onDurabilityChanged;

        protected override void Awake()
        {
            base.Awake();

            Stat statDurability = new Stat(StatType.Durability, 100);
            stats.Add(StatType.Durability, statDurability);

            durability = stats[StatType.Durability].valueModified;
        }

        public abstract void Use();
    }
}
