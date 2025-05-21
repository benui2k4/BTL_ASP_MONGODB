namespace ASP_MongoDB.Models
{
    public class CartViewModel
    {
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();

        public double GrandTotal => CartItems.Sum(item => item.Total);
    }
}
