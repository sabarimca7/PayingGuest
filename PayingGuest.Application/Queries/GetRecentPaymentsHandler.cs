using MediatR;
using PayingGuest.Application.DTOs;
using PayingGuest.Application.Interfaces;
using PayingGuest.Domain.Interfaces;

public class GetRecentPaymentsHandler
    : IRequestHandler<GetRecentPaymentsQuery, List<RecentPaymentDto>>
{
    // ✅ 1. FIELD DECLARATION (THIS WAS MISSING)
    private readonly IRecentPaymentRepository _repository;

    // ✅ 2. CONSTRUCTOR INJECTION
    public GetRecentPaymentsHandler(IRecentPaymentRepository repository)
    {
        _repository = repository;
    }

    // ✅ 3. HANDLE METHOD
    public async Task<List<RecentPaymentDto>> Handle(
         GetRecentPaymentsQuery request,
         CancellationToken cancellationToken)
    {
        return await _repository.GetRecentPaymentsAsync(request.Take);
    }
}