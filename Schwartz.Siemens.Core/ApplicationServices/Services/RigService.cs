﻿using Schwartz.Siemens.Core.DomainServices;
using Schwartz.Siemens.Core.DomainServices.Repositories;
using Schwartz.Siemens.Core.Entities.Rigs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Schwartz.Siemens.Core.ApplicationServices.Services
{
    public class RigService : IRigService
    {
        public RigService(IRigRepository rigRepository,
            IWebSpider spider)
        {
            RigRepository = rigRepository;
            Spider = spider;
        }

        private IRigRepository RigRepository { get; }
        private IWebSpider Spider { get; }

        /// <summary>
        /// Creates a new Rig entity.
        /// This method will automatically fetch the correct data and fill in the entity
        /// </summary>
        /// <param name="item">Only Rig.Imo is considered. Everything else will be overwritten</param>
        /// <returns>A new instance of the Rig entity with all the data filled in</returns>
        public Rig Create(Rig item)
        {
            if (item.Imo == 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(item.Imo),
                    "Rigs can't have an IMO of 0. This value should match the vessel's International Maritime Organization number");
            }

            if (item.Imo < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(item.Imo),
                    "A Rig's IMO can't be below 1. this value should match the vessel's International Maritime Organization number");
            }

            if (Read(item.Imo) != null)
            {
                throw new ArgumentException(
                    "A Rig with the same IMO has already been found in the database.",
                    nameof(item.Imo));
            }

            //var rig = Spider.GetRig(item.Imo);

            return RigRepository.Create(item);
        }

        public Rig Delete(int id)
        {
            if (id < 1) throw new ArgumentOutOfRangeException(nameof(id), "IMO at 0 and below are invalid and can't be created, therefor, not deleted");

            var rig = Read(id);
            if (rig == null) throw new KeyNotFoundException($"No Rig was found with the given IMO: {id}. The rig wasn't deleted");

            return RigRepository.Delete(rig);
        }

        public Rig Read(int id)
        {
            var rig = RigRepository.Read(id);

            return rig == null ? null : ManageLocation(rig);
        }

        public List<Rig> ReadAll()
        {
            var rigs = RigRepository.ReadAll().ToList();

            for (var i = 0; i < rigs.Count; i++)
            {
                rigs[i] = ManageLocation(rigs[i]);
            }

            return rigs;
        }

        public Rig Update(int imo, Rig item)
        {
            if (imo < 1) throw new ArgumentOutOfRangeException(nameof(imo), "The given IMO cannot be 0 or below. Make sure the rig exists and you have the right IMO");

            if (item == null) throw new ArgumentNullException(nameof(item), "The Rig entity passed in as an argument is null.");

            item.Imo = imo;
            return RigRepository.Update(item);
        }

        /// <summary>
        /// Fetches the latest Location for the Rig with the given IMO
        /// </summary>
        /// <returns></returns>
        public Rig UpdateLocation(Rig rig)
        {
            //var location = Spider.GetLatestLocation(rig.Imo);
            //rig.Locations.Add(location);
            return Update(rig.Imo, rig);
        }

        private Rig ManageLocation(Rig rig)
        {
            if (rig.Locations.Count > 1)
            {
                rig.Locations = rig.Locations.OrderByDescending(l => l.Date).ToList();
                if (DateTime.Now.Subtract(rig.Locations[0].Date).TotalHours > 12)
                {
                    rig = UpdateLocation(rig);
                    rig.Outdated = true;
                }
                else
                {
                    rig.Outdated = false;
                }
            }

            return rig;
        }
    }
}