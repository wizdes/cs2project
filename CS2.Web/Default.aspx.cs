using System;
using System.IO;

namespace CS2.Web
{
    public partial class _Default : System.Web.UI.Page
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
        }

        protected void ButtonSearch_Click(object sender, EventArgs e)
        {
            DataList1.DataSource = Global.SearchService.SearchWithHighlighting(TextBoxSearch.Text);
            DataList1.DataBind();
        }

  
    }
}
