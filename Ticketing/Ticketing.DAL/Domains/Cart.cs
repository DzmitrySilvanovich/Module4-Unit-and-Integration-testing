using Ticketing.DAL.Domains;

namespace Ticketing.DAL.Domain
{
    public class Cart
    {
        public Guid Id { get; set; }
        public List<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();
    }
}
