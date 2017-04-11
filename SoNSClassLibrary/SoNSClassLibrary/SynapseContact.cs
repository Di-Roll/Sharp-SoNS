using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoNSClassLibrary
{
    /// <summary>
    /// Представляет собой контактный синапс - сигнал передается сообщением, активирует нейроэлемент если он не активен и не находится в состоянии отдыха, иначе игнорируется.
    /// <remarks></remarks>
    /// </summary>
    public class SynapseContact : Synapse<bool>
    {
        public SynapseContact(NeuroElement parent, bool useDelay, bool message) : base(parent, useDelay)
        {
            Par = message;
        }
    }
}
