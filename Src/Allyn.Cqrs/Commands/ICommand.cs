using System;
using System.Collections.Generic;
using System.Text;

namespace Allyn.Cqrs.Commands
{
    /// <summary>
    /// Represents the "command" type.
    /// </summary>
    public interface ICommand
    {
        Guid Id { get; }
    }
}
