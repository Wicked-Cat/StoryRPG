using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class Body
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<BodyPart> Parts { get; set; }
        public Body(int id, string name, string description) 
        {
            Id = id;
            Name = name;
            Description = description;
            Parts = new List<BodyPart>();
        }
        public BodyPart DetermineRandomTarget()
        {
            double maxCount = Parts.Count();
            int rand = RandomNumberGenerator.NumberBetween(1, Convert.ToInt32(maxCount));

            return Parts[rand - 1];
        }
        public Body Clone()
        {
            Body body = new Body(Id, Name, Description);
            
            foreach (BodyPart part in Parts)
            {
                body.Parts.Add(part);
            }

            return body;
        }
    }
}
