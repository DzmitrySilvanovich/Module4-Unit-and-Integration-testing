using Mapster;
using Microsoft.EntityFrameworkCore;
using Ticketing.BAL.Contracts;
using Ticketing.BAL.Model;
using Ticketing.DAL.Contracts;
using Ticketing.DAL.Domain;
using Ticketing.DAL.Domains;
using Ticketing.DAL.Repositories;
using static Ticketing.DAL.Enums.Statuses;

namespace Ticketing.BAL.Services
{
    public class CartService : ICartService
    {
        private readonly IRepository<ShoppingCart> _repositoryShoppingCart;
        private readonly IRepository<Seat> _repositorySeat;
        private readonly IRepository<Payment> _repositoryPayment;

        public CartService(Repository<ShoppingCart> repository, Repository<Seat> repositorySeat, Repository<Payment> repositoryPayment)
        {
            _repositoryShoppingCart = repository;
            _repositorySeat = repositorySeat;
            _repositoryPayment = repositoryPayment;
        }

        public async Task<CartStateReturnModel> AddSeatToCartAsync(Guid cartId, OrderCartModel orderCartModel)
        {
            var shoppingCarts = await _repositoryShoppingCart.GetAllAsync();

            var item = shoppingCarts.FirstOrDefault(c => c.CartId == cartId && c.EventId == orderCartModel.EventId && c.SeatId == orderCartModel.SeatId);

            var shoppingCartDto = new ShoppingCart
            {
                EventId = orderCartModel.EventId,
                SeatId = orderCartModel.SeatId,
                PriceTypeId = orderCartModel.PriceTypeId,
                Price = orderCartModel.Price,
                CartId = cartId,
            };

            await CreateOrUpdateAsync();

            var totalAmount = shoppingCarts.Sum(sc => sc.Price);

            return new CartStateReturnModel
            {
                CartId = cartId,
                TotalAmount = totalAmount
            };

            async Task CreateOrUpdateAsync()
            {
                if (item is null)
                {
                    await _repositoryShoppingCart.CreateAsync(shoppingCartDto);
                }
                else
                {
                    item.PriceTypeId = orderCartModel.PriceTypeId;
                    item.Price = orderCartModel.Price;
                    await _repositoryShoppingCart.UpdateAsync(shoppingCartDto);
                }
            }
        }

        public async Task<int> BookSeatToCartAsync(Guid cartId)
        {
            var shoppingCarts = await _repositoryShoppingCart.GetAllAsync();

            var shoppingCartItems = shoppingCarts.Where(c => c.CartId == cartId).ToList();

            if (!shoppingCartItems.Any())
            {
                return 0;
            }

            var shoppingCartSeats = shoppingCartItems.Select(sh => sh.SeatId).ToList();

            var allSeats = await _repositorySeat.GetAllAsync();

            var seats = allSeats.Where(s => shoppingCartSeats.Contains(s.Id));

            if (!seats.Any())
            {
                return 0;
            }

            decimal totalAmount = shoppingCartItems.Sum(i => i.Price);

            foreach (var seat in seats)
            {
                seat.SeatStatusState = SeatState.Booked;
                await _repositorySeat.UpdateAsync(seat);
            }

            var payment = new Payment
            {
                Amount = totalAmount,
                CartId = cartId,
                PaymentStatusId = PaymentState.NoPayment
            };

            var newPayment = await _repositoryPayment.CreateAsync(payment);

            return newPayment.Id;
        }

        public async Task<bool> DeleteSeatForCartAsync(Guid cartId, int eventId, int seatId)
        {
            var shoppingCarts = await _repositoryShoppingCart.GetAllAsync();

            var shoppingCartItems = await shoppingCarts.Where(c => c.CartId == cartId && c.EventId == eventId && c.SeatId == seatId).ToListAsync();

            if (!shoppingCartItems.Any())
            {
                return false;
            }

            foreach (var item in shoppingCartItems)
            {
                await _repositoryShoppingCart.DeleteAsync(item);
            }

            return true;
        }

        public async Task<IEnumerable<ShoppingCartReturnModel>> CartItemsAsync(Guid cartId)
        {
            var items = await _repositoryShoppingCart.GetAllAsync();
            return items.Where(i => i.CartId == cartId).ProjectToType<ShoppingCartReturnModel>().ToList();
        }
    }
}
