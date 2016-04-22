using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data;

namespace ZoranPonuda
{
    
    class Crud
    {
        private static string conStr;
        private static SqlConnection connection;

        //kupac
        private List<string> Kupac = new List<string>();
        private List<string> OibMjestoUlica = new List<string>();
        //trgovina
        private static List<string> Trgovina = new List<string>();
        private List<string> UlicaTrg = new List<string>();
        private List<long> Oib = new List<long>();
        private List<string> Iban = new List<string>();
        private List<string> Mob = new List<string>();
        private List<string> Mjesto = new List<string>();
        private List<string> Email = new List<string>();

        //kupac
        public List<string> GetKupac { get { return Kupac; } }
        public List<string> GetOibMjestoUlica { get { return OibMjestoUlica; } }

        //trgovine
        public List<string> GetTrgovina { get { return Trgovina; } }
        public List<string> GetUlicaTrg { get { return UlicaTrg; } }
        public List<long> GetOib { get { return Oib; } }
        public List<string> GetIban { get { return Iban; } }
        public List<string> GetMob { get { return Mob; } }
        public List<string> GetMjesto { get { return Mjesto; } }
        public List<string> GetEmail { get { return Email; } }

        public void RemoveLists()
        {
            Kupac.Clear();
        }
        public void RemoveOibMjestoUlica() 
        {
            OibMjestoUlica.Clear();
        }

        public void RemoveTrgovina()
        {
            Trgovina.Clear();
            UlicaTrg.Clear();
            Oib.Clear();
            Iban.Clear();
            Mob.Clear();
            Mjesto.Clear();
            Email.Clear();
        }

        public static void Init()
        {
            conStr = ConfigurationManager.ConnectionStrings["ZoranPonuda.Properties.Settings.ArtikliConnectionString"].ConnectionString;
        }

