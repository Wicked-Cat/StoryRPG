namespace Engine.Models
{
    public class Background
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Background(string name, string description) 
        {
            Name = name;
            Description = description;
        }
    }
}
