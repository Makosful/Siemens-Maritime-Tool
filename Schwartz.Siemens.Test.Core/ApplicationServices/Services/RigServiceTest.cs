using Moq;
using Schwartz.Siemens.Core.ApplicationServices;
using Schwartz.Siemens.Core.ApplicationServices.Services;
using Schwartz.Siemens.Core.DomainServices.Repositories;
using Schwartz.Siemens.Core.Entities.Rigs;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Schwartz.Siemens.Test.Core.ApplicationServices.Services
{
    public class RigServiceTest
    {
        private static Mock<IRigRepository> CreateMoqRepository()
        {
            var repository = new Mock<IRigRepository>();

            // Setup create
            repository.Setup(rigRepository => rigRepository.Create(It.IsAny<Rig>()))
                .Returns((Rig rig) => rig);

            // Setup ReadAll
            repository.Setup(rigRepository => rigRepository.ReadAll())
                .Returns(MockRigs);

            // Setup Read single
            for (var i = 0; i < 3; i++)
            {
                var i1 = i;
                repository.Setup(rigRepository =>
                        rigRepository.Read(i1 + 1))
                    .Returns(MockRigs().ToList()[i]);
            }

            return repository;
        }

        private static IEnumerable<Rig> MockRigs()
        {
            var rigs = new List<Rig>
            {
                new Rig {Imo = 1, Name = "Number One"},
                new Rig {Imo = 2, Name = "Number Two"},
                new Rig {Imo = 3, Name = "Number Three"}
            };

            return rigs;
        }

        [Fact]
        public void RigService_Create_UnsatId_ExpectsException()
        {
            var repository = CreateMoqRepository();
            IRigService service = new RigService(repository.Object);

            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                service.Create(new Rig()));

            Assert.NotNull(exception);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void RigService_Create_ExistingRigId_ExpectsException(int id)
        {
            var repository = CreateMoqRepository();
            IRigService service = new RigService(repository.Object);

            var exception = Assert.Throws<ArgumentException>(() =>
                service.Create(new Rig() { Imo = id }));

            Assert.NotNull(exception);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-5)]
        [InlineData(-10)]
        public void RigService_Create_NegativeId_ExpectsException(int id)
        {
            var repository = CreateMoqRepository();
            IRigService service = new RigService(repository.Object);

            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                service.Create(new Rig { Imo = id }));

            Assert.NotNull(exception);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(15)]
        public void RigService_Create_ValidRigParam_ExpectsSameRigReturned(int id)
        {
            var repository = CreateMoqRepository();
            IRigService service = new RigService(repository.Object);

            var expectedRig = new Rig()
            {
                Imo = id,
                Location = new List<Location>(),
                Name = $"Number {id}"
            };

            var actualRig = service.Create(expectedRig);

            repository.Verify(
                r => r.Create(It.IsAny<Rig>()),
                Times.AtLeastOnce);
            Assert.NotNull(actualRig);
            Assert.Equal(expectedRig.Imo, actualRig.Imo);
        }
    }
}