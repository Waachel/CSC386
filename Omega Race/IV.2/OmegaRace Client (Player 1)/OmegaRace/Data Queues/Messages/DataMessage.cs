using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;

namespace OmegaRace
{

    [Serializable]
    public abstract class DataMessage
    {

        // abstract serialize.
        public abstract void Serialize(ref BinaryWriter writer);

        // abstract deserialize.
        public abstract void Deserialize(ref BinaryReader reader);

        // abstract execute.
        public abstract void Execute();

        // message pool reset
        public abstract void Reset();
    }
}

