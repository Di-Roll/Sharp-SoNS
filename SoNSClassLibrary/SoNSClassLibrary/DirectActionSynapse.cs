using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoNSClassLibrary
{
    /// <summary>
    /// 
    /// </summary>
    public class DirectActionSynapse : Synapse<float>
    {
        public DirectActionSynapse(float force, NeuroElement parent) : base(parent)
        {
            Par = force;
        }
    }
}
