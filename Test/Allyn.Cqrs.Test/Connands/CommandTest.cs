using Allyn.Cqrs.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Allyn.Cqrs.Test.Connands
{
    public class CommandTest
    {
        [Fact]
        public void GetId() {
            //Arrange
            Guid id = Guid.NewGuid();
            var instance = new Command(id);
       
            //Atc
            var result = instance.Id;

            //Assert
            Assert.Equal(result, id);
        }
    }
}
