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
    public class CallReport
    {
        public async Task GenerateReport(string rootpath)
        {
            PageSize pg = PageSize.A4.Rotate();
            var helvetica = PdfFontFactory.CreateFont(StandardFontFamilies.HELVETICA);
            PdfWriter writer = new(rootpath + "/pdfs/callreport.pdf",
                               new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfDocument pdf = new(writer);
            Document document = new(pdf, pg);


            document.Add(new Image(ImageDataFactory.Create(rootpath + "/img/10306707.png"))
            .ScaleAbsolute(100, 100)
            .SetFixedPosition(((pg.GetWidth() - 100) / 2), 450));



            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("Current Calls")
                .SetFont(helvetica)
                .SetFontSize(24)
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER));
            document.Add(new Paragraph(""));
            document.Add(new Paragraph(""));
            Table table = new(6);
            table
                .SetWidth(298) // roughly 50% 
                .SetTextAlignment(TextAlignment.CENTER)
                .SetRelativePosition(0, 0, 0, 0)
                .SetHorizontalAlignment(HorizontalAlignment.CENTER);
            table.AddCell(new Cell().Add(new Paragraph("Opened")
            .SetFontSize(16)
            .SetBold()
            .SetPaddingLeft(18)
            .SetTextAlignment(TextAlignment.LEFT))
            .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("Last Name")
            .SetFontSize(16)
            .SetBold()
            .SetPaddingLeft(16)
            .SetTextAlignment(TextAlignment.LEFT))
            .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("Tech")
            .SetBold()
            .SetFontSize(16)
            .SetPaddingLeft(16)
            .SetTextAlignment(TextAlignment.LEFT))
            .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("Problem")
            .SetBold()
            .SetFontSize(16)
            .SetPaddingLeft(16)
            .SetTextAlignment(TextAlignment.LEFT))
            .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("Status")
            .SetBold()
            .SetFontSize(16)
            .SetMarginLeft(16)
            .SetTextAlignment(TextAlignment.LEFT))
            .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("Closed")
            .SetBold()
            .SetFontSize(16)
            .SetPaddingLeft(16)
            .SetTextAlignment(TextAlignment.LEFT))
            .SetBorder(Border.NO_BORDER));


            List<CallViewModel> allcall;
            CallViewModel callvm = new();
            allcall = await callvm.GetAll();

            List<EmployeeViewModel> allEmployeeVms;
            EmployeeViewModel vm = new();
            allEmployeeVms = await vm.GetAll();

            List<ProblemViewModel> allProblem;
            ProblemViewModel prbvm = new();
            allProblem = await prbvm.GetAll();
            // Print the list of students 

            foreach (var employee in allcall)
            {
                var employeename = allEmployeeVms.Find(a => a.Id == employee.EmployeeId);
                var techname = allEmployeeVms.Find(a => a.Id == employee.CustomerId);
                var problem = allProblem.Find(a => a.Id == employee.ProblemId);
                table.AddCell(new Cell().Add(new Paragraph(employee.DateOpened.ToShortDateString())
                .SetFontSize(14)
                .SetPaddingLeft(24)
                .SetTextAlignment(TextAlignment.LEFT))
                .SetBorder(Border.NO_BORDER));



                table.AddCell(new Cell().Add(new Paragraph(employeename.Lastname.ToString())
                .SetFontSize(14)
                .SetPaddingLeft(24)
                .SetTextAlignment(TextAlignment.LEFT))
                .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(techname.Lastname.ToString())
                .SetFontSize(14)
                .SetPaddingLeft(24)
                .SetTextAlignment(TextAlignment.LEFT))
                .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(problem.Description.ToString())
                .SetFontSize(14)
                .SetPaddingLeft(24)
                .SetWidth(200)
                .SetTextAlignment(TextAlignment.LEFT))
                .SetBorder(Border.NO_BORDER));

                if (employee.OpenStatus)
                {
                    table.AddCell(new Cell().Add(new Paragraph("Open")
                .SetFontSize(14)
                .SetPaddingLeft(24)
                .SetTextAlignment(TextAlignment.LEFT))
                .SetBorder(Border.NO_BORDER));
                }
                else
                {
                    table.AddCell(new Cell().Add(new Paragraph("Closed")
                .SetFontSize(14)
                .SetPaddingLeft(24)
                .SetTextAlignment(TextAlignment.LEFT))
                .SetBorder(Border.NO_BORDER));
                }

                if (employee.DateClosed == null)
                {
                    table.AddCell(new Cell().Add(new Paragraph("--")
                .SetFontSize(14)
                .SetPaddingLeft(24)
                .SetTextAlignment(TextAlignment.LEFT))
                .SetBorder(Border.NO_BORDER));
                }
                else
                {
                    table.AddCell(new Cell().Add(new Paragraph(employee.DateClosed?.ToShortDateString())
                .SetFontSize(14)
                .SetPaddingLeft(24)
                .SetTextAlignment(TextAlignment.LEFT))
                .SetBorder(Border.NO_BORDER));
                }

            }

            document.Add(table);
            document.Add(new Paragraph("Employee report written on - " + DateTime.Now)
            .SetFontSize(6)
            .SetTextAlignment(TextAlignment.CENTER));
            document.Close();
        }
    }
}