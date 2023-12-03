using Mapster;
using Ticketing.BAL.Contracts;
using Ticketing.BAL.Model;
using Ticketing.DAL.Contracts;
using Ticketing.DAL.Domain;
using Ticketing.DAL.Domains;
using Ticketing.DAL.Repositories;

namespace Ticketing.BAL.Services
{
    public class EventService : IEventService
    {
        private readonly IRepository<Event> _repositoryEvent;
        private readonly IRepository<Seat> _repositorySeat;
        private readonly IRepository<SeatStatus> _repositorySeatStatus;
        private readonly IRepository<PriceType> _repositoryPriceType;
        private readonly IRepository<ShoppingCart> _repositoryShoppingCart;

        public EventService(Repository<Event> repositoryEvent,
                            Repository<Seat> repositorySeat,
                            Repository<SeatStatus> repositorySeatStatus,
                            Repository<PriceType> repositoryPriceType,
                            Repository<ShoppingCart> repositoryShoppingCart)
        {
            _repositoryEvent = repositoryEvent;
            _repositorySeat = repositorySeat;
            _repositorySeatStatus = repositorySeatStatus;
            _repositoryPriceType = repositoryPriceType;
            _repositoryShoppingCart = repositoryShoppingCart;
        }

        public async Task<IEnumerable<EventReturnModel>> GetEventsAsync()
        {
            var events = await _repositoryEvent.GetAllAsync();
            return events.ProjectToType<EventReturnModel>().ToList();
        }

        public async Task<List<SeatReturnModel>> GetSeatsAsync(int eventId, int sectionId)
        {
            var seatStatuses = await _repositorySeatStatus.GetAllAsync();
            var priceTypes = await _repositoryPriceType.GetAllAsync();

            var shoppingCarts = await _repositoryShoppingCart.GetAllAsync();
            var seats = await _repositorySeat.GetAllAsync();

            var result = (from seat in seats.Where(s => s.SectionId == sectionId)
                          join shoppingCart in shoppingCarts.Where(sh => sh.EventId == eventId)
                          on seat.Id equals shoppingCart.SeatId
                          join seatStatus in seatStatuses
                          on seat.SeatStatusState equals seatStatus.Id
                          join priceType in priceTypes
                          on shoppingCart.PriceTypeId equals priceType.Id
                          select new
                          {
                              SeatId = seat.Id,
                              seat.SectionId,
                              seat.RowNumber,
                              seat.SeatNumber,
                              seat.SeatStatusState,
                              NameSeatStatus = seatStatus.Name,
                              shoppingCart.PriceTypeId,
                              NamePriceType = priceType.Name
                          }).ProjectToType<SeatReturnModel>().ToList();

            return result;
        }
    }
}
