﻿using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using CloneDeploy_Entities;
using CloneDeploy_Web;
using CloneDeploy_Web.BasePages;
using CloneDeploy_Web.Helpers;

public partial class views_global_rooms_search : Global
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) BindGrid();
        else
        {
            if (gvRooms.Rows[0].Cells[0].Text == "No Rooms Have Been Created")
            {
                gvRooms.Rows[0].Cells.Clear();
                gvRooms.Rows[0].Cells.Add(new TableCell());
                gvRooms.Rows[0].Cells[0].Text = "No Rooms Have Been Created";
            }
        }
    }

    protected void search_Changed(object sender, EventArgs e)
    {
        BindGrid();

    }
    protected void BindGrid()
    {
        gvRooms.DataSource = Call.RoomApi.GetAll(Int32.MaxValue,txtSearch.Text);
        gvRooms.DataBind();

        if (gvRooms.Rows.Count == 0)
        {
            var obj = new List<RoomEntity>();
            obj.Add(new RoomEntity());
            gvRooms.DataSource = obj;
            gvRooms.DataBind();

            gvRooms.Rows[0].Cells.Clear();
            gvRooms.Rows[0].Cells.Add(new TableCell());

            gvRooms.Rows[0].Cells[0].Text = "No Rooms Have Been Created";

        }
    }

    protected void Insert(object sender, EventArgs e)
    {
        RequiresAuthorization(Authorizations.CreateGlobal);
        GridViewRow gvRow = (GridViewRow)(sender as Control).Parent.Parent;
        var room = new RoomEntity()
        {
            Name = ((TextBox)gvRow.FindControl("txtNameAdd")).Text,
            DistributionPointId = Convert.ToInt32(((DropDownList)gvRow.FindControl("ddlDpAdd")).SelectedValue)
        };

        Call.RoomApi.Post(room);
        BindGrid();
    }

    protected void OnRowEditing(object sender, GridViewEditEventArgs e)
    {
        gvRooms.EditIndex = e.NewEditIndex;
        BindGrid();
    }


    protected void OnRowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        RequiresAuthorization(Authorizations.UpdateGlobal);
        GridViewRow gvRow = gvRooms.Rows[e.RowIndex];
        var room = new RoomEntity()
        {
            Id = Convert.ToInt32(gvRooms.DataKeys[e.RowIndex].Values[0]),
            Name = ((TextBox)gvRow.FindControl("txtName")).Text,
            DistributionPointId = Convert.ToInt32(((DropDownList)gvRow.FindControl("ddlDp")).SelectedValue)

        };
        Call.RoomApi.Put(room.Id, room);

        gvRooms.EditIndex = -1;
        this.BindGrid();
    }

    protected void OnRowCancelingEdit(object sender, EventArgs e)
    {
        gvRooms.EditIndex = -1;
        BindGrid();
    }

    protected void OnRowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        RequiresAuthorization(Authorizations.DeleteGlobal);
        Call.RoomApi.Delete(Convert.ToInt32(gvRooms.DataKeys[e.RowIndex].Values[0]));
        BindGrid();
    }

    protected void gvRooms_OnRowDataBound(object sender, GridViewRowEventArgs e)
    {
        DropDownList ddlDps = null;

        if (e.Row.RowType == DataControlRowType.Footer)
        {
            ddlDps = e.Row.FindControl("ddlDpAdd") as DropDownList;
            PopulateClusterGroupsDdl(ddlDps);

        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ddlDps = e.Row.FindControl("ddlDp") as DropDownList;
            if (ddlDps != null)
            {
                PopulateClusterGroupsDdl(ddlDps);
                ddlDps.SelectedValue = ((RoomEntity)(e.Row.DataItem)).ClusterGroup.ToString();
            }
        } 
    }
}