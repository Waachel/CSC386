using System;

namespace CS_Basics
{
    enum FSM_Type
    {
        FSM_SET,
        FSM_ADV
    }

    struct fsm_type_data
    {
        public FSM_Type type;
    }
    struct fsm_set_data
    {
        public FSM_STATE s;
    }

    struct fsm_adv_data
    {
        public int value;
    }
    class FSM_Data
    {
        public fsm_type_data type;
        public fsm_set_data set;
        public fsm_adv_data adv;
        internal void SetAdvData(int v)
        {
            type.type = FSM_Type.FSM_ADV;
            adv.value = v;
        }

        internal void SetStateData(FSM_STATE d)
        {
            type.type = FSM_Type.FSM_SET;
            set.s = d;
        }
    }
}