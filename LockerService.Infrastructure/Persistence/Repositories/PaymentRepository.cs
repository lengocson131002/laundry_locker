using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain;
using LockerService.Infrastructure.Persistence.Contexts;

namespace LockerService.Infrastructure.Persistence.Repositories;

public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
{
    private readonly ApplicationDbContext _context;
    
    public PaymentRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        this._context = dbContext;
    }
}