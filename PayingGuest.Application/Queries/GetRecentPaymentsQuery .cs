using MediatR;
using PayingGuest.Application.DTOs;
using System.Collections.Generic;

public class GetRecentPaymentsQuery : IRequest<List<RecentPaymentDto>>
{

   
    public int Take { get; set; } = 5; // default latest 5
}
