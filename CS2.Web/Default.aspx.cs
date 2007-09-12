using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using CS2.Core.Searching;

namespace CS2.Web
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void ButtonRequestIndexing_Click(object sender, EventArgs e)
        {
            if(File.Exists(TextBoxPath.Text))
                Global.IndexingService.RequestIndexing(new FileInfo(TextBoxPath.Text));
            else if(Directory.Exists(TextBoxPath.Text))
                Global.IndexingService.RequestIndexing(new DirectoryInfo(TextBoxPath.Text));

            GridView1.Visible = false;
            LiteralIndexingResult.Text = string.Format("Successfully requested indexing for {0}", TextBoxPath.Text);
        }

        protected override void OnPreRenderComplete(EventArgs e)
        {
            int count = ((List<SearchResult>) Context.Items["results"]).Count;
            long elapsed = (long) Context.Items["elapsed"];

            if(count > 0)
                ResultsLiteral.Text =
                    string.Format("Results <b>{0}</b> - <b>{1}</b> of about <b>{2}</b>. (<b>{3}</b> milliseconds)",
                                  GridView1.PageIndex * GridView1.PageSize + 1,
                                  GridView1.PageIndex * GridView1.PageSize + GridView1.Rows.Count, count, elapsed);

            base.OnPreRender(e);
        }
    }
}