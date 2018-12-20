﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaKioscoCash.Interfaces
{
    public interface IDispenser
    {
        void open();
        void close();
        bool isConnection();
        void enable();
        void returnCash(int [] count);

    }
}
