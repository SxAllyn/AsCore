using System;
using System.Collections.Generic;
using System.Text;

namespace Allyn.Cqrs.Events
{
    /// <summary>
    ///  A base class for all cqrs events.
    /// </summary>
    [Serializable]
    public class Event : IEvent
    {
        /// <summary>
        /// Get event identification.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Get event version.
        /// </summary>
        public int Version { get; }

        /// <summary>
        /// Initialize the <see cref="Event"/> base.
        /// </summary>
        /// <param name="id"><see cref="Guid"/> value for event identification.</param>
        /// <param name="version"><see cref="int"/> value for event version.</param>
        public Event(Guid id, int version) {
            Id = id;
            Version = version;
        }
    }
}
