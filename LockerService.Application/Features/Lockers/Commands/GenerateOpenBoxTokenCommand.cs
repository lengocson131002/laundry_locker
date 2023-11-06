using LockerService.Application.Features.Tokens.Models;

namespace LockerService.Application.Features.Lockers.Commands;

public record GenerateOpenBoxTokenCommand(long LockerId) : IRequest<TokenResponse>;
