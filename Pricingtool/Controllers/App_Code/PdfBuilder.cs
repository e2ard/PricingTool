﻿using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.Linq;

namespace PricingTool.MVC.Controllers.App_Code
{
    public class PdfBuilder : IDisposable
    {
        private PdfPTable table;
        private string documentTitle, city;
        public string PATH, fileName;
        private int puYear, puMonth, puDay;
        public SiteBase SiteName;
        private int dayNum;
        private Document doc;
        private DateTime doDate;
        private Font font = FontFactory.GetFont("Arial", 8, Font.NORMAL);
        private string suppliers = string.Empty;

        public PdfBuilder(SiteBase site)
        {
            if (site != null)
            {
                this.SiteName = site;
                SetDocumentDetails();

                PATH = HttpContext.Current.Server.MapPath("~/");
                fileName = "\\pdf\\" + documentTitle + puMonth + "-" + puDay + city;
                FileStream fs;
                try
                {
                    fs = new FileStream(PATH + fileName + ".pdf", FileMode.Create, FileAccess.Write, FileShare.None);
                }
                catch(IOException ioe)
                {
                    fs = new FileStream(PATH +  fileName + "(1).pdf", FileMode.Create, FileAccess.Write, FileShare.None);
                }
                fileName += ".pdf";
                doc = new Document(PageSize.A4, 10, 10, 10, 10);
                doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
                PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                doc.Open();

                doDate = new DateTime(puYear, puMonth, puDay);
                dayNum = 1;
                AddSuppliers("GREEN MOTION");
            }
            else
            {
                throw new Exception("Site is null");
            }
        }

        public PdfBuilder()
        {
        }

        public void Close()
        {
            doc.Add(table);
            GetSuppliers();
            doc.Close();
        }

        public void SetDocumentDetails()
        {
            if (SiteName != null)
            {
                documentTitle = SiteName.GetTitle();
                city = SiteName.GetCity();
                int.TryParse(SiteName.GetPuDay(), out puDay);
                int.TryParse(SiteName.GetPuMonth(), out puMonth);
                int.TryParse(SiteName.GetPuYear(), out puYear);
            }
            else
            {
                Debug.WriteLine("SITENAME IS EMPTY or INCORRECT");
            }
        }

        public void CreateHeaders()
        {
            int colSpan = Const.categories.Count + 1;
            table = new PdfPTable(colSpan);
            //fix the absolute width of the table
            table.LockedWidth = true;

            //relative col widths in proportions - 1/3 and 2/3

            float[] widths = new float[colSpan];

            for (int i = 0; i < colSpan; i++)
            {
                widths[i] = 2f;
            }
            widths[0] = 1.5f;
            widths[1] = 2.5f;

            table.SetWidths(widths);

            table.HorizontalAlignment = 0;

            //leave a gap before and after the table

            table.SpacingBefore = 10f;
            table.SpacingAfter = 30f;
            table.TotalWidth = doc.Right - doc.Left;

            Font font1 = FontFactory.GetFont("Arial", 9, Font.NORMAL);
            PdfPCell cell = new PdfPCell(new Phrase(documentTitle + " " + puMonth + "-" + puDay + " " + city, font1));

            cell.Colspan = colSpan;

            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right

            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date", font));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            table.AddCell(cell);

            for (int i = 0; i < Const.categories.Count; i++)
            {
                cell = new PdfPCell(new Phrase(Const.categories.ElementAt(i).PdfClass.ToLower(), font));
                table.AddCell(cell);
            }

        }

        public void addRow(JOffer[] offers)
        {
            PdfPCell cell = new PdfPCell(new Phrase(puMonth + "-" + puDay + "/" + doDate.AddDays(dayNum).Day + "\n" + dayNum, font));
            dayNum++;
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            table.AddCell(cell);

            for (int i = 0; i < offers.Length; i++)
            {
                cell = new PdfPCell(new Phrase(offers[i].GetOffer(), font));

                var chunk = new Chunk(offers[i].GetOffer(), font);
                AddSuppliers(offers[i].GetSupplier());
                if(offers[i].GetBest() != null)
                    AddSuppliers(offers[i].GetBest());
                if (offers[i].GetCR() != null)
                    AddSuppliers(offers[i].GetCR());
                if (i == 0)
                    chunk.SetAnchor(offers[i].GetSiteName());
                cell.AddElement(chunk);
                if (offers[i].price > offers[i].gmPrice)
                {
                    cell.BackgroundColor = BaseColor.BLUE;
                    if (offers[i].price - offers[i].gmPrice < 2.5f && offers[i].price - offers[i].gmPrice > 0)
                    {
                        cell.BackgroundColor = BaseColor.ORANGE;
                        if (offers[i].price - offers[i].gmPrice < 1.5f && offers[i].price - offers[i].gmPrice > 0)
                            cell.BackgroundColor = BaseColor.GREEN;

                    }
                }
                else
                {
                    cell.BackgroundColor = BaseColor.RED;
                }

                table.AddCell(cell);
            }
            table.CompleteRow();
        }

        public void GetSuppliers()
        {
            doc.Add(new Paragraph(suppliers, FontFactory.GetFont("Arial", 9, Font.NORMAL)));
        }

        public void AddSuppliers(string supplier)
        {
            if (supplier != null && !suppliers.Contains(supplier))
                suppliers += supplier + "- " + (supplier.Length > 3 ? supplier.ToLower().Substring(0, 4) : supplier.ToLower().Substring(0, 3)) + "\n";
        }

        public void Dispose()
        {
            doc.Add(table);
            GetSuppliers();
            doc.Close();
        }
    }
}