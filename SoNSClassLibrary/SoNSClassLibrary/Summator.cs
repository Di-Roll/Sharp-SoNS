using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SoNSClassLibrary
{
    public class Summator : NeuroElement
    {
        public Summator():base()
        {

        }

        protected override async Task OnUpdate()
        {
            await Task.Run(async () =>
            {
                // ожидаем входящие сигналы, иначе говоря не создана задача активации нейроэлемента
                if (ActivateRef == null)
                {
                    // преодолен порог активации
                    if (Sum > ThresholdTop)
                    {
                        // Создаем задачу активации нейроэлемента и после выполнения обнуляем ее, чтобы нейроэлемент снова принимал внешние сигналы
                        ActivateRef = Activate().ContinueWith(task => { ActivateRef = null; });
                        await ActivateRef;
                    }
                }
                else
                {
                    
                }

                // изменяем значения сумматора (демпфер)

                // если величина меньше либо равна величине демпфера - обнуляем
                if (Math.Abs(Sum) <= SumDamfer) Sum = 0f;
                // если величина больше величины демпфера - отнимаем величинк демпфера
                if (Sum > SumDamfer) Sum -= SumDamfer;
                // если величина меньше величины демпфера - отнимаем величинк демпфера
                if (Sum < -SumDamfer) Sum += SumDamfer;

                // добавляем к сумматору сигналы, полученные от синапсов
                Sum += _addForce;
                _addForce = 0;
            });
        }

        /// <summary>
        /// Происходит при получении сигнала от синапса.
        /// </summary>
        protected override void SynapseOnSynapseActivated<T>(object sender, T e)
        {
            // определяем тип синапса, вызвавшего событие и реагируем/не реагируем.

            if(sender.GetType().Equals(typeof(SynapseDirect)))
            {
                _addForce += float.Parse(e.ToString());
            }

            else
            {
                throw new NotImplementedException("Не реализовано!");
            }
        }

        /// <summary>
        /// Задача, выполняется при активации сумматора.
        /// </summary>
        /// <returns></returns>
        protected internal override async Task Activate()
        {
            await Task.Run(() =>
            {
                // ожидаем заданный интервал времени, перед возвращением ответа
                Thread.Sleep(ResponceTime);
                // передача возбуждения
                OnActivatedEvent();
                // ожидаем заданный интервал времени, перед возвращением к получению внешних сигналов
                Thread.Sleep(SleepTime);
            });
        }
    }
}
