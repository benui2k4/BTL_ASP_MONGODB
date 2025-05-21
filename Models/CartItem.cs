namespace ASP_MongoDB.Models
{
    public class CartItem
    {
        
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

        public double Total => Quantity * Price;

        public CartItem() { }

        public CartItem(Product product)
        {
            ProductId = product.ProductId;
            ProductName = product.ProductName;
            Image = product.Image;
            Price = product.Price ?? 0;
            Quantity = 1;
        }
    }
}

