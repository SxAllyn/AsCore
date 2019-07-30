using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Allyn.Cqrs.Commands
{
    /// <summary>
    /// A marker interface for base which define a command handler.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Execution of specific handler from specified commands.
        /// </summary>
        /// <typeparam name="T">Types derived from <see cref="ICommand"/>.</typeparam>
        /// <param name="command">A command instance to execute.</param>
        /// <returns>A <see cref="Task"/></returns>
        Task ExecuteAsync<T>(T command) where T : class, ICommand;
    }
}
