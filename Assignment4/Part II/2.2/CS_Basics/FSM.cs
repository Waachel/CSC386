using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;

namespace CS_Basics
{
    enum FSM_STATE
    {
        A,
        B,
        C,
        D,
        E

    }

    public class FSM
    {
        private STATE _state = null;
        FSM_STATE type;
        private int t = 0;
        private bool ifSetOnly = true;
        private int value = 0;

        public FSM(STATE state)
        {
            this.TransitionTo(state);
        }

        public FSM()
        {
            Console.WriteLine($"Context: Transition to A.");
            this._state = new A();
            this._state.SetContext(this);
        }

        internal void advance(int v)
        {
            t = 0;
            value = v;
            ifSetOnly = false;

            if (v == 0)
            {
                Request0();
            }
            else //v = 1
            {
                Request1();
            }

            ifSetOnly = true;
        }

        internal void set(FSM_STATE s)
        {
            if (ifSetOnly == true)
            {
                t = 1;
            }

           
            if (s == FSM_STATE.A)
            {
                type = FSM_STATE.A;
                this.TransitionTo(new A());
            }
            
            else if (s == FSM_STATE.B)
            {
                type = FSM_STATE.B;
                this.TransitionTo(new B()); 
            }

            else if(s == FSM_STATE.C) 
            {
                type = FSM_STATE.C;
                this.TransitionTo(new C());             
            }

            else if(s == FSM_STATE.D) 
            {
                type = FSM_STATE.D;
                this.TransitionTo(new D());
            }

            else //s = FSM_STATE.E
            {
                type = FSM_STATE.E;
                this.TransitionTo(new E());
            }
        }

        public void TransitionTo(STATE state)
        {
            Console.WriteLine($"Context: Transition to {state.GetType().Name}.");
            this._state = state;
            this._state.SetContext(this);
            PrintMe();
        }

        public void Request0()
        {
            this._state.Handle0();
        }

        public void Request1()
        {
            this._state.Handle1();
        }

        private void PrintMe()
        {
            if (t == 0)//advance
            {
                Debug.WriteLine("FSM():      advance(" + value + ") : State_" + this._state.GetType().Name);
            }
            
            else //t = 1 set
            {
                Debug.WriteLine("FSM():    set(STATE_" + this._state.GetType().Name + ") : State_" + this._state.GetType().Name);
            }
           
        }

        internal void doWork(FSM_Data myData)
        {
            if (myData.type.type == FSM_Type.FSM_SET)
            {
                set(myData.set.s);
            }
            else //advance
            {
                advance(myData.adv.value);
            }
        }
    }
    public abstract class STATE
    {
        protected FSM _context;
        
        public void SetContext(FSM context)
        {
            this._context = context;
        }

        public abstract void Handle0();
        public abstract void Handle1();
    }

    public class A : STATE
    {
        public override void Handle0()
        {
            Console.WriteLine("State A advance 0 to State A");
            this._context.set(FSM_STATE.A);
            
        }
        
        public override void Handle1()
        {
            Console.WriteLine("State A advance 1 to State B");
            this._context.set(FSM_STATE.B);
        }
    }
    public class B : STATE
    {
        public override void Handle0()
        {
            Console.WriteLine("State B advance 0 to State E");
            this._context.set(FSM_STATE.E);
        }

        public override void Handle1()
        {
            Console.WriteLine("State B advance 1 to State C");
            this._context.set(FSM_STATE.C);
        }
    }

    public class C : STATE
    {
        public override void Handle0()
        {
            Console.WriteLine("State C advance 0 to State D");
            this._context.set(FSM_STATE.D);
        }

        public override void Handle1()
        {
            Console.WriteLine("State C advance 1 to State E");
            this._context.set(FSM_STATE.E);
        }
    }

    public class D : STATE
    {
        public override void Handle0()
        {
            Console.WriteLine("State D advance 0 to State D");
            this._context.set(FSM_STATE.D);
        }

        public override void Handle1()
        {
            Console.WriteLine("State D advance 1 to State B");
            this._context.set(FSM_STATE.B);
        }
    }

    public class E : STATE
    {
        public override void Handle0()
        {
            Console.WriteLine("State E advance 0 to State C");
            this._context.set(FSM_STATE.C);
        }

        public override void Handle1()
        {
            Console.WriteLine("State E advance 1 to State A");
            this._context.set(FSM_STATE.A);
        }
    }
}
