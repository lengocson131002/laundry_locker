using LockerService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LockerService.Application.Common.Persistence;

public interface IApplicationDbContext
{
    DbSet<Account> Accounts { get; }
}