﻿<%@ Page Title="" Language="C#" MasterPageFile="~/views/tasks/task.master" AutoEventWireup="true" Inherits="CloneDeploy_Web.views.tasks.views_tasks_activemulticast" Codebehind="activemulticast.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="BreadcrumbSub" Runat="Server">
    <li>Active Multicasts</li>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Help" Runat="Server">
    <li role="separator" class="divider"></li>
    <li>
        <a href="<%= ResolveUrl("~/views/help/tasks-activemulticast.aspx") %>" target="_blank">Help</a>
    </li>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="SubPageActionsRight">

    <button type="button" class="btn btn-default dropdown-toggle width_140" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
        <span class="caret"></span>
    </button>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="SubContent" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#multicast').addClass("nav-current");
        });
    </script>
 
    <asp:UpdatePanel ID="UpdatePanel" runat="server">
        <ContentTemplate>
            <asp:Timer ID="TimerMC" runat="server" Interval="2000" OnTick="TimerMC_Tick">
            </asp:Timer>
            <p class="total">
                <asp:Label ID="lblTotal" runat="server"></asp:Label>
            </p>
            <br class="clear"/>
            <br/>
            <asp:GridView ID="gvMcTasks" runat="server" CssClass="Gridview extraPad" AutoGenerateColumns="False" DataKeyNames="Id" AlternatingRowStyle-CssClass="alt">
                <Columns>
                    <asp:BoundField DataField="Id" HeaderText="mcTaskID" InsertVisible="False" ReadOnly="True" SortExpression="Id" Visible="False"/>
                    <asp:TemplateField ShowHeader="False" ItemStyle-CssClass="width_30">
                        <ItemTemplate>
                            <div style="width: 0">
                                <asp:LinkButton ID="btnMembers" runat="server" CausesValidation="false" CommandName="" Text="+" OnClick="btnMembers_Click"></asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ShowHeader="False" ItemStyle-CssClass="chkboxwidth">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnCancelMc" runat="server" CausesValidation="false" CommandName="" Text="Cancel" OnClick="btnCancelMc_Click"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="mcTaskName"/>
                    <asp:BoundField DataField="Pid" HeaderText="PID" SortExpression="mcPID" ItemStyle-CssClass="mobi-hide-smallest" HeaderStyle-CssClass="mobi-hide-smallest"/>
                    <asp:TemplateField ShowHeader="True" HeaderText="Partition" ItemStyle-CssClass="mobi-hide-smaller" HeaderStyle-CssClass="mobi-hide-smaller">
                        <ItemTemplate>
                            <asp:Label ID="lblPartition" runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ShowHeader="True" HeaderText="Elapsed" ItemStyle-CssClass="mobi-hide-small" HeaderStyle-CssClass="mobi-hide-small">
                        <ItemTemplate>
                            <asp:Label ID="lblElapsed" runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ShowHeader="True" HeaderText="Remaining" ItemStyle-CssClass="mobi-hide-small" HeaderStyle-CssClass="mobi-hide-small">
                        <ItemTemplate>
                            <asp:Label ID="lblRemaining" runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ShowHeader="True" HeaderText="Completed">
                        <ItemTemplate>
                            <asp:Label ID="lblCompleted" runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ShowHeader="True" HeaderText="Rate" ItemStyle-CssClass="mobi-hide-smallest" HeaderStyle-CssClass="mobi-hide-smallest">
                        <ItemTemplate>
                            <asp:Label ID="lblRate" runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField>
                        <ItemTemplate>
                            <tr>
                                <td runat="server" id="tdMembers" colspan="700" visible="false">
                                    <asp:GridView ID="gvMembers" AutoGenerateColumns="false" runat="server" CssClass="Gridview gv_members" ShowHeader="false" Visible="false" AlternatingRowStyle-CssClass="alt">
                                        <Columns>
                                            <asp:TemplateField ItemStyle-CssClass="width_150" HeaderText="Name">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblComputer" runat="server" Text='<%# Bind("Computer.Name") %>'/>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" ItemStyle-CssClass="width_50"/>
                                            <asp:BoundField DataField="Partition" HeaderText="Partition" ItemStyle-CssClass="mobi-hide-smaller" HeaderStyle-CssClass="mobi-hide-smaller"/>
                                            <asp:BoundField DataField="Elapsed" HeaderText="Elapsed" ItemStyle-CssClass="mobi-hide-small" HeaderStyle-CssClass="mobi-hide-small"/>
                                            <asp:BoundField DataField="Remaining" HeaderText="Remaining" ItemStyle-CssClass="mobi-hide-small" HeaderStyle-CssClass="mobi-hide-small"/>
                                            <asp:BoundField DataField="Completed" HeaderText="Completed"/>
                                            <asp:BoundField DataField="Rate" HeaderText="Rate" ItemStyle-CssClass="mobi-hide-smallest" HeaderStyle-CssClass="mobi-hide-smallest"/>
                                        </Columns>
                                    </asp:GridView>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    No Active Multicasts
                </EmptyDataTemplate>
                <HeaderStyle CssClass="taskgridheader"></HeaderStyle>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>