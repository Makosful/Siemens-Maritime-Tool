using Moq;
using Schwartz.Siemens.Core.ApplicationServices;
using Schwartz.Siemens.Core.ApplicationServices.Services;
using Schwartz.Siemens.Core.DomainServices;
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

        private static Mock<IRigRepository> CreateMoqRigRepository()
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

            // Setup UpdatePosition
            repository.Setup(rigRepository =>
                    rigRepository.UpdateLocation(It.IsAny<int>()))
                .Returns(() => new Location());

            return repository;
        }

        private static Mock<ILocationRepository> CreateMoqLocationRepository()
        {
            var locationRepository = new Mock<ILocationRepository>();

            locationRepository.Setup(repository => repository.Create(It.IsAny<Location>()))
                .Returns((Location loc) => loc);

            return locationRepository;
        }

        private static Mock<IWebSpider> CreateMoqSpider()
        {
            var spider = new Mock<IWebSpider>();

            spider.Setup(webSpider => webSpider.GetLatestLocation(It.IsAny<int>()))
                .Returns(() => new Location());

            spider.Setup(webSpider => webSpider.GetMultipleLocations(It.IsAny<List<int>>()))
                .Returns(() => new List<Location>
                {
                    new Location{Id = 1,},
                    new Location{Id = 2,},
                    new Location{Id = 3,},
                });

            spider.Setup(webSpider => webSpider.GetRig(It.IsAny<int>()))
                .Returns((int imo) => new Rig() { Imo = imo });

            return spider;
        }

        private static IEnumerable<Rig> MockRigs()
        {
            var rigs = new List<Rig>
            {
                new Rig
                {
                    Imo = 1,
                    Name = "Number One",
                    Locations = new List<Location>
                    {
                        new Location {Id = 1, Date = new DateTime(2018, 12, 5)},
                        new Location {Id = 2, Date = new DateTime(2018, 12, 3)},
                        new Location {Id = 3, Date = new DateTime(2018, 12, 1)},
                    }
                },
                new Rig
                {
                    Imo = 2,
                    Name = "Number Two",
                    Locations = new List<Location>
                    {
                        new Location {Id = 4, Date = new DateTime(2018, 12, 1)},
                        new Location {Id = 5, Date = new DateTime(2018, 12, 3)},
                        new Location {Id = 6, Date = new DateTime(2018, 12, 5)},
                    }
                },
                new Rig
                {
                    Imo = 3,
                    Name = "Number Three",
                    Locations = new List<Location>
                    {
                        new Location {Id = 7, Date = new DateTime(2018, 12, 1)},
                        new Location {Id = 8, Date = new DateTime(2018, 12, 3)},
                        new Location {Id = 9, Date = new DateTime(2018, 12, 2)},
                    }
                }
            };

            return rigs;
        }

        private static IRigService CreateService(
            out Mock<IRigRepository> rigRepository,
            out Mock<ILocationRepository> locationRepository,
            out Mock<IWebSpider> spider)
        {
            rigRepository = CreateMoqRigRepository();
            locationRepository = CreateMoqLocationRepository();
            spider = CreateMoqSpider();

            return new RigService(
                rigRepository.Object,
                locationRepository.Object,
                spider.Object);
        }

        #endregion Setup

        #region Create

        [Fact]
        public void RigService_Create_UnsatImo_ExpectsException()
        {
            var service = CreateService(out _, out _, out _);

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
            var service = CreateService(out _, out _, out _);

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
            var service = CreateService(out _, out _, out _);

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
            var service = CreateService(out var rigRepository, out _, out _);

            var expectedRig = new Rig()
            {
                Imo = imo
            };

            var actualRig = service.Create(expectedRig);

            rigRepository.Verify(
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
            var service = CreateService(out _, out _, out _);

            var rig = service.Read(imo);

            Assert.Null(rig);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void RigService_Read_ValidImo_ExpectsNotNull(int imo)
        {
            var service = CreateService(out _, out _, out _);

            var rig = service.Read(imo);

            Assert.NotNull(rig);
        }

        [Fact]
        public void RigService_Read_OrderPositions_ExpectsDescendingDate()
        {
            var service = CreateService(out _, out var locationRepository, out _);

            var rig1 = service.Read(1);
            Assert.NotNull(rig1);
            Assert.Equal(1, rig1.Locations[0].Id);
            Assert.Equal(2, rig1.Locations[1].Id);
            Assert.Equal(3, rig1.Locations[2].Id);

            var rig2 = service.Read(2);
            Assert.NotNull(rig2);
            Assert.Equal(6, rig2.Locations[0].Id);
            Assert.Equal(5, rig2.Locations[1].Id);
            Assert.Equal(4, rig2.Locations[2].Id);

            var rig3 = service.Read(3);
            Assert.NotNull(rig3);
            Assert.Equal(8, rig3.Locations[0].Id);
            Assert.Equal(9, rig3.Locations[1].Id);
            Assert.Equal(7, rig3.Locations[2].Id);
        }

        [Theory]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        public void RigService_UpdateLocation_InvalidValues_ExpectsNull(int imo)
        {
            var service = CreateService(out _, out _, out _);

            var location = service.UpdateLocation(imo);

            Assert.Null(location);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void RigService_UpdateLocation_ValidValues_ExpectsLocation(int imo)
        {
            var service = CreateService(out _, out _, out _);

            var location = service.UpdateLocation(imo);

            Assert.NotNull(location);
        }

        #endregion Read

        #region Update

        [Theory]
        [InlineData(0)]  // Invalid by default
        [InlineData(-1)] // IMO out of range
        public void RigService_Update_InvalidId_ExpectsException(int imo)
        {
            var service = CreateService(out _, out _, out _);

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
            var service = CreateService(out _, out _, out _);

            Assert.Throws<KeyNotFoundException>(() =>
                service.Update(imo, new Rig()));
        }

        [Fact] // The Rig entity argument holds the updated information. It cannot be null
        public void RigService_Update_RigArgumentNull_ExpectsException()
        {
            var service = CreateService(out _, out _, out _);

            Assert.Throws<ArgumentNullException>(() => service.Update(1, null));
        }

        [Fact]
        public void RigService_Update_ValidParams_ExpectsUpdatedRigReturned()
        {
            var service = CreateService(out var rigRepository, out _, out _);

            var update = service.Update(1, new Rig() { Name = "Test Rig 101" });

            Assert.NotNull(update);
            Assert.Equal(1, update.Imo);

            rigRepository.Verify(repo =>
                    repo.Update(It.IsAny<int>(), It.IsAny<Rig>()),
                Times.AtLeastOnce);
        }

        #endregion Update

        #region Delete

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-5)] // 0 and negative IMOs are invalid
        public void RigService_Delete_InvalidImo_ExpectsException(int imo)
        {
            var service = CreateService(out _, out _, out _);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                service.Delete(imo));
        }

        [Theory]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(10)] // These IMO are out of range
        public void RigService_Delete_ImoNotFound_ExpectsException(int imo)
        {
            var service = CreateService(out _, out _, out _);

            Assert.Throws<KeyNotFoundException>(() =>
                service.Delete(imo));
        }

        #endregion Delete
    }
}