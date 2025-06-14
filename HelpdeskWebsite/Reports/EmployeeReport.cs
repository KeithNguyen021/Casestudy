using HelpdeskViewModels;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
namespace HelpdeskWebsite.Reports
{
    public class EmployeeReport
    {
        public async Task GenerateReport(string rootpath)
        {
            PageSize pg = PageSize.A4.Rotate();
            var helvetica = PdfFontFactory.CreateFont(StandardFontFamilies.HELVETICA);

            PdfWriter writer = new(rootpath + "/pdfs/employeereport.pdf",
            new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfDocument pdf = new(writer);
            Document document = new(pdf,pg);

            document.Add(new Image(ImageDataFactory.Create(rootpath + "/img/10306707.png"))
            .ScaleAbsolute(100, 100)
            .SetFixedPosition(((pg.GetWidth() - 200) / 2), 450));

            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph(" Current Employees")
                .SetFont(helvetica)
                .SetFontSize(24)
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER));
            document.Add(new Paragraph(""));
            document.Add(new Paragraph(""));


            Table table = new(3);


            table.SetWidth(298)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetRelativePosition(0, 0, 0, 0)
                .SetHorizontalAlignment(HorizontalAlignment.CENTER);


            table.AddCell(new Cell().Add(new Paragraph("Title")
            .SetFontSize(16)
            .SetBold()
            .SetPaddingLeft(18)
            .SetTextAlignment(TextAlignment.LEFT))
            .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("First Name")
            .SetFontSize(16)
            .SetBold()
            .SetPaddingLeft(16)
            .SetTextAlignment(TextAlignment.LEFT))
            .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("Last Name")
            .SetBold()
            .SetFontSize(16)
            .SetPaddingLeft(16)
            .SetTextAlignment(TextAlignment.LEFT))
            .SetBorder(Border.NO_BORDER));

            List<EmployeeViewModel> allStudentVms;
            EmployeeViewModel vm = new();
            allStudentVms = await vm.GetAll();

            foreach (var student in allStudentVms)
            {
                table.AddCell(new Cell().Add(new Paragraph(student.Title)
                .SetFontSize(14)
                .SetPaddingLeft(24)
                .SetTextAlignment(TextAlignment.LEFT))
                .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(student.Firstname)
                .SetFontSize(14)
                .SetPaddingLeft(24)
                .SetTextAlignment(TextAlignment.LEFT))
                .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(student.Lastname)
                .SetFontSize(14)
                .SetPaddingLeft(24)
                .SetTextAlignment(TextAlignment.LEFT))
                .SetBorder(Border.NO_BORDER));
            }


            document.Add(table);
            document.Add(new Paragraph("Student report written on - " + DateTime.Now)
            .SetFontSize(6)
            .SetTextAlignment(TextAlignment.CENTER));

            document.Close();
        }
    }
}