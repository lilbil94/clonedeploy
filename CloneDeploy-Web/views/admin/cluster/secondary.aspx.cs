﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CloneDeploy_Web.views.admin.cluster
{
    public partial class secondary : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ButtonConfirmDelete_Click(object sender, EventArgs e)
        {
        }

        protected void search_Changed(object sender, EventArgs e)
        {
            
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ChkAll(gvServers);
        }
    }
}