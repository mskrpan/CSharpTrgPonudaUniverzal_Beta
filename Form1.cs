using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
    public partial class Form1 : Form
    {
        AutoCompleteStringCollection ColSifra, ColNaziv;
        Crud crud = new Crud();
        PdfCreate pdf = new PdfCreate();
        private string jm, sifra1, sifra2;
        private static List<string> trgovina = new List<string>();
        private static string sifraPon;

        public List<string> GetTrgovina { get { return trgovina; } }

        public Form1()
        {
            InitializeComponent();
            Crud.Init();
            crud.ReadKupac();
            FillCombo();
            ClearAutoCollection();
            CreateAutoCollection();
            
            //odmah prikazuje bazu
            crud.ReadArtiklDataGridView(dgvNoviArtikli);
            PromjenaColm();
            //postavljanje checkbuttona i buttona
            onLoadPostavke();
            //popunjavanje trgovina
            Crud.ReadTrgovina(cbTrgovina);
        }

        //clear autocomplate
        public void ClearAutoCollection()
        {
            if (ColSifra != null)
                ColSifra.Clear();
            if (ColNaziv != null)
                ColNaziv.Clear();
            
        }
        //popuni autocomplete
        public void CreateAutoCollection()
        {
            ColSifra = new AutoCompleteStringCollection();
            ColNaziv = new AutoCompleteStringCollection();

            crud.ReadArtikl(ColSifra, ColNaziv, txNaziv.Text);

            txSifra.AutoCompleteCustomSource = ColSifra;
            txNaziv.AutoCompleteCustomSource = ColNaziv;
        }

        //popunjavanje komboa
        public void FillCombo()
        {
            foreach (string item in crud.GetKupac)
            {
                cbKupac.Items.Add(item);
            }
        }
        //dodavanje kupca
        private void btDodajKupca_Click(object sender, EventArgs e)
        {

            if ((txOib.Text).Length == 11)
            {
                if(!String.IsNullOrWhiteSpace(txKupac.Text) 
                    && !String.IsNullOrWhiteSpace(txUlica.Text) 
                    && !String.IsNullOrWhiteSpace(txMjesto.Text))
                {
                    Crud.SaveKupac(txKupac.Text, txUlica.Text, txMjesto.Text, txOib.Text);
                    crud.RemoveLists();
                    cbKupac.Items.Clear();
                    crud.ReadKupac();
                    FillCombo();
                    ClearAutoCollection();
                    CreateAutoCollection();
                    ClearTextKupac();
                    txBrponude.Text = Convert.ToString(crud.ReadZadnjiBrPonude());
                    MessageBox.Show("Kupac dodan");
                }
                else{MessageBox.Show("Molim da se popune sva polja.");}
            }
            else { MessageBox.Show("Oib je manji/veći od 11 znamenki, molim provjeriti i popraviti."); }
        }

        //brisanje nakon unosa kupca
        private void ClearTextKupac()
        {
            txKupac.Clear();
            txUlica.Clear();
            txMjesto.Clear();
            txOib.Clear();
        }
        
        //brisanje nakon unosa ponude
        private void ClearTextPonude()
        {
            txSifra.Clear();
            txNaziv.Clear();
            numericUpDown.Value = 0;
        }
        
        //brisanje nakon kreiranja pdf
        private void ClearTextPonudePdf()
        {
            cbKupac.SelectedIndex = - 1;
            txSifra.Clear();
            txNaziv.Clear();
            numericUpDown.Value = 0;
        }

        //save ponude
        private void btArtikl_Click(object sender, EventArgs e)
        {
            btPDF.Enabled = true;

            if (!string.IsNullOrWhiteSpace(txNaziv.Text)
                && !string.IsNullOrWhiteSpace(cbTrgovina.Text)
                && !string.IsNullOrWhiteSpace(txSifra.Text)
                && !string.IsNullOrWhiteSpace(cbRabat.Text)
                && !string.IsNullOrWhiteSpace(cbKupac.Text)
                && numericUpDown.Value != 0)
            {
                DateTime dateAndTime = dateTimePicker.Value.Date;
                crud.ReadKupacZaPonudu(cbKupac.Text);
                string ulica = crud.GetOibMjestoUlica[0];
                string mjesto = crud.GetOibMjestoUlica[1];
                string oib = crud.GetOibMjestoUlica[2];
                //dodaj cjenu mamlaze
                decimal cijena = Crud.CijenaArtikla(txSifra.Text);
                Crud.SavePonuda(cbKupac.Text, txBrponude.Text, txSifra.Text, txNaziv.Text, numericUpDown.Value.ToString(), "KOM", ulica, mjesto, cbRabat.Text, "25,00", Convert.ToString(cijena), dateAndTime.ToString("dd.MM.yyyy"), oib);
                crud.RemoveOibMjestoUlica();
                int brPonude = Convert.ToInt32(txBrponude.Text);
                Crud.ReadPonuda(dgvPonuda, cbKupac.Text, brPonude, dateAndTime.ToString("yyyy-MM-dd"));
                createDgvPonude();
                ClearTextPonude();
            }
            else { MessageBox.Show("Molim popunite sve"); }
        }

        //kreiranje datagridview ponude
        private void createDgvPonude() 
        {
            dgvPonuda.ReadOnly = true;
            dgvPonuda.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPonuda.Columns["Br_ponude"].Width = 50;
            dgvPonuda.Columns["Sifra"].Width = 50;
            dgvPonuda.Columns["Naziv"].Width = 260;
            dgvPonuda.Columns["Kolicina"].Width = 50;
            dgvPonuda.Columns["Rabat"].Width = 50;
            dgvPonuda.Columns["Porez"].Width = 50;
            dgvPonuda.Columns["Cijena"].Width = 50;
            dgvPonuda.Columns["Cijena"].DefaultCellStyle.Format = "0.00##";
            dgvPonuda.Columns["Jm"].Width = 40;
            dgvPonuda.Columns["Datum"].Width = 63;
        }
        //kreiranje pdf
        private void btPDF_Click(object sender, EventArgs e)
        {
            if (dgvPonuda.Rows.Count != 0)
            {
                //dodaje trg u pdf
                trgovina.Add(cbTrgovina.Text);
                
                crud.ReadKupacZaPonudu(cbKupac.Text);
                string ulica = crud.GetOibMjestoUlica[0];
                string mjesto = crud.GetOibMjestoUlica[1];
                string oib = crud.GetOibMjestoUlica[2];
                int brPonude = Convert.ToInt32(txBrponude.Text);

                pdf.CreatePdf(dgvPonuda, ulica, mjesto, oib, brPonude);
                crud.RemoveOibMjestoUlica();
                txBrponude.Text = Convert.ToString(crud.ReadZadnjiBrPonude() + 1);
                ClearTextPonudePdf();
                this.dgvPonuda.DataSource = null;
                btPDF.Enabled = false;
                trgovina.Clear();
                crud.RemoveTrgovina();
            }
            else { MessageBox.Show("Dodaj artikle u ponudu."); }
        }

        //kontrola br ponude
        private void cbKupac_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (txBrponude.Text != "0")
            {
                txBrponude.Text = Convert.ToString(crud.ReadZadnjiBrPonude() + 1);
            }
            else
                txBrponude.Text = "1";
        }

        //popunjavanje sifre ukoliko je artikl pronađen
        private void CompleteSifru(object sender, EventArgs e) 
        {
            txSifra.Text = Convert.ToString(crud.PopunjavanjeSifre(txNaziv.Text));
        }

        private void btTest2_Click(object sender, EventArgs e)
        {

            //decimal cc = Crud.CijenaArtikla(txSifra.Text);
            //MessageBox.Show(""+cc);
            /*string str = cbRabat.Text;
            switch(str)
            {
                case "0%":
                    MessageBox.Show("0 posto matereti");
                    break;
                case "1%":
                    MessageBox.Show("1 posto");
                    break;
            }*/
            decimal Mpc;
            decimal vpc = 100M;
            decimal pdv = 1.25M;
            Mpc = Decimal.Multiply(vpc, pdv);
            MessageBox.Show("REz: " + Mpc);

           // trgovina.Add(cbTrgovina.Text);
           // MessageBox.Show(trgovina[0]);
           // Crud crud = new Crud();
           /* crud.PopunjavanjeTrgovinePdf(cbTrgovina.Text);

            MessageBox.Show(crud.GetTrgovina[0] +"||"+ crud.GetMatBr[0]+"||"+ crud.GetIban[0]+"||"+ crud.GetOib[0]+"||"+crud.GetMob[0]);
            crud.RemoveTrgovina();
            trgovina.Clear();*/
        }

        private void PromjenaColm()
        {
            dgvNoviArtikli.ReadOnly = true;
            dgvNoviArtikli.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvNoviArtikli.Columns["Id"].Width = 50;
            dgvNoviArtikli.Columns["Sifra"].Width = 50;
            dgvNoviArtikli.Columns["Naziv"].Width = 338;
            dgvNoviArtikli.Columns["Jm"].Width = 50;
            dgvNoviArtikli.Columns["Mpc"].Width = 75;
            dgvNoviArtikli.Columns["Vpc"].Width = 75;
            dgvNoviArtikli.Columns["Porez"].Width = 30;
            dgvNoviArtikli.Columns["Mpc"].DefaultCellStyle.Format = "0.00##";
            dgvNoviArtikli.Columns["Vpc"].DefaultCellStyle.Format = "0.00##";
        }

        private void txPretraga_TextChanged(object sender, EventArgs e)
        {
            crud.SearchArtiklDataGridView(dgvNoviArtikli, txPretraga.Text);
        }
        
        //popunjavanje na klik misa
        private void dgvNoviArtikli_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            btIzmjena.Enabled = true;
            btDeleteArtikl.Enabled = true;
            btSacuvaj.Enabled = false;
            dgvNoviArtikli = sender as DataGridView;
            DataGridViewRow row;
            if (e.RowIndex < 0)
                row = dgvNoviArtikli.Rows[0];
            else
            {
                row = dgvNoviArtikli.Rows[e.RowIndex];
                if (dgvNoviArtikli == null)
                { return; }
                else
                {

                    sifra2 = row.Cells["Sifra"].Value.ToString();
                    jm = row.Cells["Jm"].Value.ToString();
                    string artiklUpdate = row.Cells["Naziv"].Value.ToString();
                    string cijenaUpdate = row.Cells["Vpc"].Value.ToString();
                    if (jm == "M")
                    {
                        checkBoxM.Checked = true;
                        checkBoxKOM.Checked = false;
                        checkBoxM2.Checked = false;
                    }
                    else if (jm == "KOM")
                    {
                        checkBoxKOM.Checked = true;
                        checkBoxM.Checked = false;
                        checkBoxM2.Checked = false;
                    }
                    else if (jm == "M2")
                    {
                        checkBoxM2.Checked = true;
                        checkBoxM.Checked = false;
                        checkBoxKOM.Checked = false;
                    }
                    else
                    {
                        MessageBox.Show("JM nije ni M ni KOM ni M2");
                        checkBoxM.Checked = false;
                        checkBoxKOM.Checked = false;
                        checkBoxM2.Checked = false;
                    }

                    int rowNum = e.RowIndex;

                    txNoviArtikl.Text = artiklUpdate;
                    Double value = Convert.ToDouble(cijenaUpdate);
                    txNovaCijena.Text = String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0}", value);
                }
            }
        }

        private void btSacuvaj_Click(object sender, EventArgs e)
        {
            string cbJm;
            if (!string.IsNullOrWhiteSpace(txNoviArtikl.Text) && !string.IsNullOrWhiteSpace(txNovaCijena.Text) && (checkBoxKOM.Checked == true || checkBoxM.Checked == true))
            {
                if (checkBoxM.Checked)
                    cbJm = "M";
                else if (checkBoxKOM.Checked)
                    cbJm = "KOM";
                else if (checkBoxM2.Checked)
                    cbJm = "M2";
                else
                    cbJm = "";
                sifra1 = Convert.ToString((crud.ReadSifraNoviArtikl() + 1));
                Crud.SaveArtikl(Convert.ToInt32(sifra1), txNoviArtikl.Text, cbJm, txNovaCijena.Text, "25,00");
                //refresh tablice
                crud.ReadArtiklDataGridView(dgvNoviArtikli);
                //ponovo dodaje u autokolekcion za dodavanje u ponudu
                ClearAutoCollection();
                CreateAutoCollection();
                ClearAllNewArtikl();
                MessageBox.Show("Artikl je spremljen!");
            }
            else
                MessageBox.Show("Molim popunite Naziv artikla i cijunu!");
        }

        private void btIzmjena_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txNoviArtikl.Text) && !string.IsNullOrWhiteSpace(txNovaCijena.Text) && (checkBoxKOM.Checked == true || checkBoxM.Checked == true))
            {
                string cbJm;
                if (checkBoxM.Checked)
                    cbJm = "M";
                else if (checkBoxKOM.Checked)
                    cbJm = "KOM";
                else if (checkBoxM2.Checked)
                    cbJm = "M2";
                else
                    cbJm = "";

                Crud.ChangeArtikl(Convert.ToInt32(sifra2), txNoviArtikl.Text, cbJm, txNovaCijena.Text);
                //refresh tablice
                crud.ReadArtiklDataGridView(dgvNoviArtikli);
                ClearAutoCollection();
                CreateAutoCollection();
                MessageBox.Show("Artikl je izmjenjen i spremljen!");
                ClearAllNewArtikl();
                btSacuvaj.Enabled = true;
                btIzmjena.Enabled = false;
                btDeleteArtikl.Enabled = false;
            }
            else
                MessageBox.Show("Molim popunite Naziv artikla i cijunu!");
        }

        private void checkBoxM_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxM.Checked)
            {
                checkBoxKOM.Checked = false;
                checkBoxM2.Checked = false;
            }
        }

        private void checkBoxKOM_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxKOM.Checked)
            {
                checkBoxM.Checked = false;
                checkBoxM2.Checked = false;
            }
        }

        private void checkBoxM2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxM2.Checked)
            {
                checkBoxM.Checked = false;
                checkBoxKOM.Checked = false;
            }
        }

        private void onLoadPostavke() 
        {
            btIzmjena.Enabled = false;
            btDeleteArtikl.Enabled = false;
            checkBoxM.Checked = true;
            btPDF.Enabled = false;
            btDeletePonuda.Enabled = false;
        }

        private void ClearAllNewArtikl() 
        {
            txNoviArtikl.Text = null;
            txNovaCijena.Text = null;
            txPretraga.Text = null;
        }

        private void btNewTrgovina_Click(object sender, EventArgs e)
        {
                if ((txOibTrg.Text).Length == 11)
                {
                    if (!String.IsNullOrWhiteSpace(txNewTrgovina.Text)
                        && !String.IsNullOrWhiteSpace(txIban.Text)
                        && !String.IsNullOrWhiteSpace(txMob.Text)
                        && !String.IsNullOrWhiteSpace(txUlicaTrg.Text)
                        && !String.IsNullOrWhiteSpace(txMjestoTrg.Text))
                    {
                        Crud.NewTrgovina(txNewTrgovina.Text, txUlicaTrg.Text, Convert.ToInt64(txOibTrg.Text), txIban.Text, txMob.Text, txMjestoTrg.Text, txEmail.Text);
                        cbTrgovina.Items.Clear();
                        Crud.ReadTrgovina(cbTrgovina);
                        ClearTrgovineText();
                        MessageBox.Show("Trgovina dodana!");
                    }
                    else { MessageBox.Show("Niste popunli sve molim da se sve ispuni"); }
                }
                else { MessageBox.Show("Oib je manji/veći od 11 znamenki, molim promjeniti."); }
        }

        private void ClearTrgovineText()
        {
            txNewTrgovina.Clear();
            txOibTrg.Clear();
            txMob.Clear();
            txIban.Clear();
            txUlicaTrg.Clear();
            txMjestoTrg.Clear();
            txEmail.Clear();
        }

        private void btDeleteArtikl_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Jeste li sigurni da želite obrisati artikl?", "Provjera", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                Crud.DeleteArtikl(txNoviArtikl.Text);
                crud.ReadArtiklDataGridView(dgvNoviArtikli);
                btSacuvaj.Enabled = true;
                btIzmjena.Enabled = false;
                btDeleteArtikl.Enabled = false;
            }
            else if (result == DialogResult.No)
            {
                // nešto
            }
        }

        private void dgvPonuda_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            btDeletePonuda.Enabled = true;
            btArtikl.Enabled = false;
            dgvPonuda = sender as DataGridView;
            DataGridViewRow row;
            if (e.RowIndex < 0)
                row = dgvPonuda.Rows[0];
            else
            {
                row = dgvPonuda.Rows[e.RowIndex];
                if (dgvPonuda == null)
                { return; }
                else
                {
                    sifraPon = row.Cells["Sifra"].Value.ToString();
                }
            }
        }

        private void btDeletePonuda_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Jeste li sigurni da želite obrisati artikl iz ponude?", "Provjera", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                Crud.DeletePonudu(sifraPon);
                Crud.ReadAfterDeletePonudu(txBrponude.Text, dgvPonuda);
                btDeletePonuda.Enabled = false;
                btArtikl.Enabled = true;
            }
            else if (result == DialogResult.No)
            {
                // nešto
            }
        }

        private void groupBox1_MouseCaptureChanged(object sender, EventArgs e)
        {
            dgvNoviArtikli.ClearSelection();
            ClearAllNewArtikl();
            btSacuvaj.Enabled = true;
            btIzmjena.Enabled = false;
            btDeleteArtikl.Enabled = false;


            dgvPonuda.ClearSelection();
            ClearTextPonude();
            btDeletePonuda.Enabled = false;
            btArtikl.Enabled = true;
        }

    }
}
