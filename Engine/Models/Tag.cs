using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class Tag
    {
        public string Name { get; set; }
        public List<TagProperties> Properties { get; set; }
        public enum TagType { Metal, Wood, Hide, Meat, Bone, Grain, Unique, None};
        public TagType Type { get; set; }

        public Tag(string name, TagType type) 
        {
            Name = name;
            Type = type;
        }
    }
}
