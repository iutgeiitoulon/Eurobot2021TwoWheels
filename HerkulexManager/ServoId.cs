﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HerkulexManagerNS
{
    public enum ServoId
    {
        Drapeau = 110,
        Rack1 = 111,
        Rack2 = 112

            //position RackVertical - 790
            //position RackHorizontal - 510
            //drapeau leve - 280
            //drapeau180deg - 560
    }

    public enum Positions
    {
        RackVertical = 790,
        RackHorizontal = 510,
        DapeauLeve = 280,
        DrapeauManche = 560
    }
}