using DocWorks.GDocFactory.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocWorks.GDocFactory.Services
{
    interface IDataConversion
    {
        string ConvertMarkdownToHtml(string markdownContent);
    }
}
