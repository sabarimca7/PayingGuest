using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Interfaces
{
    public interface IDateTimeService
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
    }
}
