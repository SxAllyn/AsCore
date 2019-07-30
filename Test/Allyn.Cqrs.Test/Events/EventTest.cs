using Allyn.Cqrs.Events;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Allyn.Cqrs.Test.Events
{
    public class EventTest
    {
        [Fact]
        public void GetId()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            var instance = new Event(id,0);

            //Atc
            var result = instance.Id;

            //Assert
            Assert.Equal(result, id);
        }

        [Fact]
        public void GetVersion()
        {
            //Arrange
            int version = 6;
            var instance = new Event(Guid.NewGuid(), version);

            //Atc
            var result = instance.Version;

            //Assert
            Assert.Equal(result, version);
        }
    }
}
