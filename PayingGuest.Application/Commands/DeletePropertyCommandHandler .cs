using MediatR;
using PayingGuest.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Commands
{
    public class DeletePropertyCommandHandler
     : IRequestHandler<DeletePropertyCommand, bool>
    {
        private readonly IPropertyRepository _repository;

        public DeletePropertyCommandHandler(IPropertyRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeletePropertyCommand request, CancellationToken cancellationToken)
        {
            var property = await _repository.GetByIdAsync(request.PropertyId);
            if (property == null)
                return false;

            await _repository.DeleteAsync(property);
            return true;
        }
    }
}
