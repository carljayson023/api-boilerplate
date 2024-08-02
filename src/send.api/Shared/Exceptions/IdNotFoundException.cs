using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace send.api.Shared.Exceptions
{
    public class IdNotFoundException : KeyNotFoundException
    {
        public IdNotFoundException(string key)
            : base($"{key} not found.") { }
    }
}
