﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.EventHost
{
    public interface IEvent
    {
        void Trigger();
    }
}
