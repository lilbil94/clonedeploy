﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using CloneDeploy_Common;
using CloneDeploy_Entities;
using CloneDeploy_Web.BasePages;

namespace CloneDeploy_Web.views.global.munki
{
    public partial class views_global_munki_availablecatalogs : Global
    {
        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.UpdateGlobal);

            var updateCount = 0;
            foreach (GridViewRow row in gvCatalogs.Rows)
            {
                var enabled = (CheckBox) row.FindControl("chkSelector");
                if (enabled == null) continue;
                if (!enabled.Checked) continue;

                var dataKey = gvCatalogs.DataKeys[row.RowIndex];
                if (dataKey == null) continue;

                var catalog = new MunkiManifestCatalogEntity
                {
                    Name = dataKey.Value.ToString(),
                    ManifestTemplateId = ManifestTemplate.Id
                };
                var txtPriority = row.FindControl("txtPriority") as TextBox;
                if (txtPriority != null)
                    if (!string.IsNullOrEmpty(txtPriority.Text))
                        catalog.Priority = Convert.ToInt32(txtPriority.Text);

                if (Call.MunkiManifestTemplateApi.AddCatalogToTemplate(catalog)) updateCount++;
            }

            if (updateCount > 0)
            {
                EndUserMessage = "Successfully Updated Catalogs";
                ManifestTemplate.ChangesApplied = 0;
                Call.MunkiManifestTemplateApi.Put(ManifestTemplate.Id, ManifestTemplate);
            }
            else
            {
                EndUserMessage = "Could Not Update Catalogs";
            }

            PopulateGrid();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            PopulateGrid();
        }

        protected void PopulateGrid()
        {
            var catalogs = Call.FilesystemApi.GetMunkiResources("catalogs");
            IEnumerable<FileInfo> searchedCatalogs = null;
            if (catalogs != null)
            {
                searchedCatalogs =
                    catalogs.Where(
                        f =>
                            f.Name.IndexOf(txtSearch.Text, StringComparison.CurrentCultureIgnoreCase) != -1 &&
                            f.Name != "all");
            }
            gvCatalogs.DataSource = searchedCatalogs;
            gvCatalogs.DataBind();

            if (searchedCatalogs != null)
                lblTotal.Text = gvCatalogs.Rows.Count + " Result(s) / " + searchedCatalogs.Count() + " Total Catalog(s)";
        }

        protected void search_Changed(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void showAvailable_OnClick(object sender, EventArgs e)
        {
            Available.Visible = !Available.Visible;
        }
    }
}