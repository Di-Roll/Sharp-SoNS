using System;
using System.ComponentModel;
using System.Timers;

namespace SoNSClassLibrary
{
    /// <summary>
    /// Представляет собой цикл, на основе которого проихсодит обновление состояний объектов.
    /// </summary>
    public class UpdateLoop
    {
        /// <summary>
        /// Таймер, генерирующий событие <see cref="OnUpdate"/> через равные интервалы времени.
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// Событие, возникающее через установленный интервал, при которым происходит обновление состояний объектов. Аналог метода "Update" в Unity.
        /// </summary>
        protected internal static event Action OnUpdate;

        /// <summary>
        /// Возврачает или устанавливает значение, определеяющее будет ли происходить вызов события <see cref="OnUpdate"/>.
        /// </summary>
        [Description("Возврачает или устанавливает значение, определеяющее будет ли происходить вызов события " + nameof(OnUpdate) + ".")]
        public bool Enabled
        {
            get { return _timer.Enabled; }
            set { _timer.Enabled = value; }
        }

        /// <summary>
        /// Интервал, по истечении которого возникат событие <see cref="OnUpdate"/>. Задается в миллисекундах.
        /// </summary>
        [Description("Интервал, по истечении которого возникат событие " + nameof(OnUpdate) + ". Задается в миллисекундах.")]
        public double Interval
        {
            get { return _timer.Interval; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Значение должно быть больше либо равно нулю.");
                }
                _timer.Interval = value;
            }
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="BrainLoop"/>.
        /// </summary>
        /// <param name="interval">Значение, определяющее интервал, по истечении которого возникат событие <see cref="OnUpdate"/>. Задается в миллисекундах.</param>
        public UpdateLoop(int interval)
        {
            _timer = new Timer(interval);
            _timer.Elapsed += TimerOnElapsed;
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            // надо ли останавливать таймер?
            _timer.Stop();
            OnUpdate?.Invoke();
            _timer.Start();
        }
    }
}
