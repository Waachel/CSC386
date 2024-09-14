using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CS_Basics
{

    class Calc
    {
        private int type = 0;
        private int result = 0;
        private int value = 0;
        public void add(int v)
        {
            type = 0;
            value = v;
            result  = result + v;
            PrintMe();
        }

        public void mult(int v)
        {
            type = 2;
            value = v;
            result = result * v;
            PrintMe();
        }

        public void set(int v)
        {
            type = 3;
            value = v;
            result = v;
            PrintMe();
        }

        public void sub(int v)
        {
            type = 1;
            value = v;
            result = result - v;
            PrintMe();
        }

        private void PrintMe()
        {
            if (type == 0) //add
            {
                Debug.WriteLine("calc():    add(" + value + ") : " + result);
            }
            
            else if (type == 1) //sub
            {
                Debug.WriteLine("calc():    sub(" + value + ") : " + result);
            }
            else if (type == 2) //mult
            {
                Debug.WriteLine("calc():   mult(" + value + ") : " + result);
            }
            else //set
            {
                Debug.WriteLine("calc():    set(" + value + ") : " + result);
            }
        }

        public int GetAnswer()
        {
            return result;
        }

        internal void doWork(Calc_Data data)
        {
            if (data.d.type == Calc_Type.CALC_MULT)
            {
                mult(data.d.value);
            }

            else if (data.d.type == Calc_Type.CALC_ADD)
            {
                add(data.d.value);
            }

            else if (data.d.type == Calc_Type.CALC_SUB)
            {
                sub(data.d.value);
            }

            else //Calc.Type.CALC_SET
            {
                set(data.d.value);
            }
        }
    }

}
