using System;
using System.Collections.Generic;

namespace MP.GameElements.StatSystem
{
    public class Stat
    {
        public Stat(StatType type, int value = 0)
        {
            this.type = type;
            this.value = value;
        }

        public StatType type { get; set; }
        public int value
        {
            get => _value;
            set
            {
                _value = value;
                onValueChanged?.Invoke(value);
                ModifiyValue();
            }
        }

        public int valueModified
        {
            get => _valueModified;
            set
            {
                _valueModified = value;
                onValueModifiedChanged?.Invoke(value);
            }
        }

        private int _value;
        private int _valueModified;
        private List<StatModifier> _modifiers = new List<StatModifier>();

        public event Action<int> onValueChanged;
        public event Action<int> onValueModifiedChanged;

        public void AddModifier(StatModifier modifier)
        {
            _modifiers.Add(modifier);
            ModifiyValue();
        }

        public void RemoveModifier(StatModifier modifier)
        {
            _modifiers.Remove(modifier);
            ModifiyValue();
        }

        private void ModifiyValue()
        {
            int sumAddFlat = 0;
            double sumAddPercent = 0.0;
            double sumMulPercent = 0.0;

            foreach (var modifier in _modifiers)
            {
                switch (modifier.modType)
                {
                    case StatModType.None:
                        break;
                    case StatModType.AddFlat:
                        {
                            sumAddFlat += modifier.modValue;
                        }
                        break;
                    case StatModType.AddPercent:
                        {
                            sumAddPercent += (modifier.modValue / 100.0);
                        }
                        break;
                    case StatModType.MulPercent:
                        {
                            sumMulPercent *= (modifier.modValue / 100.0f);
                        }
                        break;
                    default:
                        break;
                }
            }

            valueModified = (int)((_value + sumAddFlat) + (_value * sumAddFlat) + (_value * sumMulPercent));
        }
    }
}
