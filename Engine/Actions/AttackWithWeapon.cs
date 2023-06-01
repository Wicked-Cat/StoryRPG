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
            if (itemInUse.Category != Item.ItemCategory.Weapon)
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
            int damage = _damage;
            string actorName = (actor is Player) ? "You" : $"The {actor.Name.ToLower()}";
            string targetName = (target is Player) ? "you" : $"the {target.Name.ToLower()}";
            if (damage == 0)
            {
                ReportResult($"{actorName} missed {targetName}.");
            }
            else
            {
                ReportResult($"{actorName} hit {targetName} with {_itemInUse.Name} for {damage} point{(damage > 1 ? "s" : "")}.");
                target.TakeDamage(damage);
            }
        }

    }
}
