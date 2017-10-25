using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using DocWorks.GDocFactory.Model;
using Markdig;

namespace DocWorks.GDocFactory.Services
{
    class DataConversion:IDataConversion
    {
        public string ConvertMarkdownToHtml(string markdownContent)
        {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            return Markdown.ToHtml(markdownContent, pipeline);
        }
    }
}