using System;
using Engine.Models;
namespace Engine.Actions
{
    public class Heal : BaseAction, IAction
    {
        private readonly int _healthToHeal;

        public Heal(Item itemInUse, int healthToHeal) : base(itemInUse)
        {
            int match = itemInUse.Tags.FindIndex(t => t.Name.ToLower() == "Weapon".ToLower());
            if (match < 0)
                throw new ArgumentException($"{itemInUse.Name} is not consumable");
            

            _healthToHeal = healthToHeal;
        }

        public void Execute (LivingEntity actor, LivingEntity target)
        {
            string actorName = (actor is Player) ? "You" : $"The {actor.Name.ToLower()}.";
            string targetName = (actor is Player) ? "yourself" : "themself";

            _messageBroker.RaiseMessage($"{actorName} heal {targetName} for {_healthToHeal} point{(_healthToHeal > 1 ? "s" : "")}");
            target.Heal(_healthToHeal);
        }
    }
}
