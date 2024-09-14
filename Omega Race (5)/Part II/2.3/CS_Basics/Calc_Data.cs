using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CS_Basics
{
     enum Calc_Type
    {
        CALC_SET,
        CALC_ADD,
        CALC_SUB,
        CALC_MULT,

    }

    struct c_data
    {
        public Calc_Type type;
        public int value;
    }
    class Calc_Data
    {
        public c_data d;

        public Calc_Data()
        {
        }

        public Calc_Data(Calc_Type c_type, int v)
        {
            d.type = c_type;
            d.value = v;
        }

        internal void set(Calc_Type c_type, int v)
        {
            d.type = c_type;
            d.value = v;
        }
    }
}
