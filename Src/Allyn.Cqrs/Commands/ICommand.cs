using System;
using System.Collections.Generic;
using System.Text;

namespace Allyn.Cqrs.Commands
{
    /// <summary>
    /// A marker interface for base which define a command.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Get command identification.
        /// </summary>
        Guid Id { get; }
    }
}
