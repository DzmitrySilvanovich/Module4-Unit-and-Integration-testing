using Ticketing.BAL.Contracts;
using Ticketing.BAL.Model;
using Ticketing.DAL.Contracts;
using Ticketing.DAL.Domain;
using Ticketing.DAL.Repositories;
using Mapster;

namespace Ticketing.BAL.Services
{
    public class VenueService : IVenueService
    {
        private readonly IRepository<Venue> _repository;
        private readonly IRepository<Section> _repositorySection;

        public VenueService(Repository<Venue> repository, Repository<Section> repositorySection)
        {
            _repository = repository;
            _repositorySection = repositorySection;
        }

        public async Task<IEnumerable<VenueReturnModel>> GetVenuesAsync()
        {
            var venues = await _repository.GetAllAsync();
            return venues.ProjectToType<VenueReturnModel>().ToList();
        }

        public async Task<IEnumerable<SectionReturnModel>> GetSectionsOfVenue(int venueId)
        {
            var sections = await _repositorySection.GetAllAsync();
            return sections.Where(s => s.VenueId == venueId).ProjectToType<SectionReturnModel>().ToList();
        }
    }
}
