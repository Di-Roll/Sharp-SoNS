using System;
using System.Collections.Generic;
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
        public Synapse(NeuroElement parent)
        {
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
        private void ActivatedByNeuronEvent()
        {
            // передаем параметр (сила воздействия) нейроэлементу-получателю, с которым связан синапс.
            // [нейроэлемент-родитель]=>[синапс]=(Par)=>[нейроэлемент-получатель]
            SynapseActivated?.Invoke(this, Par);
        }
    }
}