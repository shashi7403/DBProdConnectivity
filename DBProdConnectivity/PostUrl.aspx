<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PCF_NEW.Master" CodeBehind="PostUrl.aspx.cs" Inherits="DBProdConnectivity.PostUrl" %>

<asp:content id="Content1" contentplaceholderid="headContents" runat="server">
    <head>
        <script src="../Scripts/jquery-1.3.2.min.js" type="text/javascript"></script>
        <script src="../Scripts/jquery-ui.min.js" type="text/javascript"></script>
        <script src="../scripts/ReplayStatus.js" type="text/javascript"></script>
        <link href="../StyleSheets/jquery-ui.css" rel="stylesheet" type="text/css" />
    </head>
</asp:content>
<asp:content id="Content2" contentplaceholderid="BodyContents" runat="server">
    <asp:ScriptManager runat="server" ID="ScriptMngr">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdateCtrls" runat="server">
        <ContentTemplate>
            <table runat="server" border="5" style="border-color: transparent; width: 100%;"
                cellpadding="2" cellspacing="2">
                <tr>
                    <td class="pageHeader" colspan="4">
                        <asp:Label ID="lblPageHeader" runat="server" Text="Send Order to ELMS" Font-Bold="True"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <fieldset>
                            <table>
                                <tr>
                                    <td align="left">
                                        <asp:Label runat="server" ID="Label1" Text=" Dell Sales Order Number" />
                                    </td>
                                    <td align="center">
                                        <asp:TextBox ID="txtOrderNumber" runat="server"></asp:TextBox>
                                        </textbox>
                                    </td>

                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:Label runat="server" ID="Label2" Text=" Sold to Email" />
                                    </td>
                                    <td align="center">
                                        <asp:TextBox ID="txtEmail" runat="server"></asp:TextBox>
                                        </textbox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:Label runat="server" ID="Label3" Text=" InstalAt SiteID" />
                                    </td>
                                    <td align="center">
                                        <asp:TextBox ID="txtInstall" runat="server"></asp:TextBox>
                                        </textbox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:Label runat="server" ID="Label4" Text=" EMC Part Number" />
                                    </td>
                                    <td align="center">
                                        <asp:TextBox ID="txtEmcPartNumber" runat="server"></asp:TextBox>
                                        </textbox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:Label runat="server" ID="Label5" Text="Quantity" />
                                    </td>
                                    <td align="center">
                                        <asp:TextBox ID="txtQuantity" runat="server"></asp:TextBox>
                                        </textbox>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4" align="right">
                                        <asp:HiddenField ID="hdnIndexStart" runat="server" Value="0" />
                                        <asp:Button ID="btnConnectEmc" runat="server" Font-Bold="True" Style="margin-left: 0px"
                                            Text="Connect to EMC" CausesValidation="True" OnClick="btnConnectEmc_Click" />
                                    </td>
                                </tr>
                            </table>
                            <table border="1" width="100%">
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtResponseEmc" runat="server" ReadOnly="True" Rows="15" Width="100%" placeholder="Response Message from EMC" TextMode="MultiLine"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:content>

