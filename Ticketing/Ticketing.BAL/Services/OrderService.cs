using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.BAL.Contracts;
using Ticketing.DAL.Contracts;
using Ticketing.DAL.Domain;
using Ticketing.DAL.Domains;
using Ticketing.DAL.Repositories;
using static Ticketing.DAL.Enums.Statuses;

namespace Ticketing.BAL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _repositoryOrder;
        private readonly IRepository<ShoppingCart> _repositoryShoppingCart;
        private readonly IRepository<Seat> _repositorySeat;

        public OrderService(Repository<Order> repositoryOrder, Repository<ShoppingCart> repository, Repository<Seat> repositorySeat)
        {
            _repositoryOrder = repositoryOrder;
            _repositoryShoppingCart = repository;
            _repositorySeat = repositorySeat;
        }

        public async Task<bool> ReleaseCartsFromOrderAsync(int orderId)
        {
            var order = await _repositoryOrder.GetByIdAsync(orderId);
            if (order == null)
            {
                return false;
            }

            var allCarts = await _repositoryShoppingCart.GetAllAsync();
            var carts = allCarts.Where( c => c.CartId == order.CartId).ToList();

            if (!carts.Any())
            {
                return false;
            }

            var seats = await _repositorySeat.GetAllAsync();

            var selectedSeats = carts.Join(seats,
                cart => cart.SeatId,
                seat => seat.Id,
                (cart, seat) => new Seat{Id = seat.Id,
                    RowNumber = seat.RowNumber,
                    SeatNumber = seat.SeatNumber,
                    SeatStatusState = seat.SeatStatusState,
                    SectionId = seat.SectionId}).ToList();

            foreach (var seat in selectedSeats)
            {
                var currentSeat = await _repositorySeat.GetByIdAsync(seat.Id);
             
                if (currentSeat == null)
                {
                    return false;
                }

                currentSeat.SeatStatusState = SeatState.Available;
                await _repositorySeat.UpdateAsync(currentSeat);
            }

            foreach (var cart in carts)
            {
                await _repositoryShoppingCart.DeleteAsync(cart);
            }

            await _repositoryOrder.DeleteAsync(order);

            return true;
        }
    }
}
