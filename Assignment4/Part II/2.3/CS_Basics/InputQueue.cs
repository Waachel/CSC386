using System;
using System.Xml.Linq;

namespace CS_Basics
{
    class InputQueue
    {
        public enum data_type
        {
            FSM,
            CALC
        }

        public struct queue_type_data
        {
            public data_type type;
        }
        public struct queue_data
        {
            public queue_type_data data;
            public FSM_Data f;
            public Calc_Data c;
        }

        public queue_data q;
        public queue_type_data q_data;
        System.Collections.Generic.Queue<queue_data> myQ = new System.Collections.Generic.Queue<queue_data>();
        internal void add(Calc_Data calcData)
        {
            q.data.type = data_type.CALC;
            q.c = calcData;
            myQ.Enqueue(q);
        }

        internal void add(FSM_Data fsmData)
        {
            q.data.type = data_type.FSM;
            q.f = fsmData;
            myQ.Enqueue(q);
        }

        internal void process()
        {
            int count = myQ.Count;
            queue_data process_q;
            FSM myFSM = Program.GetFSM();
            Calc myCalc = Program.GetCalc();

            for (int i = 0; i < count; i++)
            {
                process_q = myQ.Dequeue();
                if (process_q.data.type == data_type.CALC)
                {
                    Calc_Data tmpData;
                    tmpData = process_q.c;
                    myCalc.doWork(tmpData);
                }
                else //data_type.FSM
                {
                    FSM_Data tmpData;
                    tmpData = process_q.f;
                    myFSM.doWork(tmpData);
                }
                
            }
        }
    }
}