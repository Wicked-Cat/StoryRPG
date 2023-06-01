using Engine.Factories;
using Engine.Models;
using System.Xml.Linq;
using System;
using System.Diagnostics;

namespace Engine.Models
{
    public class Encounter
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public List<Monster> Monsters { get; set; }

        public Encounter(int id, string name) 
        {
            Name = name;
            ID = id;
            Monsters = new List<Monster>();
        }

        public void AddMonsterToList(int id)
        {
            Monster monster = MonsterFactory.GetMonster(id);
            bool AlreadyExists = Monsters.Any(m => m.ID == monster.ID);
            int i = 1;

            if (AlreadyExists)
            {
                foreach (Monster mon in Monsters)
                {
                    if (mon.ID == monster.ID)
                        i++;
                }
                monster.Name = $"{monster.Name} {i}";
            }

            Monsters.Add(monster);

        }

        public Encounter Clone()
        {
            Encounter encounter = new Encounter(ID, Name);

            foreach (Monster mon in Monsters)
            {
                encounter.Monsters.Add(mon.Clone());
            }

            return encounter;
        }
    }
}