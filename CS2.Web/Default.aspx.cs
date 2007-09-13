using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using CS2.Core.Searching;

namespace CS2.Web
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
                GridView1.Visible = false;
        }

        protected void ButtonRequestIndexing_Click(object sender, EventArgs e)
        {
            if(File.Exists(TextBoxPath.Text))
                Global.IndexingService.RequestIndexing(new FileInfo(TextBoxPath.Text));
            else if(Directory.Exists(TextBoxPath.Text))
                Global.IndexingService.RequestIndexing(new DirectoryInfo(TextBoxPath.Text));

            GridView1.Visible = false;

            if (File.Exists(TextBoxPath.Text) || Directory.Exists(TextBoxPath.Text))
                LiteralIndexingResult.Text = string.Format("<p>Successfully requested indexing for {0}.</p>", TextBoxPath.Text);
            else
                LiteralIndexingResult.Text = "<p>No suitable directory of file for indexing.</p>";
        }

        protected void Page_PreRenderComplete(object sender, EventArgs e)
        {
            if(GridView1.Visible)
            {
                int count = ((List<SearchResult>) Context.Items["results"]).Count;
                long elapsed = (long) Context.Items["elapsed"];

                if(count > 0)
                    ResultsLiteral.Text =
                        string.Format("Results <b>{0}</b> - <b>{1}</b> of <b>{2}</b>. (<b>{3}</b> milliseconds)",
                                      GridView1.PageIndex * GridView1.PageSize + 1,
                                      GridView1.PageIndex * GridView1.PageSize + GridView1.Rows.Count, count, elapsed);
                else
                    ResultsLiteral.Text = string.Format("No results found (<b>{0}</b> milliseconds)", elapsed);
            }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if(e.Row.RowType == DataControlRowType.Pager)
            {
                e.Row.Cells[0].Controls.AddAt(0, new LiteralControl("Page through results"));
            }
        }
    }
}