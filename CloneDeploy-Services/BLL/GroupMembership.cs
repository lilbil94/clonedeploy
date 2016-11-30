﻿using System.Collections.Generic;
using System.Linq;
using CloneDeploy_DataModel;
using CloneDeploy_Entities;
using CloneDeploy_Entities.DTOs;

namespace CloneDeploy_App.BLL
{
    public class GroupMembership
    {
        public static bool AddMembership(List<GroupMembershipEntity> groupMemberships)
        {
            var group = new GroupEntity();
            var result = false;
            using (var uow = new UnitOfWork())
            {
                foreach (var membership in groupMemberships.Where(membership => !uow.GroupMembershipRepository.Exists(
                    g => g.ComputerId == membership.ComputerId && g.GroupId == membership.GroupId)))
                {
                    uow.GroupMembershipRepository.Insert(membership);
                    group = BLL.Group.GetGroup(membership.GroupId);
                }
                uow.Save();
                result = true;
            }

            if (group.SetDefaultProperties == 1)
            {
                var groupProperty = BLL.GroupProperty.GetGroupProperty(group.Id);
                BLL.GroupProperty.UpdateComputerProperties(groupProperty);
            }

            if (group.SetDefaultBootMenu == 1)
            {
                var groupBootMenu = BLL.GroupBootMenu.GetGroupBootMenu(group.Id);
                BLL.GroupBootMenu.UpdateGroupMemberBootMenus(groupBootMenu);
            }

            return result;
        }

        public static ApiDTO GetGroupMemberCount(int groupId)
        {
            using (var uow = new UnitOfWork())
            {
                return new ApiDTO(){Value=uow.GroupMembershipRepository.Count(g => g.GroupId == groupId)};
            }
        }

        public static bool DeleteAllMembershipsForGroup(int groupId)
        {
            using (var uow = new UnitOfWork())
            {
                uow.GroupMembershipRepository.DeleteRange(x => x.GroupId == groupId);
                uow.Save();
                return true;
            }
        }


        public static bool DeleteMembership(int computerId, int groupId)
        {
            using (var uow = new UnitOfWork())
            {
                uow.GroupMembershipRepository.DeleteRange(
                    g => g.ComputerId == computerId && g.GroupId == groupId);
                uow.Save();
                return true;
            }
        }

        public static bool DeleteComputerMemberships(int computerId)
        {
            using (var uow = new UnitOfWork())
            {
                uow.GroupMembershipRepository.DeleteRange(x => x.ComputerId == computerId);
                uow.Save();
                return true;
            }
        }

        public static List<GroupMembershipEntity> GetAllComputerMemberships(int computerId)
        {
            using (var uow = new UnitOfWork())
            {
                return uow.GroupMembershipRepository.Get(x => x.ComputerId == computerId);
            }
        }

      
    }
}