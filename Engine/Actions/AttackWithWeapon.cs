using System;
using System.Security.Cryptography;
using Engine.Models;

namespace Engine.Actions
{
    public class AttackWithWeapon : BaseAction, IAction
    {
        private readonly int _damage;

        public AttackWithWeapon(
            Item itemInUse, 
            int damage ) 
            : base ( itemInUse)
        {
            int match = itemInUse.Tags.FindIndex(t => t.Name.ToLower() == "Weapon".ToLower()); //it returns -1 if not found
            if (match < 0)
            {
                throw new ArgumentException($"{itemInUse.Name} is not a weapon");
            }
            if (damage < 0)
            {
                throw new ArgumentException("Damage must be 0 or larger");
            }
            itemInUse = itemInUse;
            _damage = damage;

        }

        public void Execute(LivingEntity actor, LivingEntity target)
        {
            double strength = actor.Characteristics.FirstOrDefault(s => s.Name.ToLower() == "Strength".ToLower()).EffectiveLevel;
            double damage = (_damage + strength);
            string actorName = (actor is Player) ? "You" : $"The {actor.Name.ToLower()}";
            string targetName = (target is Player) ? "you" : $"the {target.Name.ToLower()}";
            BodyPart targetPart = target.CurrentBody.DetermineRandomTarget();

            if (damage == 0)
            {
                ReportResult($"{actorName} missed {targetName}.");
            }
            else
            {
                ReportResult($"{actorName} hit {targetName} with {_itemInUse.Name} in the {targetPart.Name} for {damage} point{(damage > 1 ? "s" : "")}.");
                target.TakeDamage(damage, targetPart);

                if (target.IsDead)
                {
                    ReportResult($"{target.Name} has died");
                }
            }
        }

    }
}
