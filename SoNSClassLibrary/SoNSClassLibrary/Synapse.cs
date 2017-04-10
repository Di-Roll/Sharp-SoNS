// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoNSClassLibrary
{
    /// <summary>
    /// Представляет собой абстрактный класс синапса.
    /// </summary>
    public abstract class Synapse<T> where T : new()
    {
        /// <summary>
        /// Уникальный идентификатор синапса.
        /// </summary>
        public Guid Id { get; private set; }

        #region Синаптическая задержка ( wip... )

        /// <summary>
        /// Определяет необходимо ли использовать задержку.
        /// </summary>
        public bool UseDelay { get; private set; }

        private int _minDelay = 0;

        /// <summary>
        /// Минимальный интервал синаптической задержки, исчисляемый в миллисекундах.
        /// </summary>
        protected internal int MinDelay
        {
            get { return _minDelay; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Значение должно быть больше либо равно нулю.");

                _minDelay = value;
            }
        }

        private int _defaultDelay = 5000;

        /// <summary>
        /// Начальная величина синаптической задержки, исчисляемая в миллисекундах.
        /// </summary>
        protected internal int DefaultDelay
        {
            get { return _defaultDelay; }
            set
            {
                if (value > MaxDelay)
                {
                    _defaultDelay = MaxDelay;
                }

                else if (value < MinDelay)
                {
                    _defaultDelay = MinDelay;
                }

                else
                {
                    _defaultDelay = value;
                }
            }
        }

        /// <summary>
        /// Максимальный интервал синаптической задержки, исчисляемый в миллисекундах.
        /// </summary>
        protected internal int MaxDelay { get; set; } = 5000; 
        #endregion

        /// <summary>
        /// Событие, возникающее при активации синапса.
        /// </summary>
        protected internal event EventHandler<T> SynapseActivated;

        /// <summary>
        /// Параметр типа <see cref="T"/>, отвечает за передаваемое синапсом значение (сила воздействия).
        /// </summary>
        public T Par { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Synapse"/>.
        /// </summary>
        /// <param name="force">Сила воздействия синапса.</param>
        /// <param name="parent">Нейроэлемент (родитель) синапса.</param>
        protected Synapse(NeuroElement parent, bool useDelay)
        {
            UseDelay = useDelay;
            Id = Guid.NewGuid();

            // не указает нейроэлемент-родитель синапса.
            if (parent == null)
                throw new ArgumentException("Не указан родитель синапса.");

            // подписываемся на событие активации нейроэлемента-родителя
            parent.NeuroElementActivatedEvent += ActivatedByNeuronEvent;
        }

        /// <summary>
        /// Происходит при активации нейроэлемента (родителя).
        /// </summary>
        private async void ActivatedByNeuronEvent()
        {
            // если необходимо использовать задержку
            if(UseDelay)
            {
                await Task.Delay(DefaultDelay);
            }

            // РЕАЛИЗОВАТЬ УМЕНЬШЕНИЕ / УВЕЛИЧЕНИЕ ЗАДЕРЖКИ В ЗАВИСИМОСТИ ОТ ЧАСТОТЫ ВЫЗОВОВ И Т.Д.

            // передаем параметр (сила воздействия) нейроэлементу-получателю, с которым связан синапс.
            // [нейроэлемент-родитель]=>[синапс]=(Par)=>[нейроэлемент-получатель]
            SynapseActivated?.Invoke(this, Par);
        }
    }
}