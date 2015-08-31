﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Global;
using Models;

public partial class views_admin_server : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;
        txtIP.Text = Settings.ServerIp;
        txtPort.Text = Settings.WebServerPort;
        txtImagePath.Text = Settings.ImageStorePath;
        txtImageHoldPath.Text = Settings.ImageHoldPath;
        txtTFTPPath.Text = Settings.TftpPath;
        txtWebService.Text = Setting.GetValueForAdminView(Settings.WebPath);
        ddlHostView.SelectedValue = Settings.DefaultHostView;
    }

    protected void btnUpdateSettings_OnClick(object sender, EventArgs e)
    {

        if (ValidateSettings())
        {
            List<Setting> listSettings = new List<Setting>
            {
                new Setting {Name = "Server IP", Value = txtIP.Text},
                new Setting {Name = "Web Server Port", Value = txtPort.Text},
                new Setting {Name = "Image Store Path", Value = txtImagePath.Text},
                new Setting {Name = "Tftp Path", Value = txtTFTPPath.Text},
                new Setting {Name = "Web Path", Value = txtWebService.Text},
                new Setting {Name = "Default Host View", Value = ddlHostView.Text},
                new Setting {Name = "Image Hold Path", Value = txtImageHoldPath.Text}
            };

            var newBootMenu = false;
            var newClientIso = false;
            if (new Setting().Update(listSettings))
            {

                if ((string) ViewState["serverIP"] != txtIP.Text)
                {
                    newBootMenu = true;
                    newClientIso = true;
                }
                if ((string) ViewState["serverPort"] != txtPort.Text)
                {
                    newBootMenu = true;
                    newClientIso = true;
                }


                if ((string) ViewState["webService"] != txtWebService.Text)
                {
                    newBootMenu = true;
                    newClientIso = true;
                }



            }

            if (newBootMenu)
            {
                var confirmTitle = Master.Master.FindControl("Content").FindControl("lblTitle") as Label;
                confirmTitle.Text = Utility.Message;
                confirmTitle.Text +=
                    "<br> Your Settings Changes Require A New PXE Boot File Be Created.  <br>Create It Now?";
                if (newClientIso)
                {
                    var isoTitle = Master.Master.FindControl("Content").FindControl("lblClientISO") as Label;
                    isoTitle.Text = "If You Are Using The Client ISO, It Must Also Be Manually Updated.";
                }
                ClientScript.RegisterStartupScript(GetType(), "modalscript",
                    "$(function() {  var menuTop = document.getElementById('confirmbox'),body = document.body;classie.toggle(menuTop, 'confirm-box-outer-open'); });",
                    true);
                Session.Remove("Message");
            }

            

        }
        new Utility().Msgbox(Utility.Message);

    }

    protected bool ValidateSettings()
    {
        if (ActiveTask.ReadAll().Count > 0)
        {
            Utility.Message = "Settings Cannot Be Changed While Tasks Are Active";
            return false;
        }
        if (txtPort.Text != "80" && txtPort.Text != "443" && !string.IsNullOrEmpty(txtPort.Text))
        {
            txtWebService.Text = "http://[server-ip]:" + txtPort.Text + "/cruciblewds/service/client.asmx/";
        }
        if (txtPort.Text == "80" || txtPort.Text == "443" || string.IsNullOrEmpty(txtPort.Text))
        {
            txtWebService.Text = "http://[server-ip]/cruciblewds/service/client.asmx/";
        }
       
        if (!txtImagePath.Text.Trim().EndsWith(Path.DirectorySeparatorChar.ToString()))
            txtImagePath.Text += Path.DirectorySeparatorChar;

        if (!txtImageHoldPath.Text.Trim().EndsWith(Path.DirectorySeparatorChar.ToString()))
            txtImageHoldPath.Text += Path.DirectorySeparatorChar;

        if (!txtTFTPPath.Text.Trim().EndsWith(Path.DirectorySeparatorChar.ToString()))
            txtTFTPPath.Text += Path.DirectorySeparatorChar;

        if (!txtWebService.Text.Trim().EndsWith("/"))
            txtWebService.Text += ("/");

        return true;
    }
}