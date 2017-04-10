// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SoNSClassLibrary
{
    /// <summary>
    /// Модулятор.
    /// </summary>
    class Modulator : NeuroElement
    {
        private object ModulatingSynapseForceLocker;

        /// <summary>
        /// Сила воздействия синапса модулирующего действия на нейроэлемент.
        /// </summary>
        protected float ModulatingSynapseForce { get; set; }

        #region Интервалы

        /// <summary>
        /// Время адаптации нейроэлемента, исчисляемое в секундах.
        /// </summary>
        [Description("Время адаптации нейроэлемента, исчисляемое в секундах.")]
        public float AdaptationTime { get; set; } = 5000;

        /// <summary>
        /// Время, затрачиваемое нейроэлементом на оценку, исчисляемое в секундах.
        /// </summary>
        [Description("Время, затрачиваемое нейроэлементом на оценку, исчисляемое в секундах.")]
        public int EvaluationTime { get; set; } = 2000;

        #endregion

        #region Порог

        private float _defaultThreshold = 1f;

        /// <summary>
        /// Стартовый порог нейроэлемента. При адаптации, общее значение порога не может опуститься ниже стартового порога.
        /// </summary>
        [Description("Стартовый порог нейроэлемента. При адаптации, общее значение порога не может опуститься ниже стартового порога.")]
        public float DefaultThreshold
        {
            get { return _defaultThreshold; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Значение должно быть больше либо равно нулю.");
                }
                _defaultThreshold = value;
            }
        }

        /// <summary>
        /// Дополнительная величина порога.
        /// </summary>
        [Description("Дополнительная величина порога.")]
        public float ExtraThreshold { get; set; } = 0f;

        /// <summary>
        /// Величина, на которую увеличивается верхний порог активации.
        /// </summary>
        [Description("Величина, на которую увеличивается верхний порог активации.")]
        public float ThresholdIncreaseStep = 1;

        /// <summary>
        /// Регулятор фактического значения порога.
        /// </summary>
        [Description("Регулятор фактического значения порога.")]
        public float DempferBonusThreshold = 1.0f;

        #endregion

        #region Счетчик повторов

        /// <summary>
        /// Счетчик повторов. Увеличивается каждый раз, при активации нейроэлемента во время оценки.
        /// </summary>
        [Description("Счетчик повторов. Значение счетчика увеличивается, если при активации нейроэлемент производит оценку.")]
        public int RepeatCounter { get; set; } = 0;

        /// <summary>
        /// Максимально допустимое число повторений.
        /// </summary>
        [Description("Максимально допустимое число повторений.")]
        public int MaxRepeatCount { get; set; } = 5;

        #endregion

        #region Оценка (Привыкание)

        /// <summary>
        /// Выполняет ожидание в течение заданного интервала времени - <see cref="EvaluationTime"/>.
        /// </summary>
        private async Task Evaluation(CancellationToken token)
        {
            await Task.Delay(EvaluationTime, token);
        }

        /// <summary>
        /// Ссылка на задачу <see cref="Evaluation"/>.
        /// </summary>
        private Task EvaluationRef;

        /// <summary>
        /// Источник токена отмены, для задачи <see cref="Evaluation"/>.
        /// </summary>
        private CancellationTokenSource EvaluationTokenSource;

        #endregion

        #region Возвращение к исходному состоянию

        /// <summary>
        /// Выполняет возвращение порога <see cref="ThresholdTop"/> к исходному состоянию.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task InitialThreshold(CancellationToken token)
        {
            //try
            //{
            while (AdaptationTime > 0 && (ThresholdTop > DefaultThreshold))
            {
                token.ThrowIfCancellationRequested(); // каждый раз true?
                await Task.Delay(TimeSpan.FromMilliseconds(AdaptationTime));
                token.ThrowIfCancellationRequested(); // каждый раз true?

                if (ThresholdTop > DefaultThreshold)
                {
                    // уменьшаем порог, но не ниже базового
                    ThresholdTop--;
                }
            }
            //}
            //catch (Exception ex)
            //{
            //    var a = ex.Message;
            //}
        }

        /// <summary>
        /// Ссылка на задачу <see cref="InitialThreshold"/>.
        /// </summary>
        private Task InitialThresholdRef;

        /// <summary>
        /// Источник токена отмены, для задачи <see cref="InitialThreshold"/>.
        /// </summary>
        private CancellationTokenSource AdaptationTokenSource;

        #endregion

        public Modulator():base()
        {

        }

        protected override async Task OnUpdate()
        {
            await Task.Run(async () =>
            {
                try
                {
                    // ожидаем входящие сигналы, иначе говоря не создана задача активации нейроэлемента
                    if (ActivateRef == null)
                    {
                        // преодолен порог активации
                        if (Sum > ThresholdTop + ExtraThreshold)
                        {
                            // если задача выполняется и она не завершена - происходит оценка
                            if (EvaluationRef != null && !EvaluationRef.IsCompleted)
                            {
                                // увеличиваем счетчик повторов
                                RepeatCounter++;
                                EvaluationTokenSource?.Cancel();
                                EvaluationRef = null;
                            }
                            // иначе сбрасываем счетчик повторов
                            else
                            {
                                RepeatCounter = 0;
                            }

                            // если превышено максимально допустимое число повторений
                            if ((RepeatCounter > MaxRepeatCount) && Math.Abs(ExtraThreshold) < 0.001)
                            {
                                // повышаем порог активации
                                ThresholdTop += ThresholdIncreaseStep;
                            }
                            AdaptationTokenSource?.Cancel();

                            // Создаем задачу активации нейроэлемента и после выполнения обнуляем ее, чтобы нейроэлемент снова принимал внешние сигналы
                            ActivateRef = Activate().ContinueWith(task => { ActivateRef = null; });
                            await ActivateRef;
                        }

                        //пороговая функция на нижний порог
                        //else if (Sum < ThresholdDown)
                        //{
                        //    if (Area != null) StartCoroutine("repolarizationTime"); //при достаточном тормозящем воздействии, биологический нейрон усиленно поляризуется
                        //}
                    }

                    #region демпфер сумматора

                    // если величина меньше либо равна величине демпфера - обнуляем
                    if (Math.Abs(Sum) <= SumDamfer) Sum = 0f;
                    // если величина больше величины демпфера - отнимаем величинк демпфера
                    if (Sum > SumDamfer) Sum -= SumDamfer;
                    // если величина меньше величины демпфера - отнимаем величинк демпфера
                    if (Sum < -SumDamfer) Sum += SumDamfer;

                    #endregion
                    
                    #region добавляем к сумматору сигналы, полученные от синапсов прямого действия

                    if (SynapseDirectForce > 0)
                    {
                        // т.к. величина SynapseDirectForce может измениться после увеличения Sum (нейроэлемент получит импульс от синапса), необходимо включить блокировку.
                        // иначе SynapseDirectForce увеличит изменит значение и затем будет обнулен, иначе говоря мы потеряем величину постушившего сигнала.
                        lock (SynapseDirectForceLocker)
                        {
                            Sum += SynapseDirectForce;
                            SynapseDirectForce = 0;
                        }
                    }
                    #endregion

                    #region демпфер дополнительного порога

                    if (Math.Abs(ExtraThreshold) <= DempferBonusThreshold)
                        ExtraThreshold = 0f; // Демпфер дополнительного порога
                    if (ExtraThreshold > DempferBonusThreshold) ExtraThreshold -= DempferBonusThreshold;
                    if (ExtraThreshold < -DempferBonusThreshold) ExtraThreshold += DempferBonusThreshold;
                    #endregion

                    #region добавляем к дополнительному порогу сигналы от синапсов модулирующего действия

                    if (ModulatingSynapseForce > 0)
                    {
                        // т.к. величина ModulatingSynapseForce может измениться после увеличения Sum (нейроэлемент получит импульс от синапса), необходимо включить блокировку.
                        // иначе м увеличит изменит значение и затем будет обнулен, иначе говоря мы потеряем величину постушившего сигнала.
                        lock (ModulatingSynapseForceLocker)
                        {
                            ExtraThreshold += ModulatingSynapseForce;
                            ModulatingSynapseForce = 0;
                        }
                    } 
                    #endregion
                }
                catch (Exception ex)
                {
                    var a = ex.Message;
                }
            });
        }

        protected override void SynapseOnSynapseActivated<T>(object sender, T e)
        {
            // определяем тип синапса, вызвавшего событие и реагируем/не реагируем.

            // синапс прямого действия
            if (sender.GetType() == typeof(SynapseDirect))
            {
                SynapseDirectForce += float.Parse(e.ToString());
            }

            // синапс модулирующего действия
            // сумма основной части порога с дополнительной не должна быть отрицательной - это приводит к спонтанной активации.
            else if (sender.GetType() == typeof(SynapseModulating))
            {
                var force = float.Parse(e.ToString());

                if (ThresholdTop + ExtraThreshold + force < 0)
                {
                    ModulatingSynapseForce = -ThresholdTop;
                }

                else
                {
                    ModulatingSynapseForce += force;
                }
            }

            else
            {
                //throw new NotImplementedException("Не реализовано!");
            }
        }

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
                // создаем задачу выполнения оценки и затем обнуляем ее. 
                EvaluationTokenSource = new CancellationTokenSource();
                EvaluationRef = Task.Run(async () => await Evaluation(EvaluationTokenSource.Token)).ContinueWith(task => { EvaluationRef = null; });

                // создаем задачу адаптации
                AdaptationTokenSource = new CancellationTokenSource();
                InitialThresholdRef = Task.Run(async () => await InitialThreshold(AdaptationTokenSource.Token));
            });
        }
    }
}
