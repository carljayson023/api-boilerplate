using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace send.api.Shared.Exceptions
{
    public enum ConflictCode
    {
        already_exist,
        already_active,
        already_inactive,
        doest_exist,
    }

    public class DomainException : Exception
    {
        public ConflictCode Code { get; }
        public string Detail { get; }

        public DomainException(ConflictCode code, string message
            , string detail = null)
            : base(message)
        {
            Code = code;
            Detail = detail;
        }
    }
}
