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
        public SynapseModulating(float force, NeuroElement parent) : base(parent)
        {
            Par = force;
        }
    }
}
