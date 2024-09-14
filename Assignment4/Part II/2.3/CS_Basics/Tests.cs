namespace CS_Basics
{
    class Tests
    {
        
        static public void CalcTest() 
        {

            // shorter access to main Calc instance
            Calc myCalc = Program.GetCalc();

            System.Diagnostics.Debug.WriteLine("\n------------ Calc Phase 1: Explicit Calls ---------------\n");

            myCalc.set(3);
            myCalc.add(2);
            myCalc.sub(8);
            myCalc.mult(2);
            myCalc.sub(5);
            myCalc.add(4);
            myCalc.add(22);
            myCalc.sub(4);
            myCalc.mult(2);
            myCalc.sub(5);
            myCalc.set(11);
            myCalc.add(2);
            myCalc.sub(7);
            myCalc.mult(3);
            myCalc.sub(5);

            System.Diagnostics.Debug.WriteLine("\n------------ Calc Phase 2: Data Driven ---------------\n");

            Calc_Data data = new Calc_Data();

            // myCalc.set(3);
            data.set(Calc_Type.CALC_SET, 3);
            myCalc.doWork(data);

            // myCalc.add(2);
            data.set(Calc_Type.CALC_ADD, 2);
            myCalc.doWork(data);

            // myCalc.sub(8);
            data.set(Calc_Type.CALC_SUB, 8);
            myCalc.doWork(data);

            // myCalc.mult(2);
            data.set(Calc_Type.CALC_MULT, 2);
            myCalc.doWork(data);

            // myCalc.sub(5);
            data.set(Calc_Type.CALC_SUB, 5);
            myCalc.doWork(data);

            // myCalc.add(4);
            data.set(Calc_Type.CALC_ADD, 4);
            myCalc.doWork(data);

            // myCalc.add(22);
            data.set(Calc_Type.CALC_ADD, 22);
            myCalc.doWork(data);

            // myCalc.sub(4);
            data.set(Calc_Type.CALC_SUB, 4);
            myCalc.doWork(data);

            // myCalc.mult(2);
            data.set(Calc_Type.CALC_MULT, 2);
            myCalc.doWork(data);

            // myCalc.sub(5);
            data.set(Calc_Type.CALC_SUB, 5);
            myCalc.doWork(data);

            // myCalc.set(11);
            data.set(Calc_Type.CALC_SET, 11);
            myCalc.doWork(data);

            // myCalc.add(2);
            data.set(Calc_Type.CALC_ADD, 2);
            myCalc.doWork(data);

            // myCalc.sub(7);
            data.set(Calc_Type.CALC_SUB, 7);
            myCalc.doWork(data);

            // myCalc.mult(3);
            data.set(Calc_Type.CALC_MULT, 3);
            myCalc.doWork(data);

            // myCalc.sub(5);
            data.set(Calc_Type.CALC_SUB, 5);
            myCalc.doWork(data);

            System.Diagnostics.Debug.WriteLine("\n------------ Calc Phase 3: Queued Data Driven ---------------\n");

            // Creates and initializes a new Queue.
            System.Collections.Generic.Queue<Calc_Data> myQ = new System.Collections.Generic.Queue<Calc_Data>();

            // myCalc.set(3);
            data = new Calc_Data(Calc_Type.CALC_SET, 3);
            myQ.Enqueue(data);

            // myCalc.add(2);
            data = new Calc_Data(Calc_Type.CALC_ADD, 2);
            myQ.Enqueue(data);

            // myCalc.sub(8);
            data = new Calc_Data(Calc_Type.CALC_SUB, 8);
            myQ.Enqueue(data);

            // myCalc.mult(2);
            data = new Calc_Data(Calc_Type.CALC_MULT, 2);
            myQ.Enqueue(data);

            // myCalc.sub(5);
            data = new Calc_Data(Calc_Type.CALC_SUB, 5);
            myQ.Enqueue(data);

            // myCalc.add(4);
            data = new Calc_Data(Calc_Type.CALC_ADD, 4);
            myQ.Enqueue(data);

            // myCalc.add(22);
            data = new Calc_Data(Calc_Type.CALC_ADD, 22);
            myQ.Enqueue(data);

            // myCalc.sub(4);
            data = new Calc_Data( Calc_Type.CALC_SUB, 4);
            myQ.Enqueue(data);

            // myCalc.mult(2);
            data = new Calc_Data(Calc_Type.CALC_MULT, 2);
            myQ.Enqueue(data);

            // myCalc.sub(5);
            data = new Calc_Data(Calc_Type.CALC_SUB, 5);
            myQ.Enqueue(data);

            // myCalc.set(11);
            data = new Calc_Data(Calc_Type.CALC_SET, 11);
            myQ.Enqueue(data);

            // myCalc.add(2);
            data = new Calc_Data(Calc_Type.CALC_ADD, 2);
            myQ.Enqueue(data);

            // myCalc.sub(7);
            data = new Calc_Data(Calc_Type.CALC_SUB, 7);
            myQ.Enqueue(data);

            // myCalc.mult(3);
            data = new Calc_Data( Calc_Type.CALC_MULT, 3);
            myQ.Enqueue(data);

            // myCalc.sub(5);
            data = new Calc_Data(Calc_Type.CALC_SUB, 5);
            myQ.Enqueue(data);


            // Now the queue is NOW filled
            // print the contents      
            while (myQ.Count > 0)
            {
                Calc_Data tmpData = myQ.Dequeue();
                myCalc.doWork(tmpData);
            }
            
        }



        static public void FSMTest()
        {
            
            System.Diagnostics.Debug.WriteLine("\n------------ FSM Phase 1: Explicit Calls ---------------\n");

            FSM myFSM = Program.GetFSM();

            myFSM.advance(0);  //a
            myFSM.advance(1);  //b
            myFSM.advance(0);  //e
            myFSM.advance(1);  //a

            myFSM.advance(1);  //b
            myFSM.advance(0);  //e
            myFSM.advance(0);  //c
            myFSM.advance(1);  //e
            myFSM.advance(1);  //a

            myFSM.advance(1);  //b
            myFSM.advance(1);  //c
            myFSM.advance(0);  //d
            myFSM.advance(0);  //d
            myFSM.advance(1);  //b

            myFSM.set(FSM_STATE.C);
            myFSM.advance(0);
            myFSM.advance(0);
            myFSM.advance(0);

            myFSM.set(FSM_STATE.C);
            myFSM.advance(1);
            myFSM.advance(0);
            myFSM.advance(0);

            myFSM.set(FSM_STATE.B);
            myFSM.advance(1);
            myFSM.advance(1);
            myFSM.advance(0);

            myFSM.set(FSM_STATE.D);
            myFSM.advance(1);
            myFSM.advance(0);
            myFSM.advance(1);

              System.Diagnostics.Debug.WriteLine("\n------------ FSM Phase 2: Data Driven ---------------\n");

              FSM_Data myData = new FSM_Data();

              //myFSM.advance(0);  //a
              myData.SetAdvData(0);
              myFSM.doWork(myData);

              //myFSM.advance(1);  //b
              myData.SetAdvData(1);
              myFSM.doWork(myData);

              //myFSM.advance(0);  //e
              myData.SetAdvData(0);
              myFSM.doWork(myData);

              //myFSM.advance(1);  //a
              myData.SetAdvData(1);
              myFSM.doWork(myData);

              //myFSM.advance(1);  //b
              myData.SetAdvData(1);
              myFSM.doWork(myData);

              //myFSM.advance(0);  //e
              myData.SetAdvData(0);
              myFSM.doWork(myData);

              //myFSM.advance(0);  //c
              myData.SetAdvData(0);
              myFSM.doWork(myData);

              //myFSM.advance(1);  //e
              myData.SetAdvData(1);
              myFSM.doWork(myData);

              //myFSM.advance(1);  //a
              myData.SetAdvData(1);
              myFSM.doWork(myData);

              //myFSM.advance(1);  //b
              myData.SetAdvData(1);
              myFSM.doWork(myData);

              //myFSM.advance(1);  //c
              myData.SetAdvData(1);
              myFSM.doWork(myData);

              //myFSM.advance(0);  //d
              myData.SetAdvData(0);
              myFSM.doWork(myData);

              //myFSM.advance(0);  //d
              myData.SetAdvData(0);
              myFSM.doWork(myData);

              //myFSM.advance(1);  //b
              myData.SetAdvData(1);
              myFSM.doWork(myData);

              //myFSM.set(FSM_StateEnum.STATE_C);
              myData.SetStateData(FSM_STATE.C);
              myFSM.doWork(myData);

              //myFSM.advance(0);
              myData.SetAdvData(0);
              myFSM.doWork(myData);

              //myFSM.advance(0);
              myData.SetAdvData(0);
              myFSM.doWork(myData);

              //myFSM.advance(0);
              myData.SetAdvData(0);
              myFSM.doWork(myData);

              //myFSM.set(FSM_StateEnum.STATE_C);
              myData.SetStateData(FSM_STATE.C);
              myFSM.doWork(myData);

              //myFSM.advance(1);
              myData.SetAdvData(1);
              myFSM.doWork(myData);

              //myFSM.advance(0);
              myData.SetAdvData(0);
              myFSM.doWork(myData);

              //myFSM.advance(0);
              myData.SetAdvData(0);
              myFSM.doWork(myData);

              //myFSM.set(FSM_StateEnum.STATE_B);
              myData.SetStateData(FSM_STATE.B);
              myFSM.doWork(myData);

              //myFSM.advance(1);
              myData.SetAdvData(1);
              myFSM.doWork(myData);

              //myFSM.advance(1);
              myData.SetAdvData(1);
              myFSM.doWork(myData);

              //myFSM.advance(0);
              myData.SetAdvData(0);
              myFSM.doWork(myData);

              //myFSM.set(FSM_StateEnum.STATE_D);
              myData.SetStateData(FSM_STATE.D);
              myFSM.doWork(myData);

              //myFSM.advance(1);
              myData.SetAdvData(1);
              myFSM.doWork(myData);

              //myFSM.advance(0);
              myData.SetAdvData(0);
              myFSM.doWork(myData);

              //myFSM.advance(1);
              myData.SetAdvData(1);
              myFSM.doWork(myData);


            System.Diagnostics.Debug.WriteLine("\n------------ FSM Phase 3: Queued Data Driven ---------------\n");

            System.Collections.Generic.Queue<FSM_Data> myQ_FSM = new System.Collections.Generic.Queue<FSM_Data>();

            myData = new FSM_Data();

            //myFSM.advance(0);  //a
            myData = new FSM_Data();
            myData.SetAdvData(0);
            myQ_FSM.Enqueue(myData);

            //myFSM.advance(1);  //b
            myData = new FSM_Data();
            myData.SetAdvData(1);
            myQ_FSM.Enqueue(myData);

            //myFSM.advance(0);  //e
            myData = new FSM_Data();
            myData.SetAdvData(0);
            myQ_FSM.Enqueue(myData);

            //myFSM.advance(1);  //a
            myData = new FSM_Data();
            myData.SetAdvData(1);
            myQ_FSM.Enqueue(myData);

            //myFSM.advance(1);  //b
            myData = new FSM_Data();
            myData.SetAdvData(1);
            myQ_FSM.Enqueue(myData);

            //myFSM.advance(0);  //e
            myData = new FSM_Data();
            myData.SetAdvData(0);
            myQ_FSM.Enqueue(myData);

            //myFSM.advance(0);  //c
            myData = new FSM_Data();
            myData.SetAdvData(0);
            myQ_FSM.Enqueue(myData);

            //myFSM.advance(1);  //e
            myData = new FSM_Data();
            myData.SetAdvData(1);
            myQ_FSM.Enqueue(myData);

            //myFSM.advance(1);  //a
            myData = new FSM_Data();
            myData.SetAdvData(1);
            myQ_FSM.Enqueue(myData);

            //myFSM.advance(1);  //b
            myData = new FSM_Data();
            myData.SetAdvData(1);
            myQ_FSM.Enqueue(myData);

            //myFSM.advance(1);  //c
            myData = new FSM_Data();
            myData.SetAdvData(1);
            myQ_FSM.Enqueue(myData);

            //myFSM.advance(0);  //d
            myData = new FSM_Data();
            myData.SetAdvData(0);
            myQ_FSM.Enqueue(myData);

            //myFSM.advance(0);  //d
            myData = new FSM_Data();
            myData.SetAdvData(0);
            myQ_FSM.Enqueue(myData);

            //myFSM.advance(1);  //b
            myData = new FSM_Data();
            myData.SetAdvData(1);
            myQ_FSM.Enqueue(myData);

            //myFSM.set(FSM_StateEnum.STATE_C);
            myData = new FSM_Data();
            myData.SetStateData(FSM_STATE.C);
            myQ_FSM.Enqueue(myData);

            //myFSM.advance(0);
            myData = new FSM_Data();
            myData.SetAdvData(0);
            myQ_FSM.Enqueue(myData);

            //myFSM.advance(0);
            myData = new FSM_Data();
            myData.SetAdvData(0);
            myQ_FSM.Enqueue(myData);

            //myFSM.advance(0);
            myData = new FSM_Data();
            myData.SetAdvData(0);
            myQ_FSM.Enqueue(myData);

            //myFSM.set(FSM_StateEnum.STATE_C);
            myData = new FSM_Data();
            myData.SetStateData(FSM_STATE.C);
            myQ_FSM.Enqueue(myData);

            //myFSM.advance(1);
            myData = new FSM_Data();
            myData.SetAdvData(1);
            myQ_FSM.Enqueue(myData);

            //myFSM.advance(0);
            myData = new FSM_Data();
            myData.SetAdvData(0);
            myQ_FSM.Enqueue(myData);

            //myFSM.advance(0);
            myData = new FSM_Data();
            myData.SetAdvData(0);
            myQ_FSM.Enqueue(myData);

            //myFSM.set(FSM_StateEnum.STATE_B);
            myData = new FSM_Data();
            myData.SetStateData(FSM_STATE.B);
            myQ_FSM.Enqueue(myData);

            //myFSM.advance(1);
            myData = new FSM_Data();
            myData.SetAdvData(1);
            myQ_FSM.Enqueue(myData);

            //myFSM.advance(1);
            myData = new FSM_Data();
            myData.SetAdvData(1);
            myQ_FSM.Enqueue(myData);

            //myFSM.advance(0);
            myData = new FSM_Data();
            myData.SetAdvData(0);
            myQ_FSM.Enqueue(myData);

            //myFSM.set(FSM_StateEnum.STATE_D);
            myData = new FSM_Data();
            myData.SetStateData(FSM_STATE.D);
            myQ_FSM.Enqueue(myData);

            //myFSM.advance(1);
            myData = new FSM_Data();
            myData.SetAdvData(1);
            myQ_FSM.Enqueue(myData);

            //myFSM.advance(0);
            myData = new FSM_Data();
            myData.SetAdvData(0);
            myQ_FSM.Enqueue(myData);

            //myFSM.advance(1);
            myData = new FSM_Data();
            myData.SetAdvData(1);
            myQ_FSM.Enqueue(myData);


            // Now the queue is NOW filled
            // print the contents      
            int count = myQ_FSM.Count;

            for (int i = 0; i < count; i++)
            {
                FSM_Data tmpData = myQ_FSM.Dequeue();
                myFSM.doWork(tmpData);
            }
            
        }



        static public void InterLeaved()
        {
            
            System.Diagnostics.Debug.WriteLine("\n------------ Interleaved Tests(): with data driven queue ---------------\n");

            InputQueue inQueue = new InputQueue();

            // temp data to fill queue
            Calc_Data calcData;
            FSM_Data fsmData;

            // -- 0  --- 
            calcData = new Calc_Data(Calc_Type.CALC_SET, 3);
            inQueue.add(calcData);

            // -- 1  --- 
            calcData = new Calc_Data(Calc_Type.CALC_ADD, 5);
            inQueue.add(calcData);

            // -- 2  --- 
            calcData = new Calc_Data(Calc_Type.CALC_SUB, 9);
            inQueue.add(calcData);

            // -- 3  --- 
            fsmData = new FSM_Data();
            fsmData.SetStateData(FSM_STATE.C);
            inQueue.add(fsmData);

            // -- 4 ---  
            fsmData = new FSM_Data();
            fsmData.SetStateData(FSM_STATE.B);
            inQueue.add(fsmData);

            // -- 5  --- 
            calcData = new Calc_Data(Calc_Type.CALC_MULT, 3);
            inQueue.add(calcData);

            // -- 6  --- 
            fsmData = new FSM_Data();
            fsmData.SetAdvData(1);
            inQueue.add(fsmData);

            // -- 7  --- 
            calcData = new Calc_Data(Calc_Type.CALC_SUB, 5);
            inQueue.add(calcData);

            // -- 8  --- 
            fsmData = new FSM_Data();
            fsmData.SetAdvData(0);
            inQueue.add(fsmData);

            // -- 9  ---      
            fsmData = new FSM_Data();
            fsmData.SetAdvData(0);
            inQueue.add(fsmData);

            // -- 10  ---
            fsmData = new FSM_Data();
            fsmData.SetStateData(FSM_STATE.B);
            inQueue.add(fsmData);

            // -- 11  --- 
            calcData = new Calc_Data(Calc_Type.CALC_MULT, 3);
            inQueue.add(calcData);

            // -- 12  --- 
            fsmData = new FSM_Data();
            fsmData.SetAdvData(1);
            inQueue.add(fsmData);

            // -- 13  --- 
            fsmData = new FSM_Data();
            fsmData.SetAdvData(0);
            inQueue.add(fsmData);

            // -- 14  --- 
            calcData = new Calc_Data(Calc_Type.CALC_ADD, 2);
            inQueue.add(calcData);

            // -- 15  --- 
            calcData = new Calc_Data(Calc_Type.CALC_SUB, 7);
            inQueue.add(calcData);

            // -- 16  --- 
            fsmData = new FSM_Data();
            fsmData.SetAdvData(1);
            inQueue.add(fsmData);

            // -- 17  --- 
            fsmData = new FSM_Data();
            fsmData.SetAdvData(0);
            inQueue.add(fsmData);

            // -- 18  --- 
            fsmData = new FSM_Data();
            fsmData.SetAdvData(0);
            inQueue.add(fsmData);

            // -- 19  --- 
            calcData = new Calc_Data(Calc_Type.CALC_ADD, 10);
            inQueue.add(calcData);

            // -- 20  --- 
            fsmData = new FSM_Data();
            fsmData.SetAdvData(0);
            inQueue.add(fsmData);

            // now that they are in queue, process them
            inQueue.process();

            
        }
        
    }
}
