﻿using Newtonsoft.Json;

namespace Engine.Models
{
    public class Ancestry
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public string Description { get; set; }
        [JsonIgnore]
        public List<Multiplier> Multipliers = new List<Multiplier>();
        [JsonIgnore]
        public List<Tag> Tags = new List<Tag>();

        public Ancestry(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public void AddMultiplierToList(string name, double num, Multiplier.Type type)
        {
            Multipliers.Add(new Multiplier(name, num, type));
        }
        public void AddTagToList(Tag tag)
        {
            Tags.Add(tag);
        }

        public Ancestry Clone()
        {
            Ancestry ancestry = new Ancestry(Id, Name, Description);
            foreach (Tag tag in Tags)
            {
                ancestry.Tags.Add(tag);
            }
            foreach (Multiplier multiplier in Multipliers)
            {
                ancestry.Multipliers.Add(multiplier);
            }

            return ancestry;
        }
    }
}

