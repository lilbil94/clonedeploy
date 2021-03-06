﻿using System;
using System.Collections.Generic;
using System.Linq;
using CloneDeploy_DataModel;
using CloneDeploy_Entities;
using CloneDeploy_Entities.DTOs;
using CloneDeploy_Services.Helpers;

namespace CloneDeploy_Services
{
    public class ImageProfileServices
    {
        private readonly ImageServices _imageServices;
        private readonly UnitOfWork _uow;

        public ImageProfileServices()
        {
            _uow = new UnitOfWork();
            _imageServices = new ImageServices();
        }

        public ActionResultDTO AddProfile(ImageProfileEntity profile)
        {
            var validationResult = ValidateImageProfile(profile, true);
            var actionResult = new ActionResultDTO();
            if (!string.IsNullOrEmpty(profile.MunkiAuthPassword))
                profile.MunkiAuthPassword = new EncryptionServices().EncryptText(profile.MunkiAuthPassword);
            if (validationResult.Success)
            {
                _uow.ImageProfileRepository.Insert(profile);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = profile.Id;
            }

            return actionResult;
        }

        public void CloneProfile(int imageProfileId)
        {
            var imageProfile = _uow.ImageProfileRepository.GetById(imageProfileId);
            var originalName = imageProfile.Name;
            using (var uow = new UnitOfWork())
            {
                for (var c = 1; c <= 100; c++)
                {
                    var newProfileName = imageProfile.Name + "_" + c;
                    if (uow.ImageProfileRepository.Exists(h => h.Name == newProfileName))
                        continue;

                    var clonedProfile = imageProfile;
                    clonedProfile.Name = newProfileName;
                    clonedProfile.Description = "Cloned From " + originalName;
                    AddProfile(clonedProfile);
                    break;
                }
            }
        }

        public ActionResultDTO DeleteImageProfileFileFolders(int profileId)
        {
            _uow.ImageProfileFileFolderRepository.DeleteRange(x => x.ProfileId == profileId);
            _uow.Save();
            var actionResult = new ActionResultDTO();
            actionResult.Success = true;
            actionResult.Id = profileId;
            return actionResult;
        }

        public ActionResultDTO DeleteImageProfileScripts(int profileId)
        {
            _uow.ImageProfileScriptRepository.DeleteRange(x => x.ProfileId == profileId);
            _uow.Save();
            var actionResult = new ActionResultDTO();
            actionResult.Success = true;
            actionResult.Id = profileId;
            return actionResult;
        }

        public bool DeleteImageProfileSysprepTags(int profileId)
        {
            _uow.ImageProfileSysprepRepository.DeleteRange(x => x.ProfileId == profileId);
            _uow.Save();
            return true;
        }

        public ActionResultDTO DeleteProfile(int profileId)
        {
            var profile = ReadProfile(profileId);
            if (profile == null) return new ActionResultDTO {ErrorMessage = "Image Profile Was Not Found", Id = 0};
            _uow.ImageProfileRepository.Delete(profileId);

            _uow.Save();
            var actionResult = new ActionResultDTO();
            actionResult.Success = true;
            actionResult.Id = profile.Id;

            return actionResult;
        }

        public List<ImageProfileWithImage> GetAllProfiles()
        {
            return _uow.ImageProfileRepository.GetImageProfilesWithImages();
        }

        public string MinimumClientSizeForGridView(int profileId, int hdNumber)
        {
            try
            {
                var profile = ReadProfile(profileId);
                var fltClientSize = new ClientPartitionHelper(profile).HardDrive(hdNumber, 1)/1024f/1024f/1024f;
                return Math.Abs(fltClientSize) < 0.1f ? "< 100M" : fltClientSize.ToString("#.##") + " GB";
            }
            catch
            {
                return "N/A";
            }
        }

        public ImageProfileWithImage ReadProfile(int profileId)
        {
            return _uow.ImageProfileRepository.GetImageProfileWithImage(profileId);
        }



        public List<ImageProfileFileFolderEntity> SearchImageProfileFileFolders(int profileId)
        {
            return _uow.ImageProfileFileFolderRepository.Get(x => x.ProfileId == profileId,
                q => q.OrderBy(t => t.Priority));
        }

        public List<ImageProfileScriptEntity> SearchImageProfileScripts(int profileId)
        {
            return _uow.ImageProfileScriptRepository.Get(x => x.ProfileId == profileId, q => q.OrderBy(t => t.Priority));
        }

        public List<ImageProfileSysprepTagEntity> SearchImageProfileSysprepTags(int profileId)
        {
            return _uow.ImageProfileSysprepRepository.Get(x => x.ProfileId == profileId, q => q.OrderBy(t => t.Priority));
        }

        public ActionResultDTO UpdateProfile(ImageProfileEntity profile)
        {
            var existingProfile = ReadProfile(profile.Id);
            if (existingProfile == null)
                return new ActionResultDTO {ErrorMessage = "Image Profile Not Found", Id = 0};

            var actionResult = new ActionResultDTO();
            var validationResult = ValidateImageProfile(profile, false);
            if (!string.IsNullOrEmpty(profile.MunkiAuthPassword))
                profile.MunkiAuthPassword = new EncryptionServices().EncryptText(profile.MunkiAuthPassword);
            if (validationResult.Success)
            {
                _uow.ImageProfileRepository.Update(profile, profile.Id);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = profile.Id;
            }
            else
            {
                actionResult.ErrorMessage = validationResult.ErrorMessage;
            }

            return actionResult;
        }

        private ValidationResultDTO ValidateImageProfile(ImageProfileEntity imageProfile, bool isNewImageProfile)
        {
            var validationResult = new ValidationResultDTO {Success = true};

            if (string.IsNullOrEmpty(imageProfile.Name) ||
                !imageProfile.Name.All(c => char.IsLetterOrDigit(c) || c == '_'))
            {
                validationResult.Success = false;
                validationResult.ErrorMessage = "Image Profile Name Is Not Valid";
                return validationResult;
            }

            if (isNewImageProfile)
            {
                if (
                    _uow.ImageProfileRepository.Exists(
                        h => h.Name == imageProfile.Name && h.ImageId == imageProfile.ImageId))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "This Image Profile Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var originalImageProfile = _uow.ImageProfileRepository.GetById(imageProfile.Id);
                if (originalImageProfile.Name != imageProfile.Name)
                {
                    if (
                        _uow.ImageProfileRepository.Exists(
                            h => h.Name == imageProfile.Name && h.ImageId == imageProfile.ImageId))
                    {
                        validationResult.Success = false;
                        validationResult.ErrorMessage = "This Image Profile Already Exists";
                        return validationResult;
                    }
                }
            }

            return validationResult;
        }
    }
}