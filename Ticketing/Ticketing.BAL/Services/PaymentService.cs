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
    public class PaymentService : IPaymentService
    {
        readonly IRepository<Payment> _repositoryPayment;
        readonly IRepository<PaymentStatus> _repositoryPaymentStatus;
        readonly IRepository<ShoppingCart> _repositoryShoppingCart;
        readonly IRepository<Seat> _repositorySeat;

        public PaymentService(Repository<Payment> repositoryPayment,
             Repository<PaymentStatus> repositoryPaymentStatus,
             Repository<ShoppingCart> repositoryShoppingCart,
             Repository<Seat> repositorySeat)
        {
            _repositoryPayment = repositoryPayment;
            _repositoryPaymentStatus = repositoryPaymentStatus;
            _repositoryShoppingCart = repositoryShoppingCart;
            _repositorySeat = repositorySeat;
        }

        public async Task<PaymentStatusReturnModel> GetPaymentStatusAsync(int paymentId)
        {
            var returnPaymentStatus = new PaymentStatusReturnModel { Id = 0, Name = string.Empty };

            var payment = await _repositoryPayment.GetByIdAsync(paymentId);

            if (payment is null)
            {
                return returnPaymentStatus;
            }

            var paymentStatus = await _repositoryPaymentStatus.GetByIdAsync(payment.PaymentStatusId);

            if (paymentStatus is null)
            {
                return returnPaymentStatus;
            }

            return paymentStatus.Adapt(returnPaymentStatus);
        }

        public async Task<bool> CompletePaymentAsync(int paymentId)
        {
            var payment = await _repositoryPayment.GetByIdAsync(paymentId);

            if (payment is null)
            {
                return false;
            }

            payment.PaymentStatusId = PaymentState.FullPayment;
            await _repositoryPayment.UpdateAsync(payment);

            var shoppingCarts = await _repositoryShoppingCart.GetAllAsync();

            var shoppingCartItems = shoppingCarts.Where(c => c.CartId == payment.CartId).ToList();

            var shoppingCartSeats = shoppingCartItems.Select(sh => sh.SeatId).ToList();

            var allSeats = await _repositorySeat.GetAllAsync();

            var seats = allSeats.Where(s => shoppingCartSeats.Contains(s.Id));

            if (!seats.Any())
            {
                return false;
            }

            foreach (var seat in seats)
            {
                seat.SeatStatusState = SeatState.Sold;
                await _repositorySeat.UpdateAsync(seat);
            }

            return true;
        }

        public async Task<bool> FailPaymentAsync(int paymentId)
        {
            var payment = await _repositoryPayment.GetByIdAsync(paymentId);

            if (payment is null)
            {
                return false;
            }

            payment.PaymentStatusId = PaymentState.PaymentFailed;
            await _repositoryPayment.UpdateAsync(payment);

            var shoppingCarts = await _repositoryShoppingCart.GetAllAsync();

            var shoppingCartItems = shoppingCarts.Where(c => c.CartId == payment.CartId).ToList();

            var shoppingCartSeats = shoppingCartItems.Select(sh => sh.SeatId).ToList();

            var allSeats = await _repositorySeat.GetAllAsync();

            var seats = allSeats.Where(s => shoppingCartSeats.Contains(s.Id));

            if (!seats.Any())
            {
                return false;
            }

            foreach (var seat in seats)
            {
                seat.SeatStatusState = SeatState.Available;
                await _repositorySeat.UpdateAsync(seat);
            }

            return true;
        }
    }
}
