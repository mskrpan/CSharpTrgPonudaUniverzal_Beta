using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//za pdf
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
//za otvaranje
using System.Diagnostics;

namespace ZoranPonuda
{
    class PdfCreate
    {

        Crud crud = new Crud();
        
        BaseFont f_cb = BaseFont.CreateFont("c:\\windows\\fonts\\calibrib.ttf", BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        BaseFont f_cn = BaseFont.CreateFont("c:\\windows\\fonts\\calibri.ttf", BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

        public void Test(DataGridView view)
        {
            double d = Convert.ToDouble(view.Rows[14].Cells["Cijena"].Value);
            MessageBox.Show(""+d);
        }

        public void CreatePdf(DataGridView view, string ulica, string mjesto, string oib, int brponude)
        {
            Form1 f = new Form1();
            crud.PopunjavanjeTrgovinePdf(f.GetTrgovina[0]);
            int row = 0;
            string kupac = view.Rows[0].Cells["Kupac"].Value.ToString();

            Document document = new Document(PageSize.A4, 25, 25, 30, 1);
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream("D:\\Posao\\Ponude\\Ponuda_" + kupac + "_" + brponude + ".pdf", FileMode.Create));
            
            int page = 1;
           
            // otvara doc za upisivanje u njega
            document.Open();

            // omogućava upisivanje texta 
            // po X & Y parametrima.
            PdfContentByte cb = writer.DirectContent;
            // dodajemo footer
            cb.AddTemplate(PdfFooter(cb, page), 30, 1);

            // dodavanje slike
           /* iTextSharp.text.Image png = iTextSharp.text.Image.GetInstance(Server.MapPath("mbase_emc2.png"));
            png.ScaleAbsolute(200, 55);
            png.SetAbsolutePosition(40, 750);
            cb.AddImage(png);*/

            // prvo omogućimo pisanje texta
            cb.BeginText();

            // Naslovni dio
            writeText(cb, crud.GetTrgovina[0], 350, 820, f_cb, 14);
            writeText(cb, "OiB:", 350, 800, f_cb, 10);
            writeText(cb, Convert.ToString(crud.GetOib[0]), 420, 800, f_cn, 10);
            writeText(cb, "IBAN:", 350, 788, f_cb, 10);
            writeText(cb, crud.GetIban[0], 420, 788, f_cn, 10);
            writeText(cb, "Adresa:", 350, 776, f_cb, 10);
            writeText(cb, Convert.ToString(crud.GetUlicaTrg[0]), 420, 776, f_cn, 10);
            writeText(cb, "Mjesto:", 350, 764, f_cb, 10);
            writeText(cb, crud.GetMjesto[0], 420, 764, f_cn, 10);
            writeText(cb, "Mob Tel:", 350, 752, f_cb, 10);
            writeText(cb, crud.GetMob[0], 420, 752, f_cn, 10);


            // text primatelja pošiljeke
            int left_margin = 40;
            int top_margin = 720;
            writeText(cb, "Br. ponude: " + brponude, left_margin, top_margin, f_cb, 10);
            DateTime localDate = DateTime.Now;
            writeText(cb, "Datum: " + localDate.ToString("dd.MM.yyyy"), left_margin, top_margin - 12, f_cb, 10);
            writeText(cb, "Dostava: ", left_margin, top_margin-24, f_cb, 10);
            writeText(cb, kupac, left_margin, top_margin - 36, f_cn, 10);
            writeText(cb, ulica, left_margin, top_margin - 48, f_cn, 10);
            writeText(cb, mjesto, left_margin, top_margin - 60, f_cn, 10);
          

            // Podatci kupca
            left_margin = 40;
            top_margin = 620;
            writeText(cb, "Kupac", left_margin, top_margin, f_cb, 10);
            writeText(cb, kupac, left_margin, top_margin - 12, f_cn, 10);
            writeText(cb, "Ulica i k.br", left_margin + 100, top_margin, f_cb, 10);
            writeText(cb, ulica, left_margin + 100, top_margin - 12, f_cn, 10);
            writeText(cb, "Mjesto", left_margin +200, top_margin, f_cb, 10);
            writeText(cb, mjesto, left_margin + 200, top_margin - 12, f_cn, 10);
            writeText(cb, "Oib", left_margin + 300, top_margin, f_cb, 10);
            writeText(cb, oib, left_margin + 300, top_margin - 12, f_cn, 10);
            
            // ispod sljedeće dodavanje artikala
            left_margin = 40;
            top_margin = 575;
            writeText(cb, "Stavke", left_margin, top_margin, f_cb, 10);
            

            // endtext se dodaje prije dodavanja grafičkog djela
            cb.EndText();
            // odvajanje crtom s parametrima pozicije i veličine crte
            cb.SetLineWidth(0f);
            cb.MoveTo(40, 570);
            cb.LineTo(560, 570);
            cb.Stroke();
            // ponovno omogućavanje upisivanje texta
            cb.BeginText();

            // kritična točka gdje ćemo odvajat novu stranicu
            int lastwriteposition = 100;

            // naslovi tablice
            top_margin = 550;
            left_margin = 40;
            
            writeText(cb, "Šifra", left_margin, top_margin, f_cb, 10);
            writeText(cb, "Naziv robe", left_margin + 70, top_margin, f_cb, 10);
            writeText(cb, "J.mj.", left_margin + 280, top_margin, f_cb, 10);
            writeText(cb, "Kolicina", left_margin + 310, top_margin, f_cb, 10);
            writeText(cb, "PDV %", left_margin + 355, top_margin, f_cb, 10);
            writeText(cb, "Cijena", left_margin + 395, top_margin, f_cb, 10);
            writeText(cb, "Popust", left_margin + 435, top_margin, f_cb, 10);
            writeText(cb, "Iznos", left_margin + 480, top_margin, f_cb, 10);

            // prvi item počine odavdje
            top_margin = 538;

            // loopanje predmeta i upisivanje
            decimal zaPlatit = 0, sPorezom = 0, bezPoreza = 0, cijenaSamogPoreza = 0;
            for (int i = 1; i < view.Rows.Count; i++ )
            {
                writeText(cb, view.Rows[row].Cells["Sifra"].Value.ToString(), left_margin, top_margin, f_cn, 10);
                if (view.Rows[row].Cells["Naziv"].Value.ToString().Length > 20)
                {
                    writeText(cb, view.Rows[row].Cells["Naziv"].Value.ToString(), left_margin + 70, top_margin, f_cn, 8);
                    cb.SetFontAndSize(f_cn, 10);
                }
                else
                    writeText(cb, view.Rows[row].Cells["Naziv"].Value.ToString(), left_margin + 70, top_margin, f_cn, 10);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, view.Rows[row].Cells["Jm"].Value.ToString(), left_margin + 280, top_margin, 0);
                string kol = view.Rows[row].Cells["Kolicina"].Value.ToString();
                decimal kolicina = Convert.ToDecimal(kol);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, kol, left_margin + (310 + 15), top_margin, 0);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, view.Rows[row].Cells["Porez"].Value.ToString(), left_margin + (355 + 3), top_margin, 0);
                string pocCijena = view.Rows[row].Cells["Cijena"].Value.ToString();
                decimal novCijena = Convert.ToDecimal(pocCijena);
                decimal novaCijenaKol = novCijena * kolicina;
                bezPoreza += novaCijenaKol; 
                decimal cijena = Decimal.Multiply(novCijena, 1.25M);
                sPorezom += (cijena * kolicina);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, cijena.ToString("F2"), left_margin + 395, top_margin, 0);
                string rab = view.Rows[row].Cells["Rabat"].Value.ToString();
                decimal rabat = Convert.ToDecimal(rab);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, view.Rows[row].Cells["Rabat"].Value.ToString() + "%", left_margin + (435 + 8), top_margin, 0);
                
                decimal sPopIznos = Decimal.Multiply(cijena, rabat/100);
                
                decimal ukupno = (cijena - sPopIznos) * kolicina;
                zaPlatit += ukupno;
                writeText(cb, ukupno.ToString("F2"), left_margin + 480, top_margin, f_cn, 10);
                top_margin -= 12;
                row++;
                // page break tj novi stranica
                if (top_margin <= lastwriteposition)
                {
                    cb.EndText();
                    
                    // page break
                    document.NewPage();
                    cb.AddTemplate(PdfFooter(cb, ++page), 30, 1);    
                    
                    cb.BeginText();
                    // ponovnp postavljanje gornje margine
                    top_margin = 780;
                }
            }


            top_margin -= 80;
            left_margin = 350;

            // ukupno
            writeText(cb, "Ukupno bez poreza:", left_margin, top_margin, f_cb, 10);
            writeText(cb, "Iznos poreza (25%):", left_margin, top_margin - 12, f_cb, 10);
            writeText(cb, "Iznos popusta:", left_margin, top_margin - 24, f_cb, 10);
            writeText(cb, "Ukupno s porezom i popustom:", left_margin, top_margin - 48, f_cb, 10);
            // upisivanje rezultata desno
            left_margin = 540;
            
            cb.SetFontAndSize(f_cn, 10);
            string curr = "Kn";
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, curr, left_margin, top_margin, 0);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, curr, left_margin, top_margin - 12, 0);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, curr, left_margin, top_margin - 24, 0);
            left_margin = 535;
            cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, bezPoreza.ToString("F2"), left_margin, top_margin, 0);
            cijenaSamogPoreza = sPorezom - bezPoreza;
            cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, cijenaSamogPoreza.ToString("F2"), left_margin, top_margin - 12, 0);
            decimal samiRabat = bezPoreza + cijenaSamogPoreza - zaPlatit;
            cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, samiRabat.ToString("F2"), left_margin, top_margin - 24, 0);
            cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, zaPlatit.ToString("F2"), left_margin, top_margin - 48, 0);

            cb.EndText();

            // zatvaranje writera, documenta s fs
            document.Close();
            writer.Close();

            //otvaranje kreiranog pdf-a
            var process = new ProcessStartInfo();
            process.WorkingDirectory = "D:\\Posao\\Ponude";
            process.FileName = "Ponuda_" + kupac + "_" + brponude + ".pdf";
            Process.Start(process);

        }


        //metoda za upisivanje texta
        private void writeText(PdfContentByte cb, string Text, int X, int Y, BaseFont font, int Size)
        {
            cb.SetFontAndSize(font, Size);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, Text, X, Y, 0);
        }

        //footer
        private PdfTemplate PdfFooter(PdfContentByte cb, int num1)
        {
            // kreiranje template
            PdfTemplate tmpFooter = cb.CreateTemplate(580, 70);
            // doljnji lijevi čošak stranice
            tmpFooter.MoveTo(1, 1);
            // dodavanje footera
            tmpFooter.Stroke();
            // upisivanje u footer
            tmpFooter.BeginText();
            // postavljanje formatea i veličine
            tmpFooter.SetFontAndSize(f_cb, 8);
            // Info
            tmpFooter.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Dobavljac", 0, 53, 0);
            tmpFooter.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Adresa", 0, 45, 0);

            tmpFooter.SetFontAndSize(f_cn, 8);
            tmpFooter.ShowTextAligned(PdfContentByte.ALIGN_LEFT, crud.GetTrgovina[0], 50, 53, 0);
            tmpFooter.ShowTextAligned(PdfContentByte.ALIGN_LEFT, crud.GetUlicaTrg[0] + ", " + crud.GetMjesto[0], 50, 45, 0);

            //naslovi
            tmpFooter.SetFontAndSize(f_cb, 8);
            tmpFooter.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Tel/Mob", 215, 53, 0);
            tmpFooter.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Mail", 400, 53, 0);
            // info
            tmpFooter.SetFontAndSize(f_cn, 8);
            tmpFooter.ShowTextAligned(PdfContentByte.ALIGN_LEFT, crud.GetMob[0], 265, 53, 0);
            tmpFooter.ShowTextAligned(PdfContentByte.ALIGN_LEFT, crud.GetEmail[0], 450, 53, 0); 
            // kraj
            tmpFooter.EndText();
            // dodavanje linije iznad samog footera
            cb.SetLineWidth(0f);
            cb.MoveTo(30, 65);
            cb.LineTo(570, 65);
            cb.Stroke();
            return tmpFooter;
        }
    }

}
