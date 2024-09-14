using System;
using System.Linq;
using System.Text;

namespace CS_Basics
{
    class Program
    {
        internal static Calc GetCalc()
        {
            var Calc1 = new Calc();

            return Calc1;
        }

        // "Global" objects for this project
        //static Calc myCalc = new Calc();
        //public static Calc GetCalc() { return myCalc; }

        //static FSM myFSM = new FSM();
        //public static FSM GetFSM() { return myFSM; }

        static void Main(string[] args)
        {
            // Calc tests
            Tests.CalcTest();

            // FSM tests
            Tests.FSMTest();

            // Interleaved tests
            Tests.InterLeaved();

            System.Diagnostics.Debug.WriteLine("\n------------  program done -------------\n");

        }

        //static void DisplayMe(object o)
        //{
        //    System.Diagnostics.Debug.WriteLine("You are {0}", o);
        //}

    }
}




