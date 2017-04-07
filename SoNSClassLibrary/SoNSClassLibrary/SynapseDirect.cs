// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoNSClassLibrary
{
    /// <summary>
    /// <para>Представляет собой синапс прямого действия - сигнал передается вещественным числом, действие оказывается на сумматор (<see cref="Summator"/> ).</para>
    /// <para>Положительное число - побудительное действие, отрицательное - ингибирующее (тормозящее) действие.</para>
    /// </summary>
    public class SynapseDirect : Synapse<float>
    {
        public SynapseDirect(float force, NeuroElement parent) : base(parent)
        {
            Par = force;
        }
    }
}
