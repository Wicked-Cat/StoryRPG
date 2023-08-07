using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Engine.Service;

namespace Engine.Models
{
    public class Battle
    {
        private readonly MessageBroker _messageBroker = MessageBroker.GetInstance();
        private Player _player;
        private Encounter _encounter;
        private Location _location;
        private List<LivingEntity> Combatants = new List<LivingEntity>();
        private static List<LivingEntity> TurnOrder = new List<LivingEntity>();

        public bool AreAllMonstersDead => _encounter.Monsters.All(m => m.IsDead == true);

        public Battle(Player player, Encounter encounter, Location location)
        {
            Combatants.Clear();

            _player = player;
            _encounter = encounter;
            _location = location;

            Combatants.Add(player);
            foreach(Monster monster in encounter.Monsters)
            {
                Combatants.Add(monster);
            }

            DetermineTurnOrder(Combatants);
            for(int i = 0; i < TurnOrder.Count; i++)
            {
                if (TurnOrder[i] is Player)
                {
                    return;
                }
                else
                {
                    AttackPlayer(TurnOrder[i] as Monster);
                }
            }
        }

        //initiative

        public void AttackOpponent(string aString)
        {
            if(_player.EquippedWeapon == null)
            {
                _messageBroker.RaiseMessage("martial arts are for dweebs");
            }

            Monster target = DetermineTarget(aString);
            if (target != null)
            {
                _messageBroker.RaiseMessage("");
                _player.Attack(target);
                _messageBroker.RaiseMessage("");
            }
            else
            {
                _messageBroker.RaiseMessage($"{aString} is not a valid target");
                return;
            }

            if (_encounter != null)
            {
                foreach (Monster monster in _encounter.Monsters)
                {
                    if (!monster.IsDead && !_player.IsDead)
                    {
                        monster.Attack(_player);
                    }
                }
            }
            if (_encounter != null)
            {
                if (AreAllMonstersDead)
                {
                    OnEncounterEnd();
                    _encounter = null;
                }
            }
        }

        private void OnEncounterEnd()
        {

        }

        private void AttackPlayer(Monster monster)
        {
            monster.Attack(_player);
        }
        private void OnCombatantActionPerformed(object sender, string result)
        {
            _messageBroker.RaiseMessage(result);
        }
        private static List<LivingEntity> DetermineTurnOrder(List<LivingEntity> combatants)
        {
            List<LivingEntity> tempCombatants = new List<LivingEntity>();

            foreach(LivingEntity entity in combatants)
            {
                tempCombatants.Add(entity);
            }
            for(int i = 0; i < tempCombatants.Count; i++)
            {
                int rand = RandomNumberGenerator.NumberBetween(1, tempCombatants.Count);
                TurnOrder.Add(tempCombatants[i]);
                tempCombatants.Remove(tempCombatants[i]);
            }

            return TurnOrder;
        }

        /*
         *    private void OnEncounterEnd()
        {
            foreach (Monster monster in CurrentEncounter.Monsters)
            {

                _messageBroker.RaiseMessage("");
                foreach (ItemQuantity item in monster.Inventory)
                {
                    _messageBroker.RaiseMessage($"You recieve {item.Quantity} {item.BaseItem.Name}.");
                    for (int i = 0; i < item.Quantity; i++)
                    {
                        CurrentPlayer.AddItemToInventory(item.BaseItem);
                    }
                }
            }
            CurrentLocation.EncountersHere.Remove(CurrentLocation.EncountersHere.First(e => e.Name.ToLower() == CurrentEncounter.Name.ToLower()));

        }*/

        private Monster DetermineTarget(string aString)
        {
            foreach (Monster monster in _encounter.Monsters)
            {
                if (aString == monster.Name.ToLower())
                {
                    return monster;
                }
            }
            return null;
        }
        public void Attack(string noun)
        {
            if (_encounter == null)
            {
                _messageBroker.RaiseMessage("There is nothing to attack");
                return;
            }
            if (_player.EquippedWeapon == null)
            {
                _messageBroker.RaiseMessage("You need a weapon equipped to attack");
                return;
            }

            Monster target = DetermineTarget(noun);
            if (target != null)
            {
                _messageBroker.RaiseMessage("");
                _player.Attack(target);
                _messageBroker.RaiseMessage("");
            }
            else
            {
                _messageBroker.RaiseMessage($"{noun} is not a valid target");
                return;
            }

            if (_encounter != null)
            {
                foreach (Monster monster in _encounter.Monsters)
                {
                    if (!monster.IsDead && !_player.IsDead)
                    {
                        monster.Attack(_player);
                    }
                }
            }
            if (_encounter != null)
            {
                if (AreAllMonstersDead)
                {
                    //OnEncounterEnd();
                    _encounter = null;
                }
            }

        }
        /*
        public void Flee()
        {
            _messageBroker.RaiseMessage($"You flee from the {_encounter.Name}");
            CurrentLocation.EncountersHere.Remove(CurrentLocation.EncountersHere.First(e => e.Name.ToLower() == CurrentEncounter.Name.ToLower()));
            CurrentEncounter = null;
        }

        private void OnEncounterEnd()
        {
            foreach (Monster monster in _encounter.Monsters)
            {

                _messageBroker.RaiseMessage("");
                foreach (ItemQuantity item in monster.Inventory)
                {
                    _messageBroker.RaiseMessage($"You recieve {item.Quantity} {item.BaseItem.Name}.");
                    for (int i = 0; i < item.Quantity; i++)
                    {
                        CurrentPlayer.AddItemToInventory(item.BaseItem);
                    }
                }
            }
            CurrentLocation.EncountersHere.Remove(CurrentLocation.EncountersHere.First(e => e.Name.ToLower() == CurrentEncounter.Name.ToLower()));

        }*/
    }
}
