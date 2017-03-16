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
        //protected internal event Action<float> SynapseActivated;

        protected internal event EventHandler<T> SynapseActivated;

        public T Par { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Synapse"/>.
        /// </summary>
        /// <param name="force">Сила воздействия синапса.</param>
        /// <param name="parent">Нейроэлемент (родитель) синапса.</param>
        public Synapse(NeuroElement parent)
        {
            Id = Guid.NewGuid();

            if (parent == null)
                throw new ArgumentException("Не указан родитель синапса.");

            parent.NeuronActivatedEvent += ActivatedByNeuronEvent;
        }

        /// <summary>
        /// Происходит при активации нейроэлемента (родителя).
        /// </summary>
        private void ActivatedByNeuronEvent()
        {
            SynapseActivated?.Invoke(this, Par);
        }
    }
}