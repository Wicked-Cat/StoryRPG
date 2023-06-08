namespace Engine.Models
{
    public class MerchantStock
    {
        public int ID { get; set; }
        public int Percentage { get; set; }
        public int Quantity { get; set; }

        public MerchantStock(int id,  int percentage, int quantity)
        {
            ID = id;
            Percentage = percentage;
            Quantity = quantity;
        }
    }
}
