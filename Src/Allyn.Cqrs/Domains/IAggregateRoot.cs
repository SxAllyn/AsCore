using System;
using System.Collections.Generic;
using System.Text;

namespace Allyn.Cqrs.Domains
{
    /// <summary>
    /// This is the base type of aggregate root.
    /// </summary>
    public interface IAggregateRoot
    {
        /// <summary>
        /// Get the aggregate root Identification.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Get the aggregate root version.
        /// </summary>
        int Version { get; }
    }
}
