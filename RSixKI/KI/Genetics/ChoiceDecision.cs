using System;

namespace Genetics
{
    public class ChoiceDecision : Decision
    {
        private int choicesCount;
        private Random random;
        public ChoiceDecision(int choicesCount, Random random)
        {
            this.choicesCount = choicesCount;
            this.random = random;
            Init();
        }

        /// <summary>
        /// Selects a random choice-index (eg. PrimaryWeapon index, SecondaryWeapon index, ....)
        /// </summary>
        public override void Init()
        {
            Value = random.Next(choicesCount);
        }

        public override object Clone()
        {
            return new ChoiceDecision(choicesCount, random);
        }
    }
}
