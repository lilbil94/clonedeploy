﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CloneDeploy_Entities;
using CloneDeploy_Web.Helpers;

namespace CloneDeploy_Web.views.admin.cluster
{
    public partial class editcluster : BasePages.Admin
    {
        public ClusterGroupEntity ClusterGroup { get { return Read(); } }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) PopulateForm();
        }

        protected void PopulateForm()
        {
            txtClusterName.Text = ClusterGroup.Name;
            chkDefault.Checked = ClusterGroup.Default == 1;

            var secondaryServers = Call.SecondaryServerApi.GetAll(Int32.MaxValue, "");
            if (Settings.OperationMode == "Cluster Primary")
            {
                var primary = new SecondaryServerEntity();
                primary.Id = -1;
                primary.Name = Settings.ServerIdentifier;
                primary.ImageRole = Settings.ImageServerRole ? 1 : 0;
                primary.TftpRole = Settings.TftpServerRole ? 1 : 0;
                primary.MulticastRole = Settings.MulticastServerRole ? 1 : 0;
                secondaryServers.Insert(0, primary);
            }
            gvServers.DataSource = secondaryServers;
            gvServers.DataBind();

            foreach (GridViewRow row in gvServers.Rows)
            {
                var cb = (CheckBox)row.FindControl("chkSelector");
                var cbImage = (CheckBox)row.FindControl("chkImage");
                var cbTftp = (CheckBox)row.FindControl("chkTftp");
                var cbMulticast = (CheckBox)row.FindControl("chkMulticast");
                var dataKey = gvServers.DataKeys[row.RowIndex];
                if (dataKey == null) continue;

                if (Convert.ToInt32(dataKey.Value) == -1)
                {
                    cbImage.Visible = Settings.ImageServerRole;
                    cbTftp.Visible = Settings.TftpServerRole;
                    cbMulticast.Visible = Settings.MulticastServerRole;
                }
                else
                {
                    var secondaryServer = Call.SecondaryServerApi.Get(Convert.ToInt32(dataKey.Value));
                    if (secondaryServer.ImageRole != 1)
                        cbImage.Visible = false;
                    if (secondaryServer.TftpRole != 1)
                        cbTftp.Visible = false;
                    if (secondaryServer.MulticastRole != 1)
                        cbMulticast.Visible = false;
                }

                foreach (var clusterServer in Call.ClusterGroupApi.GetClusterServers(ClusterGroup.Id))
                {
                    if (clusterServer.SecondaryServerId == Convert.ToInt32(dataKey.Value))
                    {
                        cb.Checked = true;
                        cbImage.Checked = clusterServer.ImageRole == 1;
                        cbTftp.Checked = clusterServer.TftpRole == 1;
                        cbMulticast.Checked = clusterServer.MulticastRole == 1;
                    }
                }
            }

        }

        private ClusterGroupEntity Read()
        {
            return Call.ClusterGroupApi.Get(Convert.ToInt32(Request.QueryString["clusterid"]));

        }

        protected void btnUpdateCluster_OnClick(object sender, EventArgs e)
        {
            RequiresAuthorization(Authorizations.UpdateAdmin);
            var clusterGroup = new ClusterGroupEntity()
            {
                Id = ClusterGroup.Id,
                Name = txtClusterName.Text,
                Default = chkDefault.Checked ? 1 : 0
            };

            var result = Call.ClusterGroupApi.Put(clusterGroup.Id,clusterGroup);
            if (result.Success)
            {
                var listOfServers = new List<ClusterGroupServerEntity>();
                foreach (GridViewRow row in gvServers.Rows)
                {
                    var cb = (CheckBox)row.FindControl("chkSelector");
                    if (!cb.Checked) continue;

                    var cbImage = (CheckBox)row.FindControl("chkImage");
                    var cbTftp = (CheckBox)row.FindControl("chkTftp");
                    var cbMulticast = (CheckBox)row.FindControl("chkMulticast");
                    var dataKey = gvServers.DataKeys[row.RowIndex];
                    if (dataKey == null) continue;

                    var clusterGroupServer = new ClusterGroupServerEntity();
                    clusterGroupServer.ClusterGroupId = result.Id;
                    clusterGroupServer.SecondaryServerId = Convert.ToInt32(dataKey.Value);
                    if (cbImage.Checked)
                        clusterGroupServer.ImageRole = 1;
                    if (!cbImage.Visible)
                        clusterGroupServer.ImageRole = 0;
                    if (cbTftp.Checked)
                        clusterGroupServer.TftpRole = 1;
                    if (!cbTftp.Visible)
                        clusterGroupServer.TftpRole = 0;
                    if (cbMulticast.Checked)
                        clusterGroupServer.MulticastRole = 1;
                    if (!cbMulticast.Visible)
                        clusterGroupServer.MulticastRole = 0;

                    listOfServers.Add(clusterGroupServer);
                }

                if(listOfServers.Count == 0)
                    listOfServers.Add(new ClusterGroupServerEntity(){ClusterGroupId = ClusterGroup.Id,SecondaryServerId = -2});
                Call.ClusterGroupServerApi.Post(listOfServers);

                EndUserMessage = "Successfully Updated Cluster Group";
              
            }
            else
            {
                EndUserMessage = result.ErrorMessage;
            }
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvServers);
        }
        
    }
}