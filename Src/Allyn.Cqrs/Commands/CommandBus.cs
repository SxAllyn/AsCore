using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Allyn.Cqrs.Commands
{
    /// <summary>
    /// A default command bus implementation of <see cref="ICommandBus"/>.
    /// </summary>
    public class CommandBus : ICommandBus
    {

        public async Task SendAsync<T>(T command) where T : class, ICommand
        {
            if (command == null) { throw new ArgumentNullException(nameof(command)); }

            await Task.CompletedTask;

        }
    }
}
