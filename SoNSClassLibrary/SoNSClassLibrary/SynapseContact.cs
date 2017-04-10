using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 
/// </summary>
namespace SoNSClassLibrary
{
    public class SynapseContact : Synapse<bool>
    {
        public SynapseContact(NeuroElement parent, bool useDelay, bool message) : base(parent, useDelay)
        {
            Par = message;
        }
    }
}
