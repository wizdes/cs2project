<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CS2.Web._Default" %>
<%@ Import namespace="CS2.Services.Searching"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:TextBox ID="TextBoxPath" runat="server" Width="590px"></asp:TextBox>&nbsp;<asp:Button
                    ID="ButtonRequestIndexing" runat="server" Text="Request indexing" OnClick="ButtonRequestIndexing_Click" /><br />
                <br />
                <asp:TextBox ID="TextBoxSearch" runat="server"></asp:TextBox>
                <asp:Button ID="ButtonSearch" runat="server" OnClick="ButtonSearch_Click" Text="Button" /><br />
                <br />
                <asp:DataList ID="DataList1" runat="server" CellPadding="4" ForeColor="#333333">
                <ItemTemplate>
                <%#((SearchResult) Container.DataItem).Snippet%>
                <br />
                <%#((SearchResult) Container.DataItem).FilePath%>
                </ItemTemplate>
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <SelectedItemStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                    <AlternatingItemStyle BackColor="White" />
                    <ItemStyle BackColor="#EFF3FB" />
                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                
                </asp:DataList>
                
            </ContentTemplate>
        </asp:UpdatePanel>
    
    </div>
    </form>
</body>
</html>
