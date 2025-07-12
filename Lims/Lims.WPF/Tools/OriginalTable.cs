using Spire.Doc;
using Spire.Doc.Collections;

namespace Lims.WPF.Tools
{
    public class OriginalTable : Table
    {
        private readonly Spire.Doc.Interface.IDocument doc;

        public OriginalTable(Spire.Doc.Interface.IDocument doc) : base(doc)
        {
            this.doc = doc;
            GetCells();
        }

        public List<OriginalCell> Cells { get; set; }


        private void GetCells()
        {
            Cells = new List<OriginalCell>();
            var table = doc.Sections[0].Tables[0];
            for (int i = 0; i < table.Rows.Count; i++)
            {
                for (int j = 0; j < table.Rows[i].Cells.Count; j++)
                {
                    Cells.Add(new OriginalCell(table.Rows[i])
                    {
                        Text = table.Rows[i].Cells[j].FirstParagraph.Text,
                        Row = i + 1,
                        Column = j + 1,
                    });
                }
            }
        }
    }
    public class OriginalCell : CellCollection
    {
        public OriginalCell(TableRow owner) : base(owner)
        {
        }
        public int Row { get; set; }
        public int Column { get; set; }
        public string Text { get; set; }
    }
}
