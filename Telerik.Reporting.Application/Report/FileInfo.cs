using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.Reporting.Application.Report
{
    public struct FileInfo
    {
        public string FileName { get; set; }
        public byte[] RenderedBytes { get; set; }
    }
}
