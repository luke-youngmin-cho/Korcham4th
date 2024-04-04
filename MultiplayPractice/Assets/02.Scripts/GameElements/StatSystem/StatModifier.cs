namespace MP.GameElements.StatSystem
{
    public class StatModifier
    {
        public StatModifier(StatType statType, StatModType modType, int modValue)
        {
            this.statType = statType;
            this.modType = modType;
            this.modValue = modValue;
        }

        public StatType statType;
        public StatModType modType;
        public int modValue;
    }
}