        //ucitavanje artikla
        public void ReadArtikl(AutoCompleteStringCollection col, AutoCompleteStringCollection col1, string str)
        {
            string quarry = "SELECT * FROM [dbo].[Artikli] WHERE Naziv LIKE '%"+str+"%';";
            using (connection = new SqlConnection(conStr))
            using (SqlCommand com = new SqlCommand(quarry, connection))
            {
                connection.Open();
                SqlDataReader reader = com.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        int Sifra = reader.GetInt32(1);
                        string sifra = Convert.ToString(Sifra);
                        col.Add(sifra);
                        string naziv = reader.GetString(2);
                        col1.Add(naziv);
                    }
                    reader.Close();
                }
                catch (Exception) { MessageBox.Show("Greška u čitanju artikla"); }
            }
        }


        //save kupca
        public static void SaveKupac(string Kupac, string Ulica, string Mjesto, string Oib)
        {
            string quarry = "INSERT INTO  [dbo].[Kupci] ([Kupac],[Ulica],[Mjesto],[Oib]) VALUES (@Kupac, @Ulica, @Mjesto, @Oib);";

            using (connection = new SqlConnection(conStr))
            using (SqlCommand com = new SqlCommand(quarry, connection))
            {
                connection.Open();
                try
                {
                    com.Parameters.AddWithValue("@Kupac", Kupac);
                    com.Parameters.AddWithValue("@Ulica", Ulica);
                    com.Parameters.AddWithValue("@Mjesto", Mjesto);
                    com.Parameters.AddWithValue("@Oib", Oib);
                    com.ExecuteNonQuery();
                }
                catch (Exception) { MessageBox.Show("Greška u save DBKupca!"); }
            }
        }

        //ucitavanje kupca za combobox
        public void ReadKupac()
        {
            string quarry = "SELECT * FROM [dbo].[Kupci];";
            using (connection = new SqlConnection(conStr))
            using (SqlCommand com = new SqlCommand(quarry, connection))
            {
                connection.Open();
                SqlDataReader reader = com.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        Kupac.Add(reader.GetString(1));
                    }
                    reader.Close();
                }
                catch (Exception) { MessageBox.Show("Greška u čitanju kupca"); }
            }
        }

        //uzimanje cijena artikla
        public static decimal CijenaArtikla(string Sifra) 
        {
            decimal cijena = 0;
            string quarry = "SELECT * FROM [dbo].[Artikli] WHERE Sifra = @Sifra;";
            using (connection = new SqlConnection(conStr))
            using (SqlCommand com = new SqlCommand(quarry, connection))
            {
                connection.Open();
                com.Parameters.AddWithValue("@Sifra", Sifra);
                SqlDataReader reader = com.ExecuteReader();
                try
                {
                    
                    while (reader.Read())
                    {
                        cijena = reader.GetDecimal(5);
                    }
                    reader.Close();
                }
                catch (Exception) { MessageBox.Show("Greška u čitanju artikla"); }
            }
            return cijena;
        }

        //save ponude
        public static void SavePonuda(string Kupac, string Br_ponude, string Sifra,
            string Naziv, string Kolicina, string Jm, string Ulica, string Mjesto, string Rabat, string Porez, string Cijena, string Datum, string Oib)
        {
            string quarry = "INSERT INTO  [dbo].[Ponude] ([Kupac],[Br_ponude],[Sifra],[Naziv],[Jm],[Ulica],[Mjesto],[Kolicina], [Rabat], [Porez],"
                + "[Cijena],[Datum],[Oib]) VALUES (@Kupac, @Br_ponude, @Sifra, @Naziv, @Jm, @Ulica, @Mjesto, @Kolicina, @Rabat, @Porez, @Cijena, @Datum, @Oib);";

            using (connection = new SqlConnection(conStr))
            using (SqlCommand com = new SqlCommand(quarry, connection))
            {
                connection.Open();
                try
                {
                    com.Parameters.AddWithValue("@Kupac", Kupac);
                    int br = Convert.ToInt32(Br_ponude);
                    com.Parameters.AddWithValue("@Br_ponude", br);
                    int sf = Convert.ToInt32(Sifra);
                    com.Parameters.AddWithValue("@Sifra", sf);
                    com.Parameters.AddWithValue("@Naziv", Naziv);
                    com.Parameters.AddWithValue("@Jm", Jm);
                    com.Parameters.AddWithValue("@Ulica", Ulica);
                    com.Parameters.AddWithValue("@Mjesto", Mjesto);
                    int ko = Convert.ToInt32(Kolicina);
                    com.Parameters.AddWithValue("@Kolicina", ko);
                    com.Parameters.AddWithValue("@Rabat", Rabat);
                    decimal po = Convert.ToDecimal(Porez);
                    com.Parameters.AddWithValue("@Porez", po);
                    decimal mon = Convert.ToDecimal(Cijena);
                    com.Parameters.AddWithValue("@Cijena", mon);
                    DateTime dt = Convert.ToDateTime(Datum);
                    com.Parameters.AddWithValue("@Datum", dt);
                    com.Parameters.AddWithValue("@Oib", Oib);
                    com.ExecuteNonQuery();
                }
                catch (Exception) { MessageBox.Show("Greška u save Ponude!"); }
            }
        }
        //delete ponude ----------------------------
        public static void DeletePonudu(string sifra) 
        {
            string querry = "DELETE FROM [dbo].[Ponude] WHERE Sifra = @sifra";
            using (connection = new SqlConnection(conStr))
            using (SqlCommand com = new SqlCommand(querry, connection)) 
            {
                connection.Open();
                com.Parameters.AddWithValue("@sifra",sifra);
                com.ExecuteNonQuery();
            }
        }
        
        //read ponude --------------------------------
        public static void ReadAfterDeletePonudu(string BrPonude, DataGridView view)
        {
            string quarry = "SELECT * FROM [dbo].[Ponude] WHERE Br_ponude = '"+BrPonude+"';";
            using (connection = new SqlConnection(conStr))
            using (SqlDataAdapter adapter = new SqlDataAdapter(quarry, connection))
            {
                DataTable table = new DataTable();
                adapter.Fill(table);
                view.DataSource = table;
                adapter.Update(table);
            }
        }
        //prikaz na datagridview ponude
        public static void ReadPonuda(DataGridView view, string kupac, int brPonude, string datum)
        {
            string quarry = "SELECT [Kupac],[Br_ponude],[Sifra],[Naziv],[Kolicina], [Rabat], [Porez], [Cijena],[Jm],[Datum] "
                + "FROM [dbo].[Ponude] WHERE Kupac = '" + kupac + "' AND Br_ponude = " + brPonude + "AND Datum = '" + datum + "';";


            // AND Datum = "+datum+"
            using (connection = new SqlConnection(conStr))
            using (SqlDataAdapter adapter = new SqlDataAdapter(quarry, connection))
            {
                DataTable table = new DataTable();
                adapter.Fill(table);
                view.DataSource = table;
                adapter.Update(table);
            }

        }

        //ucitavanje kupca oib, mjesto, ulica
        public void ReadKupacZaPonudu(string kupac)
        {
            string quarry = "SELECT * FROM [dbo].[Kupci] WHERE Kupac = '"+kupac+"';";
            using (connection = new SqlConnection(conStr))
            using (SqlCommand com = new SqlCommand(quarry, connection))
            {
                connection.Open();
                SqlDataReader reader = com.ExecuteReader();
                try
                {
                    reader.Read();
                       
                        OibMjestoUlica.Add(reader.GetString(2));
                        OibMjestoUlica.Add(reader.GetString(3));
                        OibMjestoUlica.Add(reader.GetString(4));
                      
                    reader.Close();
                }
                catch (Exception) { MessageBox.Show("Greška u čitanju kupca"); }
            }
        }

        //SELECT LAST(column_name) FROM table_name;
        //ucitavanje kupca oib, mjesto, ulica
        public int ReadZadnjiBrPonude()
        {
            int brPonude = 0;
            string quarry = "SELECT TOP 1 * FROM [dbo].[Ponude] ORDER BY Br_ponude DESC";
            using (connection = new SqlConnection(conStr))
            using (SqlCommand com = new SqlCommand(quarry, connection))
            {
                connection.Open();
                SqlDataReader reader = com.ExecuteReader();
                try
                {
                    reader.Read();
                    brPonude = reader.GetInt32(2);
                    reader.Close();
                }
                catch (Exception) { Console.WriteLine("Greška u čitanju kupca"); }
            }
            return brPonude;
        }

        //popunjavanje sifre ukoliko je artikl pronađen
        public int PopunjavanjeSifre(string naziv)
        {
            int sifra = 0;
            string quarry = "SELECT * FROM [dbo].[Artikli] WHERE Naziv = '" + naziv + "' COLLATE Latin1_General_CI_AI;";
            using (connection = new SqlConnection(conStr))
            using (SqlCommand com = new SqlCommand(quarry, connection))
            {
                connection.Open();
                SqlDataReader reader = com.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        sifra = reader.GetInt32(1);
                    }
                    reader.Close();
                }
                catch (Exception) { MessageBox.Show("Greška u otkrivanju Sifre"); }
            }
            return sifra;
        }

        //popunjavanje artikla ukoliko je sifra pronadjena
        //trenutno ne koristim
        public string PopunjavanjeNaziva(int sifra)
        {
            string naziv = null;
            string quarry = "SELECT * FROM [dbo].[Artikli] WHERE Sifra = " + sifra + ";";
            using (connection = new SqlConnection(conStr))
            using (SqlCommand com = new SqlCommand(quarry, connection))
            {
                connection.Open();
                SqlDataReader reader = com.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        naziv = reader.GetString(2);
                    }
                    reader.Close();
                }
                catch (Exception) { MessageBox.Show("Greška u otkrivanju Naziva"); }
            }
            return naziv;
        }
        //------------------------------------------------------------DODAVANJE ---------------------------------------------------
        //dodavanje novih artikala u bazu
        public static void SaveArtikl(int Sifra, string Naziv, string Jm, string Vpc, string Porez)
        {
            string quarry = "INSERT INTO [dbo].[Artikli] ([Sifra],[Naziv],[Jm],[Mpc],[Vpc],[Porez]) VALUES (@Sifra, @Naziv, @Jm, @Mpc, @Vpc, @Porez);";
            decimal Mpc;
            decimal vpc = Convert.ToDecimal(Vpc);
            decimal pdv = 1.25M;
            Mpc = Decimal.Multiply(vpc, pdv);
            using (connection = new SqlConnection(conStr))
            using (SqlCommand com = new SqlCommand(quarry, connection))
            {
                connection.Open();
                try
                {
                    //riješit kako dobit šifru
                    com.Parameters.AddWithValue("@Sifra", Sifra);
                    com.Parameters.AddWithValue("@Naziv", Naziv);
                    com.Parameters.AddWithValue("@Jm", Jm);
                    com.Parameters.AddWithValue("@Mpc", Mpc);
                    com.Parameters.AddWithValue("@Vpc", Convert.ToDecimal(Vpc));
                    com.Parameters.AddWithValue("@Porez", Convert.ToDecimal(Porez));
                    com.ExecuteNonQuery();
                }
                catch (Exception) { MessageBox.Show("Greška u save Artikla!"); }
            }
        }

        //izmjena artikala u bazi
        public static void ChangeArtikl(int Sifra, string Naziv, string Jm, string Vpc)
        {
            //"UPDATE dbskrpan.namirnice SET items = '" + item + "', number = " + number + " WHERE items = '" + preItem + "';"
            string quarry = "UPDATE [dbo].[Artikli] SET Naziv = @Naziv, Jm = @Jm, Mpc = @Mpc, Vpc = @Vpc WHERE Sifra = @Sifra;";
            decimal Mpc;
            decimal vpc = Convert.ToDecimal(Vpc);
            decimal pdv = 1.25M;
            Mpc = Decimal.Multiply(vpc, pdv);

            using (connection = new SqlConnection(conStr))
            using (SqlCommand com = new SqlCommand(quarry, connection))
            {
                connection.Open();
                try
                {
                    //smislit za sifru
                    com.Parameters.AddWithValue("@Sifra", Sifra);
                    com.Parameters.AddWithValue("@Naziv", Naziv);
                    com.Parameters.AddWithValue("@Jm", Jm);
                    com.Parameters.AddWithValue("@Mpc", Mpc);
                    com.Parameters.AddWithValue("@Vpc", Convert.ToDecimal(Vpc));
                    com.ExecuteNonQuery();
                }
                catch (Exception) { MessageBox.Show("Greška u update Artikla!"); }
            }
        }

        //brisanje artikala <----------------------------------------------------------------------------------------
        public static void DeleteArtikl(string Naziv)
        {
            string querry = "DELETE FROM [dbo].[Artikli] WHERE Naziv = @Naziv;";
            using(connection = new SqlConnection(conStr))
            using(SqlCommand com = new SqlCommand(querry, connection))
            {
                connection.Open();
                try 
                {
                    com.Parameters.AddWithValue("@Naziv", Naziv);
                    com.ExecuteNonQuery();
                }
                catch (Exception) { MessageBox.Show("Greška pri brisanju artikla iz baze!"); }
            }
        }
        //vrati šifru posljednjeg artikla
        public int ReadSifraNoviArtikl() 
        { 
            int sifra = 0;
            string quarry = "SELECT TOP 1 * FROM [dbo].[Artikli] ORDER BY Sifra DESC";

            using (connection = new SqlConnection(conStr))
            using (SqlCommand com = new SqlCommand(quarry, connection))
            {
                connection.Open();
                SqlDataReader reader = com.ExecuteReader();
                try
                {
                    while(reader.Read())
                    {
                        sifra = reader.GetInt32(1);
                    }
                }
                catch (Exception) { MessageBox.Show("Greska pri uzimanju šifre!"); }
            }

            return sifra;
        }

        //ucitavanje artikla u datagrid view
        public void ReadArtiklDataGridView(DataGridView view)
        {
            string quarry = "SELECT * FROM [dbo].[Artikli];";
            using (connection = new SqlConnection(conStr))
            using (SqlDataAdapter adapter = new SqlDataAdapter(quarry, connection))
            {
                try
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    view.DataSource = table;
                    adapter.Update(table);
                }
                catch (Exception) { MessageBox.Show("Greška u čitanju artikla"); }
            }
        }

        //pretraga artikla
        public void SearchArtiklDataGridView(DataGridView view, string Naziv)
        {
            string quarry = "SELECT * FROM [dbo].[Artikli] WHERE Naziv LIKE '%" + Naziv + "%' COLLATE Latin1_General_CI_AI;";
            using (connection = new SqlConnection(conStr))
            using (SqlDataAdapter adapter = new SqlDataAdapter(quarry, connection))
            {
                try
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    view.DataSource = table;
                    adapter.Update(table);
                }
                catch (Exception) { MessageBox.Show("Greška u čitanju pretragi artikla"); }
            }
        }

        //isčitavanje trgovina
        public static void ReadTrgovina(ComboBox cb) 
        {
            string querry = "SELECT * FROM [dbo].[Trgovine]";
            using (connection = new SqlConnection(conStr))
            using (SqlCommand com = new SqlCommand(querry, connection)) 
            {
                connection.Open();
                SqlDataReader reader = com.ExecuteReader();
                try 
                {
                    while (reader.Read()) 
                    {
                        cb.Items.Add(reader.GetString(1));
                    }
                }
                catch (Exception) { MessageBox.Show("Nemre iščitati trgovinu"); }
            } 
        }
        //popunjavanje trgovine u PDF obrazac
        public void PopunjavanjeTrgovinePdf(string trg) 
        {
            string querry = "SELECT * FROM [dbo].[Trgovine] WHERE (Trgovina) = '"+ trg +"';";

            using(connection = new SqlConnection(conStr))
            using (SqlCommand com = new SqlCommand(querry, connection)) 
            {
                connection.Open();
                SqlDataReader reader = com.ExecuteReader();
                try 
                {
                while (reader.Read())
                {
                    Trgovina.Add(reader.GetString(1));
                    UlicaTrg.Add(reader.GetString(2));
                    Oib.Add(reader.GetInt64(3));
                    Iban.Add(reader.GetString(4));
                    Mob.Add(reader.GetString(5));
                    Mjesto.Add(reader.GetString(6));
                    Email.Add(reader.GetString(7));
                }
                        reader.Close();
                    
                }
                catch (Exception) { MessageBox.Show("Ne može ubaciti u PDF Trgovinu"); }
            }
        }
        //upisivanje trgovina tj.dodavanje trgovine
        public static void NewTrgovina(string Trgovina,string Ulica, long Oib,string Iban, string Mob, string Mjesto, string Email) 
        {
            string querry = "INSERT INTO [dbo].[Trgovine] ([Trgovina],[Ulica],[Oib],[Iban],[Mob], [Mjesto], [Email]) VALUES (@Trgovina, @Ulica, @Oib, @Iban, @Mob, @Mjesto, @Email);";

            using(connection = new SqlConnection(conStr))
            using (SqlCommand com = new SqlCommand(querry, connection))
            {
                connection.Open();
                try
                {
                    //smislit za sifru
                    com.Parameters.AddWithValue("@Trgovina", Trgovina);
                    com.Parameters.AddWithValue("@Ulica", Ulica);
                    com.Parameters.AddWithValue("@Oib", Oib);
                    com.Parameters.AddWithValue("@Iban", Iban);
                    com.Parameters.AddWithValue("@Mob", Mob);
                    com.Parameters.AddWithValue("@Mjesto", Mjesto);
                    com.Parameters.AddWithValue("@Email", Email);
                    com.ExecuteNonQuery();
                }
                catch (Exception) { MessageBox.Show("Greška u upisivanju trgovine!!"); }
            }
        }
    }
}
