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
        #region Setup

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

            // Setup Update
            repository.Setup(rigRepository =>
                    rigRepository.Update(It.IsAny<int>(), It.IsAny<Rig>()))
                .Returns((int id, Rig rig) =>
                {
                    rig.Imo = id;
                    return rig;
                });

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

        #endregion Setup

        #region Create

        [Fact]
        public void RigService_Create_UnsatImo_ExpectsException()
        {
            var repository = CreateMoqRepository();
            IRigService service = new RigService(repository.Object);

            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                service.Create(new Rig()));

            Assert.NotNull(exception);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)] // Rigs with these IMO have already been registered in the Moq repository
        [InlineData(3)] // Duplicates are not allowed
        public void RigService_Create_ExistingRigImo_ExpectsException(int imo)
        {
            var repository = CreateMoqRepository();
            IRigService service = new RigService(repository.Object);

            var exception = Assert.Throws<ArgumentException>(() =>
                service.Create(new Rig() { Imo = imo }));

            Assert.NotNull(exception);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-5)]
        [InlineData(-10)] // Rig IMO can't be negative
        public void RigService_Create_NegativeImo_ExpectsException(int imo)
        {
            var repository = CreateMoqRepository();
            IRigService service = new RigService(repository.Object);

            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                service.Create(new Rig { Imo = imo }));

            Assert.NotNull(exception);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(15)] // IMO doesn't need to be in sequence
        public void RigService_Create_ValidRigParam_ExpectsSameRigReturned(int imo)
        {
            var repository = CreateMoqRepository();
            IRigService service = new RigService(repository.Object);

            var expectedRig = new Rig()
            {
                Imo = imo,
                Location = new List<Location>(),
                Name = $"Number {imo}"
            };

            var actualRig = service.Create(expectedRig);

            repository.Verify(
                r => r.Create(It.IsAny<Rig>()),
                Times.AtLeastOnce);
            Assert.NotNull(actualRig);
            Assert.Equal(expectedRig.Imo, actualRig.Imo);
        }

        #endregion Create

        #region Read

        [Theory]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(10)]
        public void RigService_Read_ImoNotFound_ExpectsNull(int imo)
        {
            var repository = CreateMoqRepository();
            IRigService service = new RigService(repository.Object);

            var rig = service.Read(imo);

            Assert.Null(rig);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void RigService_Read_ValidImo_ExpectsNotNull(int imo)
        {
            var repository = CreateMoqRepository();
            IRigService service = new RigService(repository.Object);

            var rig = service.Read(imo);

            Assert.NotNull(rig);
        }

        #endregion Read

        #region Update

        [Theory]
        [InlineData(0)]  // Invalid by default
        [InlineData(-1)] // IMO out of range
        public void RigService_Update_InvalidId_ExpectsException(int imo)
        {
            var repository = CreateMoqRepository();
            IRigService service = new RigService(repository.Object);

            var rig = new Rig();

            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                service.Update(imo, rig));

            Assert.NotNull(exception);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(15)] // These IMO are not registered in the Moq repository
        public void RigService_Update_ImoNotFound_ExpectsException(int imo)
        {
            var repository = CreateMoqRepository();
            IRigService service = new RigService(repository.Object);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                service.Update(imo, new Rig()));
        }

        [Fact] // The Rig entity argument holds the updated information. It cannot be null
        public void RigService_Update_RigArgumentNull_ExpectsException()
        {
            var repository = CreateMoqRepository();
            IRigService service = new RigService(repository.Object);

            Assert.Throws<ArgumentNullException>(() => service.Update(1, null));
        }

        [Fact]
        public void RigService_Update_ValidParams_ExpectsUpdatedRigReturned()
        {
            var repository = CreateMoqRepository();
            IRigService service = new RigService(repository.Object);

            var update = service.Update(1, new Rig() { Name = "Test Rig 101" });

            Assert.NotNull(update);
            Assert.Equal(1, update.Imo);

            repository.Verify(rigRepository =>
                    rigRepository.Update(It.IsAny<int>(), It.IsAny<Rig>()),
                Times.AtLeastOnce);
        }

        #endregion Update
    }
}