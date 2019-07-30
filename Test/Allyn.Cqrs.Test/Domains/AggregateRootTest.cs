using Allyn.Cqrs.Domains;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using Allyn.Cqrs.Commands;
using System.Threading.Tasks;
using Allyn.Cqrs.Events;

namespace Allyn.Cqrs.Test.Domains
{
    public class AggregateRootTest
    {
        [Fact]
        public void ApplyEvent_ArgumentNullExceptionWhichCommand() {
            // Arrange
            var instance = new Mock<AggregateRoot>().Object;

            // Atc & Assert
            Assert.Throws<ArgumentNullException>(() => instance.ApplyEvent(null));
        }

        [Fact]
        public void ApplyEvent_DeriveClassCreate()
        {
            //Arrange
            int version = 0;
            string name = "Tester";
            string remarks = "Test remarks ...";

            // Act
            var result = new Tester(name,remarks);

            // Assert
            Assert.NotEqual(result.Id, Guid.Empty);
            Assert.Equal(result.Version, version);
            Assert.Equal(result.Name, name);
            Assert.Equal(result.Remarks, remarks);
        }

        [Fact]
        public void ApplyEvent_DeriveClassUpdate()
        {
            //Arrange
            var instance = new Tester("Allyn", "Not ohter remark ...");

            Guid id = instance.Id;
            int version = instance.Version;
            string name = "Tester";
            string remarks = "Test remarks ...";

            // Act
            instance.Update(name, remarks);
            
            // Assert
            Assert.Equal(instance.Id, id);
            Assert.Equal(instance.Version, version + 1);
            Assert.Equal(instance.Name, name);
            Assert.Equal(instance.Remarks, remarks);
        }

        public class Tester : AggregateRoot
        {
            public string Name { get; set; }
            public string Remarks { get; set; }

            public Tester(string name, string remarks) 
                => ApplyEvent(new TesterCreateEvnt(Guid.NewGuid(), 0, name, remarks));

            public void Update(string name, string remarks) 
                => ApplyEvent(new TesterUpdateEvnt(Id, Version + 1, name, remarks));

            //invoke specified event handler
            protected override void Handler(IEvent evnt)
                => Handler(evnt as dynamic);

            private void Handler(TesterCreateEvnt evnt)
            {
                Id = evnt.Id;
                Version = evnt.Version;
                Name = evnt.Name;
                Remarks = evnt.Remarks;
            }

            private void Handler(TesterUpdateEvnt evnt)
            {
                Version = evnt.Version;
                Name = evnt.Name;
                Remarks = evnt.Remarks;
            }
        }

        public class TesterCreateEvnt : Event
        {
            public string Name { get; set; }
            public string Remarks { get; set; }
            public TesterCreateEvnt(Guid id, int version, string name, string remarks) : base(id,version)
            {
                Name = name;
                Remarks = remarks;
            }
        }

        public class TesterUpdateEvnt : Event
        {
            public string Name { get; set; }
            public string Remarks { get; set; }
            public TesterUpdateEvnt(Guid id, int version, string name, string remarks) : base(id,version)
            {
                Name = name;
                Remarks = remarks;
            }
        }
    }
}
