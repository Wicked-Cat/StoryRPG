namespace Engine.Models
{
    public class MonsterLoot
    {
        public int ID { get; set; }
        public int Percentage { get; set; } 
        public int Quantity { get; set; }

        public MonsterLoot(int id, int percentage, int quantity) 
        {
            ID = id;
            Percentage = percentage;
            Quantity = quantity;
        }

    }
}
