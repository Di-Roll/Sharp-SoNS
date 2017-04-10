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
    /// <para>Представляет собой синапс модулирующего действия - сигнал передается вещественным числом, действие оказывается на дополнительную часть порога, изменяя чувствительность нейроэлемента.</para>
    /// <para>Положительное число - снижение чувствительности (повышение порога), отрицательное число - повышение чувствительности.</para>
    /// <para>Сигнал синапса игнорируется нейроэлементом типа сумматор (<see cref="Summator"/>) по причине отсутствия в нём соответствующего механизма.</para>
    /// </summary>
    public class SynapseModulating : Synapse<float>
    {
        public SynapseModulating(NeuroElement parent, bool useDelay, float force) : base(parent, useDelay)
        {
            Par = force;
        }
    }
}
