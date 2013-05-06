using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mailmerge_splitter
{
    public partial class Form1 : Form
    {
        private log4net.ILog _log = log4net.LogManager.GetLogger(typeof(Form1));

        public Form1()
        {
            InitializeComponent();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = txtFilePath.Text;
                _log.DebugFormat("Opening document {0}...", fileName);

                using (DocumentFormat.OpenXml.Packaging.WordprocessingDocument document = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(fileName, true))
                {
                    DocumentFormat.OpenXml.Wordprocessing.Body body = document.MainDocumentPart.Document.Body;

                    ExtractPages(document);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }

        public void ExtractPages(DocumentFormat.OpenXml.Packaging.WordprocessingDocument document)
        {
            IEnumerable<DocumentFormat.OpenXml.Wordprocessing.Paragraph> topP = document.MainDocumentPart.Document.Body.Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>();
            foreach (DocumentFormat.OpenXml.Wordprocessing.Paragraph p in topP)
            {
                _log.DebugFormat("p: {0}", p.InnerText);
                if (ContainSectionBreak(p))
                {
                    _log.Debug("============= Section Break =============");
                }
                if (ContainPageBreak(p))
                {
                    _log.Debug("============= Page Break =============");
                }
            }
        }
        public bool ContainPageBreak(DocumentFormat.OpenXml.Wordprocessing.Paragraph p)
        {
            return p.Elements<DocumentFormat.OpenXml.Wordprocessing.Run>().FirstOrDefault(r => r.Elements<DocumentFormat.OpenXml.Wordprocessing.Break>().FirstOrDefault(b => b.Type == DocumentFormat.OpenXml.Wordprocessing.BreakValues.Page) != null) != null;
        }
        public bool ContainLastRenderedPageBreak(DocumentFormat.OpenXml.Wordprocessing.Paragraph p)
        {
            return p.Elements<DocumentFormat.OpenXml.Wordprocessing.Run>().FirstOrDefault(r => r.Elements<DocumentFormat.OpenXml.Wordprocessing.LastRenderedPageBreak>().FirstOrDefault() != null) != null;
        }
        public bool ContainSectionBreak(DocumentFormat.OpenXml.Wordprocessing.Paragraph p)
        {
            if (p.ParagraphProperties != null && p.ParagraphProperties.SectionProperties != null)
            {
                if (p.ParagraphProperties.SectionProperties.LocalName == "sectPr")
                {
                    return true;
                }
            }
            
            return false;
        }


        private void btnOpenFileDialog_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Word Documents (*.doc/*.docx)|*.doc;*.docx|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtFilePath.Text = openFileDialog1.FileName;
            }
        }
    }
}
