﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using CloneDeploy_Common;
using CloneDeploy_Entities;
using CloneDeploy_Web.BasePages;

namespace CloneDeploy_Web.views.global.munki
{
    public partial class views_global_munki_availablemanagedinstalls : Global
    {
        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.UpdateGlobal);

            var updateCount = 0;
            foreach (GridViewRow row in gvPkgInfos.Rows)
            {
                var enabled = (CheckBox) row.FindControl("chkSelector");
                if (enabled == null) continue;
                if (!enabled.Checked) continue;

                var dataKey = gvPkgInfos.DataKeys[row.RowIndex];
                if (dataKey == null) continue;

                var managedInstall = new MunkiManifestManagedInstallEntity
                {
                    Name = dataKey.Value.ToString(),
                    ManifestTemplateId = ManifestTemplate.Id
                };

                var cbUseVersion = (CheckBox) row.FindControl("chkUseVersion");
                if (cbUseVersion.Checked)
                {
                    managedInstall.Version = row.Cells[2].Text;
                    managedInstall.IncludeVersion = 1;
                }

                var condition = (TextBox) row.FindControl("txtCondition");
                managedInstall.Condition = condition.Text;

                if (Call.MunkiManifestTemplateApi.AddManagedInstallToTemplate(managedInstall)) updateCount++;
            }

            if (updateCount > 0)
            {
                EndUserMessage = "Successfully Updated Managed Installs";
                ManifestTemplate.ChangesApplied = 0;
                Call.MunkiManifestTemplateApi.Put(ManifestTemplate.Id, ManifestTemplate);
            }
            else
            {
                EndUserMessage = "Could Not Update Managed Installs";
            }

            PopulateGrid();
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvPkgInfos);
        }

        protected void ddlLimit_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            PopulateGrid();
        }

        protected void PopulateGrid()
        {
            var availableLimit = ddlLimitAvailable.Text == "All"
                ? int.MaxValue
                : Convert.ToInt32(ddlLimitAvailable.Text);

            var listOfPackages = new List<MunkiPackageInfoEntity>();
            var pkgInfos = Call.FilesystemApi.GetMunkiResources("pkgsinfo");
            if (pkgInfos != null)
            {
                foreach (var pkgInfoFile in pkgInfos)
                {
                    var pkg = Call.FilesystemApi.GetPlist(pkgInfoFile.FullName);
                    if (pkg != null)
                        listOfPackages.Add(pkg);
                }

                listOfPackages =
                    listOfPackages.Where(
                        s => s.Name.IndexOf(txtSearchAvailable.Text, StringComparison.CurrentCultureIgnoreCase) != -1)
                        .Take(availableLimit)
                        .ToList();
                gvPkgInfos.DataSource = listOfPackages;
                gvPkgInfos.DataBind();

                lblTotalAvailable.Text = gvPkgInfos.Rows.Count + " Result(s) / " + pkgInfos.Count +
                                         " Total Packages(s)";
            }
            else
            {
                gvPkgInfos.DataBind();
            }
        }

        protected void search_Changed(object sender, EventArgs e)
        {
            PopulateGrid();
        }
    }
}