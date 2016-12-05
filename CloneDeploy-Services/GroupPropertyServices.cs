﻿using System;
using CloneDeploy_DataModel;
using CloneDeploy_Entities;
using CloneDeploy_Entities.DTOs;

namespace CloneDeploy_Services
{
    public class GroupPropertyServices
    {
        private readonly UnitOfWork _uow;

        public GroupPropertyServices()
        {
            _uow = new UnitOfWork();
        }

        public ActionResultDTO AddGroupProperty(GroupPropertyEntity groupProperty)
        {

            _uow.GroupPropertyRepository.Insert(groupProperty);
            _uow.Save();
            var actionResult = new ActionResultDTO();
            actionResult.Success = true;
            actionResult.Id = groupProperty.Id;

            UpdateComputerProperties(groupProperty);

            return actionResult;
        }

       

        public ActionResultDTO UpdateGroupProperty(GroupPropertyEntity groupProperty)
        {

            _uow.GroupPropertyRepository.Update(groupProperty, groupProperty.Id);
            _uow.Save();
            var actionResult = new ActionResultDTO();
            actionResult.Success = true;
            actionResult.Id = groupProperty.Id;
            UpdateComputerProperties(groupProperty);
            return actionResult;
            
        }

        public  void UpdateComputerProperties(GroupPropertyEntity groupProperty)
        {
            foreach (var computer in new GroupServices().GetGroupMembers(groupProperty.GroupId))
            {
                if (Convert.ToBoolean(groupProperty.ImageEnabled))
                    computer.ImageId = groupProperty.ImageId;
                if (Convert.ToBoolean(groupProperty.ImageProfileEnabled))
                    computer.ImageProfileId = groupProperty.ImageProfileId;
                if (Convert.ToBoolean(groupProperty.DescriptionEnabled))
                    computer.Description = groupProperty.Description;
                if (Convert.ToBoolean(groupProperty.SiteEnabled))
                    computer.SiteId = groupProperty.SiteId;
                if (Convert.ToBoolean(groupProperty.BuildingEnabled))
                    computer.BuildingId = groupProperty.BuildingId;
                if (Convert.ToBoolean(groupProperty.RoomEnabled))
                    computer.RoomId = groupProperty.RoomId;
                if (Convert.ToBoolean(groupProperty.CustomAttribute1Enabled))
                    computer.CustomAttribute1 = groupProperty.CustomAttribute1;
                if (Convert.ToBoolean(groupProperty.CustomAttribute2Enabled))
                    computer.CustomAttribute2 = groupProperty.CustomAttribute2;
                if (Convert.ToBoolean(groupProperty.CustomAttribute3Enabled))
                    computer.CustomAttribute3 = groupProperty.CustomAttribute3;
                if (Convert.ToBoolean(groupProperty.CustomAttribute4Enabled))
                    computer.CustomAttribute4 = groupProperty.CustomAttribute4;
                if (Convert.ToBoolean(groupProperty.CustomAttribute5Enabled))
                    computer.CustomAttribute5 = groupProperty.CustomAttribute5;
                if (Convert.ToBoolean(groupProperty.ProxyEnabledEnabled))
                    computer.ProxyReservation = groupProperty.ProxyEnabled;

                var computerServices = new ComputerServices();
                computerServices.UpdateComputer(computer);
                if (Convert.ToBoolean(groupProperty.TftpServerEnabled) || Convert.ToBoolean(groupProperty.BootFileEnabled))
                {
                    var proxyServices = new ComputerProxyReservationServices();
                    var computerProxy = computerServices.GetComputerProxyReservation(computer.Id);
                    if (Convert.ToBoolean(groupProperty.TftpServerEnabled))
                        computerProxy.NextServer = groupProperty.TftpServer;
                    if (Convert.ToBoolean(groupProperty.BootFileEnabled))
                        computerProxy.BootFile = groupProperty.BootFile;
                    proxyServices.UpdateComputerProxyReservation(computerProxy);
                }
            }
        }

      
       

      
    }
}