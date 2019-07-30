using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Allyn.Cqrs.Commands;
using Allyn.Cqrs.Events;

namespace Allyn.Cqrs.Domains
{
    /// <summary>
    /// Base types of all aggregate roots.
    /// </summary>
    public abstract class AggregateRoot : IAggregateRoot
    {
        /// <summary>
        /// Get the uncommitted events;
        /// </summary>
        public Queue<IEvent> UnCommittedEvents { get; } = new Queue<IEvent>();

        /// <summary>
        /// Get or set the aggregate root Identification.
        /// </summary>
        public Guid Id { get; protected set; }

        /// <summary>
        ///  Get or set the aggregate root version.
        /// </summary>
        public int Version { get; protected set; }

        /// <summary>
        /// All events handler
        /// </summary>
        /// <param name="evnt">Events to be handled</param>
        /// <remarks>
        /// Matching specific event processing at implementation time
        /// </remarks>
        protected abstract void Handler(IEvent evnt);

        /// <summary>
        /// Apply a event.
        /// </summary>
        /// <param name="evnt">A domanin evnt.</param>
        public void ApplyEvent(IEvent evnt)
        {
            if (evnt == null) { throw new ArgumentNullException(nameof(evnt)); }
            Handler(evnt);
            UnCommittedEvents.Enqueue(evnt);
        }
    }
}
