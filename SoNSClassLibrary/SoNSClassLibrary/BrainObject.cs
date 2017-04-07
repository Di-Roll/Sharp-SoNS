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
    /// Представляет собой абстрактный класс, который подписан на событие глобального цикла.
    /// </summary>
    public abstract class BrainObject
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="BrainObject"/>.
        /// </summary>
        protected BrainObject()
        {
            UpdateLoop.OnUpdate += async () => await OnUpdate();
        }

        protected abstract Task OnUpdate();
    }
}
