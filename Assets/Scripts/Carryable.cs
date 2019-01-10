using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    interface Carryable
    {
        eCarryableType pType { get; set; }
    }

    class CarryableCustomer : Carryable
    {

    }


}


