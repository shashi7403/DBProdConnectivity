<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PCF_NEW.Master" CodeBehind="CBRQuery.aspx.cs" Inherits="DBProdConnectivity.CBRQuery" %>


<asp:Content ID="Content1" ContentPlaceHolderID="headContents" runat="server">
    <head>
        <script src="../Scripts/jquery-1.3.2.min.js" type="text/javascript"></script>
        <script src="../Scripts/jquery-ui.min.js" type="text/javascript"></script>
        <script src="../scripts/ReplayStatus.js" type="text/javascript"></script>
        <link href="../StyleSheets/jquery-ui.css" rel="stylesheet" type="text/css" />
    </head>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContents" runat="server">
    <asp:ScriptManager runat="server" ID="ScriptMngr">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdateCtrls" runat="server">
        <ContentTemplate>
            <table runat="server" id="tblDailyPOReceiptReport" border="5" style="border-color: transparent;"
                width="100%" cellpadding="2" cellspacing="2">
                <tr>
                    <td class="PageHeader" colspan="4">
                        <asp:Label ID="lblPageHeader" runat="server" Font-Bold="True" Text="CBR Query View"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <fieldset>
                            <table>
                                <tr>
                                    <td colspan="2">
                                        <asp:Label ID="lblError" runat="server" CssClass="text-red" />
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="top">
                                        <asp:Label ID="lblProductKeySerialNumber" Text="DPK Serial #" runat="server"></asp:Label>
                                        <asp:Label ID="lblCommaSeparate" Text="(Separated by comma)  " runat="server"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtProductKeySerialNumber" runat="server" TextMode="MultiLine" MaxLength="1600"
                                            Width="500" Height="100"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="reqValThresholdHrs" ControlToValidate="txtProductKeySerialNumber"
                                            runat="server" ErrorMessage="Required" ForeColor="Red" ValidationGroup="AddEdit"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ErrorMessage="Invalid Product Key Serial Number"
                                            ControlToValidate="txtProductKeySerialNumber" ValidationExpression="^[a-zA-Z0-9_,-]+$"
                                            CssClass="text-red" ValidationGroup="AddEdit"></asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td>
                                        <asp:CheckBox ID="chkIsArchiveReq" runat="server" Text="Include archived data" />
                                    </td>
                                </tr>
                                <%--  <tr>
                         <td align="center" colspan="4">
                                <asp:Label ID="lblMessage" Text="Atleast One Criteria Needs to be Available" runat="server"></asp:Label>    
                            </td>
                        </tr>--%>
                                <tr>
                                    <td colspan="2" align="center">
                                        <asp:Button ID="btnViewReport" runat="server" Font-Bold="True" Style="margin-left: 0px"
                                            Text="View Report" ValidationGroup="AddEdit" OnClick="btnViewReport_Click" />
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                    </td>
                </tr>
            </table>
            <table width="100%">
                <%--<tr>
            <td align="right">
                <asp:DropDownList ID="ddlExportFileType" runat="server" Width="200px" DataTextField="value"
                    DataValueField="key">
                </asp:DropDownList>
                <asp:Button ID="btnExportReport" runat="server" Font-Bold="True" Style="margin-left: 0px"
                    Text="Export Report" ValidationGroup="AddEdit" OnClick="btnExportReport_Click" />
            </td>
        </tr>--%>
            </table>
            <table border="1" width="100%">
                <tr>
                    <td>
                        <asp:GridView ID="gvCBRQueryReport" runat="server" Width="100%" CellPadding="2" CssClass="gridviewBorder"
                            AutoGenerateColumns="false" BorderColor="Transparent" BorderWidth="5" GridLines="None"
                            EmptyDataText="No Records Found" ShowHeaderWhenEmpty="True" AllowSorting="true"
                            AllowPaging="true" PageSize="200">
                            <Columns>
                                <asp:TemplateField HeaderText="Serial Number" HeaderStyle-CssClass="GridHeader" ItemStyle-Width="10%"
                                    SortExpression="DPKSerialNumber">
                                    <ItemTemplate>
                                        <asp:Label ID="lblProductKeySerialNumbers" runat="server" Text='<%#(DataBinder.Eval(Container.DataItem,"DPKSerialNumber"))%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="DPK Status" HeaderStyle-CssClass="GridHeader" ItemStyle-Width="10%"
                                    SortExpression="DPKStatus">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCbrStatus" runat="server" Text='<%#(DataBinder.Eval(Container.DataItem,"DPKStatus"))%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="CBR Status" HeaderStyle-CssClass="GridHeader" ItemStyle-Width="10%"
                                    SortExpression="CBRStatus">
                                    <ItemTemplate>
                                        <asp:Label ID="Label1" runat="server" Text='<%#(DataBinder.Eval(Container.DataItem,"CBRStatus"))%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="CBR ACK NACK" HeaderStyle-CssClass="GridHeader" ItemStyle-Width="10%"
                                    SortExpression="CBR_ACK_NACK">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCBR_ACK_NACK" runat="server" Text='<%#(DataBinder.Eval(Container.DataItem,"CBR_ACK_NACK"))%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="CBR NACK Reason" HeaderStyle-CssClass="GridHeader"
                                    ItemStyle-Width="10%" SortExpression="CBR_NACK_Reason">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCbrNackReason" runat="server" Text='<%#(DataBinder.Eval(Container.DataItem,"CBR_NACK_Reason"))%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="CBR Report Unique Id" HeaderStyle-CssClass="GridHeader"
                                    ItemStyle-Width="10%" SortExpression="CBR_Report_Unique_Id">
                                    <ItemTemplate>
                                        <asp:Label ID="lblReportUniqueId" runat="server" Text='<%#(DataBinder.Eval(Container.DataItem,"CBR_Report_Unique_Id"))%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="CBR Sent Date" HeaderStyle-CssClass="GridHeader" ItemStyle-Width="10%"
                                    SortExpression="CBRSentDate">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCbrSendtDate" runat="server" Text='<%#(DataBinder.Eval(Container.DataItem,"CBRSentDate"))%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="CBR Ack Nack Date" HeaderStyle-CssClass="GridHeader"
                                    ItemStyle-Width="10%" SortExpression="CBRAckNackDate">
                                    <ItemTemplate>
                                        <asp:Label ID="Label2" runat="server" Text='<%#(DataBinder.Eval(Container.DataItem,"CBRAckNackDate"))%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Service Tag" HeaderStyle-CssClass="GridHeader" ItemStyle-Width="10%"
                                    SortExpression="ServiceTag">
                                    <ItemTemplate>
                                        <asp:Label ID="lblServiceTag" runat="server" Text='<%#(DataBinder.Eval(Container.DataItem,"ServiceTag"))%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order Number" HeaderStyle-CssClass="GridHeader" ItemStyle-Width="10%"
                                    SortExpression="SalesOrderNumber">
                                    <ItemTemplate>
                                        <asp:Label ID="lblOrderNumber" runat="server" Text='<%#(DataBinder.Eval(Container.DataItem,"SalesOrderNumber"))%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <RowStyle CssClass="grid-alternatereportitem" />
                            <AlternatingRowStyle CssClass="grid-alternateitem" />
                            <SelectedRowStyle CssClass="grid-selecteditem" />
                            <FooterStyle BackColor="#5D7B9D" Font-Names="Verdana" Font-Size="8pt" Font-Bold="True"
                                ForeColor="White" />
                            <PagerStyle BackColor="LightGray" Font-Names="Verdana" Font-Size="8pt" Font-Bold="True" />
                            <HeaderStyle BackColor="Gray" Font-Bold="True" ForeColor="White" />
                        </asp:GridView>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
