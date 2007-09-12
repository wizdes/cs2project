<%@ Page Language="C#" AutoEventWireup="true" Codebehind="Default.aspx.cs" Inherits="CS2.Web._Default" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Import Namespace="CS2.Core.Searching" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>CS2 - C# Code Search</title>
    <link rel="stylesheet" href="Static/Style.css" />

    <script type="text/javascript">
        function toggle(obj) {
	        var el = document.getElementById(obj);
	        el.style.display = (el.style.display != 'none' ? 'none' : '' );
        }
    </script>

</head>
<body>
    <form id="form1" runat="server" defaultbutton="ButtonSearch">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server" />
            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                <tr>
                    <td style="width: 120px;">
                        <img src="Static/cs2.png" alt="cs2" />
                    </td>
                    <td align="left">
                        <p>
                            <asp:TextBox ID="TextBoxSearch" runat="server" Width="300px"></asp:TextBox>
                            <asp:Button ID="ButtonSearch" runat="server" Text="Search" />
                            <cc1:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server" TargetControlID="TextBoxSearch"
                                WatermarkText="Type search query here" WatermarkCssClass="watermark">
                            </cc1:TextBoxWatermarkExtender>
                        </p>
                        <p>
                            <asp:TextBox ID="TextBoxPath" runat="server" Width="500px" EnableViewState="False"></asp:TextBox>
                            <asp:Button ID="ButtonRequestIndexing" runat="server" Text="Request indexing" OnClick="ButtonRequestIndexing_Click" />
                            <cc1:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender2" runat="server" TargetControlID="TextBoxPath"
                                WatermarkCssClass="watermark" WatermarkText="Type file or directory path to request indexing">
                            </cc1:TextBoxWatermarkExtender>
                            <br />
                        </p>
                    </td>
                    <td align="right" valign="top">
                        <a onclick="toggle('searchSyntax');" href="#">Query syntax</a>
                        <div id="searchSyntax" style="display: none;">
                            ciao<br />
                            ciiaiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiii ciao<br />
                            ciao<br />
                            ciao<br />
                            ciao<br />
                            ciao<br />
                            ciao<br />
                        </div>
                    </td>
                </tr>
            </table>
            <table id="title" style="width: 100%">
                <tr>
                    <td style="height: 16px">
                        <b>Code search results</b>
                    </td>
                    <td style="text-align: right; height: 16px;">
                        <asp:Literal runat="server" ID="ResultsLiteral" EnableViewState="False"></asp:Literal>
                    </td>
                </tr>
            </table>
            <asp:GridView EnableViewState="false" ID="GridView1" Width="100%" runat="server" AutoGenerateColumns="False" GridLines="None"
                ShowHeader="False" AllowPaging="True" DataSourceID="ObjectDataSourceSearchResults" PageSize="15">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <div class="h">
                                <a href="<%# ResolveClientUrl("FileHandler.ashx") + "?f=" + ((SearchResult) Container.DataItem).FilePath%>">
                                    <%#((SearchResult) Container.DataItem).FilePath%>
                                </a>
                            </div>
                            <pre class="j"><%#((SearchResult) Container.DataItem).Snippet%></pre>
                            <div class="f">
                                <span class="a">Language:
                                    <%#((SearchResult) Container.DataItem).Language%>
                                </span>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                <br />
                No results were found
                </EmptyDataTemplate>
            </asp:GridView>
            <asp:ObjectDataSource ID="ObjectDataSourceSearchResults" runat="server" SelectCountMethod="Count" EnablePaging="True"
                SelectMethod="Search" TypeName="CS2.Web.Global">
                <SelectParameters>
                    <asp:ControlParameter ControlID="TextBoxSearch" Name="query" PropertyName="Text"
                        Type="String" DefaultValue="" ConvertEmptyStringToNull="true" />
                </SelectParameters>
            </asp:ObjectDataSource>
        </div>
        <br />
        <asp:Literal ID="LiteralIndexingResult" runat="server" EnableViewState="False"></asp:Literal>
    </form>
</body>
</html>
