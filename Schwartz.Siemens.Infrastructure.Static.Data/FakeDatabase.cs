﻿using Schwartz.Siemens.Core.ApplicationServices;
using Schwartz.Siemens.Core.DomainServices;
using Schwartz.Siemens.Core.Entities.Rigs;
using Schwartz.Siemens.Core.Entities.UserBase;
using Schwartz.Siemens.Infrastructure.Data;
using System;
using System.Collections.Generic;

namespace Schwartz.Siemens.Infrastructure.Static.Data
{
    public class FakeDatabase : IDatabase
    {
        public FakeDatabase(IAuthenticationHelper authenticationHelper, MaritimeContext context)
        {
            AuthenticationHelper = authenticationHelper;
            Context = context;
        }

        private IAuthenticationHelper AuthenticationHelper { get; }
        private MaritimeContext Context { get; }

        public void Initialize()
        {
            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();

            MockRigs();

            MockUsers();

            Context.SaveChanges();
        }

        private void MockRigs()
        {
            var location1 = Context.Locations.Add(new Location
            {
                Date = DateTime.Now,
                Latitude = 55.5224998,
                Longitude = 8.3934396
            }).Entity;
            var location2 = Context.Locations.Add(new Location
            {
                Date = DateTime.Now.AddHours(12),
                Latitude = 55.5224998,
                Longitude = 8.4934396
            }).Entity;
            var location3 = Context.Locations.Add(new Location
            {
                Date = DateTime.Now.AddHours(36),
                Latitude = 55.5224998,
                Longitude = 8.5934396
            }).Entity;
            var location4 = Context.Locations.Add(new Location
            {
                Date = DateTime.Now.AddHours(24),
                Latitude = 55.5224998,
                Longitude = 8.6934396
            }).Entity;
            Context.Rigs.Add(new Rig()
            {
                Imo = 8765280,
                Name = "MAERSK INSPIRER",
                Locations = new List<Location>
                {
                    location1,
                    location2,
                    location3,
                    location4
                }
            });
        }

        private void MockUsers()
        {
            const string pass = "1234";
            AuthenticationHelper.CreatePasswordHash(pass, out var hash, out var salt);

            Context.Users.AddRange(
                new User { Email = "admin@mail.com", Username = "Admin", PasswordHash = hash, PasswordSalt = salt, IsAdmin = true },
                new User { Email = "user@mail.com", Username = "User", PasswordHash = hash, PasswordSalt = salt, IsAdmin = true }
                );
        }
    }
}