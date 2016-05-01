using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZPO.Core.Conditions
{
    public class CreateModelException : Exception
    {
        public CreateModelException()
        {
        }

        public CreateModelException(string message) : base(message)
        {
        }
    }
}
