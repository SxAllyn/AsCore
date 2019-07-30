using System;
using System.Collections.Generic;
using System.Text;

namespace Allyn.Cqrs.Events
{
    /// <summary>
    /// A marker interface for base which define a event.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// Get event identification.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Get event version.
        /// </summary>
        int Version { get; }
    }
}
