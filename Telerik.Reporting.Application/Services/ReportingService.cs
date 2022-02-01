using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Telerik.Documents.Core.Fonts;
using Telerik.Documents.Primitives;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Windows.Documents.Fixed.Model.ColorSpaces;
using Telerik.Windows.Documents.Fixed.Model.Editing;
using Telerik.Windows.Documents.Fixed.Model.Editing.Flow;
using Telerik.Windows.Documents.Fixed.Model.Editing.Tables;
using Telerik.Windows.Documents.Fixed.Model.Fonts;
using FileInfo = Telerik.Reporting.Application.Report.FileInfo;
using Table = Telerik.Windows.Documents.Fixed.Model.Editing.Tables.Table;
using TableCell = Telerik.Windows.Documents.Fixed.Model.Editing.Tables.TableCell;
using TableRow = Telerik.Windows.Documents.Fixed.Model.Editing.Tables.TableRow;
using FontStyle = Telerik.Documents.Core.Fonts.FontStyle;
using FontFamily = Telerik.Documents.Core.Fonts.FontFamily;
using System.Threading.Tasks;

namespace Telerik.Reporting.Application.Services
{
    public class ReportingService : IReportingService
    {
        private readonly string _blobContainer;
        private readonly string _userEmail;
        private readonly FontFamily _fontFamilyRegular;
        private readonly FontFamily _fontFamilyBold;

        private const double PageWidth = 792;
        private const double PageHeight = 1128;
        private const double DefaultLeftIndent = 20;
        private const double DefaultRightIndent = 650;
        private const double DefaultLineHeight = 18;

        private double _currentTopOffset;
        private int _pageCounter;
        private int _totalPageCounter;


        public ReportingService()
        {
            byte[] fontDataRegular = File.ReadAllBytes(@"Fonts/Arial-Unicode-Regular.ttf");
            _fontFamilyRegular = new Telerik.Documents.Core.Fonts.FontFamily("Arial-Unicode-Regular");
            FontsRepository.RegisterFont(_fontFamilyRegular, Telerik.Documents.Core.Fonts.FontStyles.Normal, Telerik.Documents.Core.Fonts.FontWeights.Normal, fontDataRegular);

            byte[] fontDataBold = File.ReadAllBytes(@"Fonts/Arial-Unicode-Bold.ttf");
            _fontFamilyBold = new Telerik.Documents.Core.Fonts.FontFamily("Arial-Unicode-Bold");
            FontsRepository.RegisterFont(_fontFamilyBold, Telerik.Documents.Core.Fonts.FontStyles.Normal, Telerik.Documents.Core.Fonts.FontWeights.Bold, fontDataBold);
        }

        public async Task<FileInfo> GetFileForAutomatedTest()
        {
            string fileName = String.Empty;
            Telerik.Windows.Documents.Fixed.FormatProviders.Pdf.PdfFormatProvider formatProvider =
                new Telerik.Windows.Documents.Fixed.FormatProviders.Pdf.PdfFormatProvider
                {
                    ExportSettings = { ImageQuality = Windows.Documents.Fixed.FormatProviders.Pdf.Export.ImageQuality.High }
                };

            RadFixedDocument document = await CreateDocumentAutomatedTest();

            byte[] renderedBytes;

            await using (MemoryStream ms = new MemoryStream())
            {
                formatProvider.Export(document, ms);
                renderedBytes = ms.ToArray();
                fileName = "blah.pdf";

            }

            return new FileInfo { FileName = fileName, RenderedBytes = renderedBytes };
        }

        public async Task<RadFixedDocument> CreateDocumentAutomatedTest()
        {
            RadFixedDocument document = new RadFixedDocument();
            //RadFixedPage page = document.Pages.AddPage();
            //page.Size = new Telerik.Documents.Primitives.Size(PageWidth, PageHeight);
            //FixedContentEditor editor = new FixedContentEditor(page);

            //_currentTopOffset = 0;
            //_currentTopOffset += DefaultLineHeight * 2;
            //_pageCounter = 1;
            //_totalPageCounter = 2;

            //editor.Position.Translate(DefaultLeftIndent, _currentTopOffset);
   
            await DrawContentAutomatedTest(document);

            return document;
        }

        private async Task DrawContentAutomatedTest(RadFixedDocument document)
        {

            DrawContentTest(document);

        }

        private void DrawContentTest(RadFixedDocument document)
        {
            RadFixedPage page = document.Pages.AddPage();
            page.Size = new Telerik.Documents.Primitives.Size(PageWidth, PageHeight);
            _currentTopOffset = 0;

            FixedContentEditor editorNewPageTest = new FixedContentEditor(page);
            DrawHeading(editorNewPageTest, "blah sdfsdf sdfsdf sdfsdf sdfsdf sdf", 11, _fontFamilyRegular);

        }
        private void DrawHeading(FixedContentEditor editor, string heading, int fontSize, FontFamily fontFamily)
        {

            _currentTopOffset = 0;
            editor.Position.Translate(DefaultLeftIndent, _currentTopOffset);

            RgbColor bordersColor = new RgbColor(106, 108, 111);
            Border border = new Border(1, BorderStyle.Single, bordersColor);
            Table table = new Table();
            table.Borders = new TableBorders(new Border(BorderStyle.None), new Border(BorderStyle.None), new Border(BorderStyle.None), border);
            table.LayoutType = TableLayoutType.FixedWidth;
            table.DefaultCellProperties.Padding = new Thickness(10);
            table.Background = new RgbColor(245, 245, 245);
            TableRow tableRow = table.Rows.AddTableRow();

            TableCell tableCell = tableRow.Cells.AddTableCell();
            tableCell.Borders = new TableCellBorders(new Border(BorderStyle.None));
            Block tableBlock = tableCell.Blocks.AddBlock();
            tableBlock.GraphicProperties.FillColor = new RgbColor(106, 108, 111); //#6a6c6f
            tableBlock.HorizontalAlignment = HorizontalAlignment.Left;
            tableBlock.TextProperties.FontSize = fontSize;
            tableBlock.InsertText(_fontFamilyBold, FontStyles.Normal, FontWeights.Bold, heading);

            table.Draw(editor, new Rect(DefaultLeftIndent - 20, _currentTopOffset, PageWidth, 100));


        }
    }
}
