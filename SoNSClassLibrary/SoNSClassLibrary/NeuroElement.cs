using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SoNSClassLibrary
{
    /// <summary>
    /// Представляет собой абстрактный класс нейроэлемента.
    /// </summary>
    public abstract class NeuroElement : BrainObject
    {
        /// <summary>
        /// Уникальный идентификатор нейроэлемента.
        /// </summary>
        [Description("Уникальный идентификатор нейроэлемента.")]
        public Guid Id { get; private set; }
        
        #region Интервалы

        /// <summary>
        /// Время ответа, исчисляемое в миллисекундах.
        /// </summary>
        [Description("Время ответа, исчисляемое в миллисекундах.")]
        public int ResponceTime { get; set; } = 500;

        /// <summary>
        /// "Время отдыха, исчисляемое в миллисекундах."
        /// </summary>
        [Description("Время отдыха, исчисляемое в миллисекундах.")]
        public int SleepTime { get; set; } = 1500;

        #endregion

        #region Сумма

        private float _sum = 0.0f;

        /// <summary>
        /// Сумма внешних сигналов.
        /// <remarks>Значение <see cref="Sum"/> не может превышать <see cref="MaxSumm"/> или быть меньше -<see cref="MaxSumm"/>.</remarks>
        /// </summary>
        [Description("Сумма внешних сигналов.")]
        public float Sum
        {
            get { return _sum; }
            set
            {
                if (value > MaxSumm)
                {
                    _sum = MaxSumm;
                }

                else if (value < -MaxSumm)
                {
                    _sum = -MaxSumm;
                }

                else
                {
                    _sum = value;
                }
            }
        }

        private float _sumDamfer = 1.0f;

        /// <summary>
        /// Регулятор сумматора.
        /// <remarks>Значение не может быть меньше 0.</remarks>
        /// </summary>
        [Description("Регулятор сумматора.")]
        public float SumDamfer
        {
            get { return _sumDamfer; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Значение должно быть больше либо равно нулю.");

                _sumDamfer = value;
            }
        }

        private float _maxSumm = 30f;
        
        /// <summary>
        /// Максимально допустимое значение сумматора.
        /// </summary>
        [Description("Максимально допустимое значение сумматора.")]
        public float MaxSumm
        {
            get { return _maxSumm; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Значение должно быть больше либо равно нулю.");

                _maxSumm = value;
            }
        }

        #endregion

        #region Порог

        /// <summary>
        /// Верхний порог активации нейроэлемента.
        /// </summary>
        [Description("Верхний порог активации.")]
        public float ThresholdTop { get; set; } = 1.0f;

        /// <summary>
        /// Нижний порог активации.
        /// </summary>
        [Description("Нижний порог активации.")]
        public float ThresholdDown { get; set; } = -5.0f;

        #endregion

        #region Событие - активация нейроэлемента.

        /// <summary>
        /// <para>Событие, возникающее при активации нейроэлемента.</para>
        /// <remarks><para>Возбуждение передается на синапс, который, в свою очередь, генерирует новое событие, передающееся на другой нейроэлемент, который подписан на событие генерируемое синапсом.</para>
        /// <para>neuroElement1=>synapse=>neuroElement2.</para></remarks>
        /// </summary>
        public event Action NeuronActivatedEvent;

        /// <summary>
        /// Вызывает событие, возникающее при активации нейроэлемента.
        /// </summary>
        protected void OnActivatedEvent()
        {
            NeuronActivatedEvent?.Invoke();
        }

        #endregion

        #region Синапс - событие/обработчик

        /// <summary>
        /// Сила воздействия синапса на нейроэлемент.
        /// </summary>
        protected float _addForce;

        /// <summary>
        /// Подписывает нейроэлемент на событие, возникающее при передаче ему сигнала синапсом.
        /// </summary>
        /// <param name="synapse"></param>
        public void ConnectToSynapse<T>(Synapse<T> synapse) where T : new()
        {
            synapse.SynapseActivated += SynapseOnSynapseActivated;
        }

        /// <summary>
        /// Обработчик события, возникающего при передача сигнала синапсом.
        /// </summary>
        protected abstract void SynapseOnSynapseActivated<T>(object sender, T e);

        #endregion

        /// <summary>
        /// Ссылка на задачу, выполняющую активацию нейроэлемента.
        /// </summary>
        protected internal Task ActivateRef;

        /// <summary>
        /// Задача, выполняется при активации нейроэлемента.
        /// </summary>
        /// <returns></returns>
        protected internal abstract Task Activate();

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="NeuroElement"/>.
        /// </summary>
        protected NeuroElement()
        {
            Id = Guid.NewGuid();
        }
    }
}
