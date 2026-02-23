using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Commands
{
    public record DeletePropertyCommand(int PropertyId) : IRequest<bool>;
}
