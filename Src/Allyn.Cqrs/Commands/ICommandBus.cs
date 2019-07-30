using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Allyn.Cqrs.Commands
{
    /// <summary>
    /// A marker interface for base which define a command bus.
    /// </summary>
    public interface ICommandBus
    {
        /// <summary>
        /// Send a command to handler.
        /// </summary>
        /// <typeparam name="T">Types derived from <see cref="ICommand"/>.</typeparam>
        /// <param name="command">A command instance to send.</param>
        /// <returns>A <see cref="Task"/></returns>
        Task SendAsync<T>(T command) where T : class, ICommand;
    }
}
