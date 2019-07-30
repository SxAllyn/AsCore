using System;

namespace Allyn.Cqrs.Commands
{
    /// <summary>
    /// A base class for all cqrs commands.
    /// </summary>
    [Serializable]
    public class Command : ICommand
    {
        /// <summary>
        /// Get command identification.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Initialize the <see cref="Command"/> base.
        /// </summary>
        /// <param name="id"><see cref="Guid"/> value for command identification.</param>
        public Command(Guid id) => Id = id;
    }
}
