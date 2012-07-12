<%@ Import Namespace="CS2.Core.Searching" %>

<%@ Page Language="NEMERLE" AutoEventWireup="true" Codebehind="Default.aspx.n" Inherits="CS2.Web.Default" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
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
    <form id="form1" defaultbutton="ButtonSearch" defaultfocus="TextBoxSearch" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <div style="position: absolute; top: 6px; right: 10px; text-align: right">
                <a onclick="toggle('searchSyntax');" href="#">Query syntax</a>
                <div id="searchSyntax" style="display: none;">
                    Documents are indexed and can be queried against<br />
                    on several fields, respecting the following syntax:
                    <br />
                    <br />
                    <b><i>fieldname</i>:<i>query</i></b>
                    <br />
                    <br />
                    If no field is specified full-text queries are performed.<br />
                    The following fields are available:<br />
                    <br />
                    <i>language</i> -> the programming language of the source code file<br />
                    <i>comment</i> -> search into source code comments<br />
                    <i>method</i> -> search into method names<br />
                    <i>class</i> -> search into class names<br />
                    <i>interface</i> -> search into interface names<br />
                    <i>namespace</i> -> search into namespaces<br />
                    <i>property</i> -> search into property names<br />
                    <br />
                    Wildcard queries are supported as well.
                </div>
            </div>
            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                <tr>
                    <td style="width: 120px;">
                        <a href="Default.aspx">
                            <img src="Static/cs2.png" alt="cs2" />
                        </a>
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
                </tr>
            </table>
            <table id="title" style="width: 100%">
                <tr>
                    <td style="height: 16px">
                        <b>Code search results</b> (Indexing a total of <b><%= global_asax.IndexingService.DocumentCount %></b> documents)
                    </td>
                    <td style="text-align: right; height: 16px;">
                        <asp:Literal runat="server" ID="ResultsLiteral" EnableViewState="False"></asp:Literal>
                    </td>
                </tr>
            </table>
            <asp:GridView EnableViewState="false" ID="GridView1" Width="100%" runat="server"
                AutoGenerateColumns="False" GridLines="None" ShowHeader="False" AllowPaging="True"
                DataSourceID="ObjectDataSourceSearchResults" PageSize="10" OnRowDataBound="GridView1_RowDataBound">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <div class="h">
                                <a href="<%# ResolveClientUrl("FileHandler.ashx") + "?f=" + (Container.DataItem :> SearchResult).FilePath%>">
                                    <%#(Container.DataItem :> SearchResult).FilePath%>
                                </a>
                            </div>
                            <pre class="j"><%#(Container.DataItem :> SearchResult).Snippet%></pre>
                            <div class="f">
                                <span class="a">Language:
                                    <%#(Container.DataItem :> SearchResult).Language%>
                                </span>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <p>
                        No results were found.</p>
                </EmptyDataTemplate>
                <PagerStyle CssClass="pager" HorizontalAlign="Center" />
            </asp:GridView>
            <asp:ObjectDataSource ID="ObjectDataSourceSearchResults" runat="server" SelectCountMethod="Count"
                EnablePaging="True" SelectMethod="Search" TypeName="CS2.Web.Global">
                <SelectParameters>
                    <asp:ControlParameter ControlID="TextBoxSearch" Name="query" PropertyName="Text"
                        Type="String" DefaultValue="" ConvertEmptyStringToNull="true" />
                </SelectParameters>
            </asp:ObjectDataSource>
            <asp:Literal ID="LiteralIndexingResult" runat="server" EnableViewState="False"></asp:Literal>
        </div>
    </form>
</body>
</html>
