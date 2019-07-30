using Allyn.Cqrs.Commands;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Allyn.Cqrs.Test.Connands
{
    public class CommandBusTest
    {
        [Fact]
        public async Task SendAsync_ArgumentNullExceptionWhichCommand()
        {
            // Arrange
            var bus = new CommandBus();

            // Act
            Task task = bus.SendAsync<ICommand>(null);

            // Assert
           await Assert.ThrowsAsync<ArgumentNullException>(async ()=> await task);
        }
    }
}
