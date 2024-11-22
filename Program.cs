using System.Data.SQLite;
using System;
namespace AkademineSistema
{
    class Program
    {
        static void Main(string[] args)
        {
            Sistema sistema = new Sistema();
            sistema.Pradeti();
        }
    }
    public class DatabaseHelper
    {
        private string connectionString = "Data Source=\"C:\\Users\\milab\\OneDrive\\Pictures\\akakaka.db\";Version=3;";
        public SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(connectionString);
        }
    }
    public class Sistema
    {
        private List<Studentas> studentai = new List<Studentas>();
        private List<Destytojas> destytojai = new List<Destytojas>();
        private List<Grupe> grupes = new List<Grupe>();
        private List<Dalykas> dalykai = new List<Dalykas>();
        private Destytojas prisijungesDestytojas;
        public void Pradeti()
        {
            while (true)
            {
                Console.WriteLine("Pasirinkite prisijungimo tipa:");
                Console.WriteLine("1. Administratorius");
                Console.WriteLine("2. Destytojas");
                Console.WriteLine("3. Studentas");
                Console.WriteLine("0. Iseiti");
                string pasirinkimas = Console.ReadLine();
                switch (pasirinkimas)
                {
                    case "1":
                        AdministratoriusMeniu();
                        break;
                    case "2":
                        DestytojasMeniu();
                        break;
                    case "3":
                        StudentasMeniu();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Netinkamas pasirinkimas");
                        break;
                }
            }
        }
        private void AdministratoriusMeniu()
        {
            Console.WriteLine("Administratorius prisijunge.");
            while (true)
            {
                Console.WriteLine("1. Kurti grupe");
                Console.WriteLine("2. Salinti grupe");
                Console.WriteLine("3. Prideti destytoja");
                Console.WriteLine("4. Prideti studenta");
                Console.WriteLine("5. Priskirti destytoja dalykui");
                Console.WriteLine("6. Priskirti dalyka grupei");
                Console.WriteLine("7. Prideti dalyka");
                Console.WriteLine("8. Priskirti studenta grupei");
                Console.WriteLine("0. Atsijungti");
                string pasirinkimas = Console.ReadLine();
                switch (pasirinkimas)
                {
                    case "1":
                        KurtiGrupe();
                        break;
                    case "2":
                        SalintiGrupe();
                        break;
                    case "3":
                        PridetiDestytoja();
                        break;
                    case "4":
                        PridetiStudenta();
                        break;
                    case "5":
                        PriskirtiDestytojaDalykui();
                        break;
                    case "6":
                        PriskirtiDalykaGrupei();
                        break;
                    case "7":
                        PridetiDalyka();
                        break;
                    case "8":
                        PriskirtiStudentaGrupei();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Netinkamas pasirinkimas");
                        break;
                }
            }
        }
        private void UzpildytiDestytojoDalykus(Destytojas destytojas)
        {
            DatabaseHelper dbHelper = new DatabaseHelper();
            using (SQLiteConnection conn = dbHelper.GetConnection())
            {
                conn.Open();
                try
                {
                    string query = "SELECT Dalykai FROM Destytojas WHERE Vardas = @Vardas AND Slaptazodis = @Slaptazodis";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Vardas", destytojas.Vardas);
                        cmd.Parameters.AddWithValue("@Slaptazodis", destytojas.Slaptazodis);
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string dalykaiText = reader["Dalykai"]?.ToString();
                                if (!string.IsNullOrEmpty(dalykaiText))
                                {
                                    var dalykaiPavadinimai = dalykaiText.Split(',')
                                                                        .Select(d => d.Trim())
                                                                        .Where(d => !string.IsNullOrEmpty(d))
                                                                        .ToList();
                                    foreach (var pavadinimas in dalykaiPavadinimai)
                                    {
                                        destytojas.Dalykai.Add(new Dalykas { Pavadinimas = pavadinimas });
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ivyko klaida: {ex.Message}");
                }
            }
        }
        private void DestytojasMeniu()
        {
            Console.Write("Iveskite varda: ");
            string vardas = Console.ReadLine();
            Console.Write("Iveskite slaptazodi: ");
            string pavarde = Console.ReadLine();
            DatabaseHelper dbHelper = new DatabaseHelper();
            using (SQLiteConnection conn = dbHelper.GetConnection())
            {
                conn.Open();
                try
                {
                    string query = "SELECT Vardas, Slaptazodis FROM Destytojas WHERE Vardas = @Vardas AND Slaptazodis = @Slaptazodis";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Vardas", vardas);
                        cmd.Parameters.AddWithValue("@Slaptazodis", pavarde);
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Destytojas destytojas = new Destytojas
                                {
                                    Vardas = reader["Vardas"].ToString(),
                                    Slaptazodis = reader["Slaptazodis"].ToString(),
                                    Dalykai = new List<Dalykas>()
                                };
                                UzpildytiDestytojoDalykus(destytojas);
                                if (!destytojas.Dalykai.Any())
                                {
                                    Console.WriteLine($"Destytojui '{destytojas.Vardas}' nepriskirta jokiu dalyku");
                                    return;
                                }
                                Console.WriteLine($"Sveiki, {destytojas.Vardas}!");
                                Console.WriteLine("Jusu priskirti dalykai:");
                                foreach (var dalykas in destytojas.Dalykai)
                                {
                                    Console.WriteLine($"- {dalykas.Pavadinimas}");
                                }
                                while (true)
                                {
                                    Console.WriteLine("1. Ivesti arba redaguoti pazymi");
                                    Console.WriteLine("2. Salinti pazymi");
                                    Console.WriteLine("0. Atsijungti");
                                    string pasirinkimas = Console.ReadLine();
                                    switch (pasirinkimas)
                                    {
                                        case "1":
                                            IvestiRedaguotiPazymi(destytojas);
                                            break;
                                        case "2":
                                            SalintiPazymi();
                                            break;
                                        case "0":
                                            return;
                                        default:
                                            Console.WriteLine("Netinkamas pasirinkimas");
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("Neteisingas vardas arba pavarde");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ivyko klaida: {ex.Message}");
                }
            }
        }
        private void StudentasMeniu()
        {
            Console.Write("Iveskite varda: ");
            string vardas = Console.ReadLine();
            Console.Write("Iveskite slaptazodi: ");
            string pavarde = Console.ReadLine();
            Studentas studentas = studentai.FirstOrDefault(s => s.Vardas == vardas && s.Slaptazodis == pavarde);
            if (studentas == null)
            {
                Console.WriteLine("Neteisingas vardas arba pavarde");
                return;
            }
            Console.WriteLine($"Sveiki, {studentas.Vardas}!");
            while (true)
            {
                Console.WriteLine("1. Perziureti savo pazymius");
                Console.WriteLine("0. Atsijungti");
                string pasirinkimas = Console.ReadLine();
                switch (pasirinkimas)
                {
                    case "1":
                        PerziuretiSavoPazymius(studentas);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Netinkamas pasirinkimas");
                        break;
                }
            }
        }
        private void IvestiRedaguotiPazymi(Destytojas destytojas)//neveikia
        {
            Console.WriteLine("Pasirinkite dalyka:");
            if (!destytojas.Dalykai.Any())
            {
                Console.WriteLine("Jums nepriskirta jokiu dalyku");
                return;
            }
            for (int i = 0; i < destytojas.Dalykai.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {destytojas.Dalykai[i].Pavadinimas}");
            }
            Console.Write("Pasirinkite dalyka: ");
            if (int.TryParse(Console.ReadLine(), out int dalykasIndeksas) && dalykasIndeksas > 0 && dalykasIndeksas <= destytojas.Dalykai.Count)
            {
                string pasirinktasDalykas = destytojas.Dalykai[dalykasIndeksas - 1].Pavadinimas;
                Console.WriteLine("Pasirinkite studenta:");
                DatabaseHelper dbHelper = new DatabaseHelper();
                using (SQLiteConnection conn = dbHelper.GetConnection())
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string query = @"
                    SELECT Studentas.Vardas, Studentas.Slaptazodis
                    FROM Studentas
                    JOIN GrupeDalykas ON GrupeDalykas.GrupePavadinimas = Studentas.GrupesPavadinimas
                    WHERE GrupeDalykas.DalykasPavadinimas = @DalykasPavadinimas";
                            using (SQLiteCommand cmd = new SQLiteCommand(query, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@DalykasPavadinimas", pasirinktasDalykas);
                                using (SQLiteDataReader reader = cmd.ExecuteReader())
                                {
                                    List<Studentas> studentai = new List<Studentas>();
                                    int index = 1;
                                    while (reader.Read())
                                    {
                                        string vardas = reader["Vardas"].ToString();
                                        string slaptazodis = reader["Slaptazodis"].ToString();
                                        Console.WriteLine($"{index}. {vardas}");
                                        studentai.Add(new Studentas { Vardas = vardas, Slaptazodis = slaptazodis });
                                        index++;
                                    }
                                    if (!studentai.Any())
                                    {
                                        Console.WriteLine("Siam dalykui nera priskirtu studentu");
                                        return;
                                    }
                                    Console.Write("Pasirinkite studenta: ");
                                    if (int.TryParse(Console.ReadLine(), out int studentasIndeksas) && studentasIndeksas > 0 && studentasIndeksas <= studentai.Count)
                                    {
                                        Studentas pasirinktasStudentas = studentai[studentasIndeksas - 1];
                                        Console.Write("Iveskite pazymi: ");
                                        if (int.TryParse(Console.ReadLine(), out int pazymys))
                                        {
                                            string insertQuery = @"
                                    INSERT INTO Pazymiai (StudentasVardas, StudentasSlaptazodis, DalykasPavadinimas, Pazymys)
                                    VALUES (@Vardas, @Slaptazodis, @DalykasPavadinimas, @Pazymys)";
                                            using (SQLiteCommand insertCmd = new SQLiteCommand(insertQuery, conn, transaction))
                                            {
                                                insertCmd.Parameters.AddWithValue("@Vardas", pasirinktasStudentas.Vardas);
                                                insertCmd.Parameters.AddWithValue("@Slaptazodis", pasirinktasStudentas.Slaptazodis);
                                                insertCmd.Parameters.AddWithValue("@DalykasPavadinimas", pasirinktasDalykas);
                                                insertCmd.Parameters.AddWithValue("@Pazymys", pazymys);
                                                insertCmd.ExecuteNonQuery();
                                            }
                                            Console.WriteLine($"Pazymys {pazymys} pridetas studentui '{pasirinktasStudentas.Vardas}' dalykui '{pasirinktasDalykas}'.");
                                            transaction.Commit();
                                        }
                                        else
                                        {
                                            Console.WriteLine("Neteisingas pazymio formatas");
                                            transaction.Rollback();
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Netinkamas studento pasirinkima");
                                        transaction.Rollback();
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine($"Ivyko klaida: {ex.Message}");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Netinkamas dalyko pasirinkimas");
            }
        }
        private void SalintiPazymi()//neveikia
        {
            Console.WriteLine("Pasirinkite grupe:");
            for (int i = 0; i < grupes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {grupes[i].Pavadinimas}");
            }
            Console.Write("Pasirinkite grupe: ");
            if (int.TryParse(Console.ReadLine(), out int grupeIndeksas) && grupeIndeksas > 0 && grupeIndeksas <= grupes.Count)
            {
                Grupe pasirinktaGrupe = grupes[grupeIndeksas - 1];
                Console.WriteLine("Pasirinkite dalyka:");
                var destytojoDalykai = pasirinktaGrupe.Dalykai.Where(d => d.Destytojai.Contains(prisijungesDestytojas)).ToList();
                if (destytojoDalykai.Count == 0)
                {
                    Console.WriteLine("Sioje grupeje nera dalyku, priskirtu jums.");
                    return;
                }
                for (int i = 0; i < destytojoDalykai.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {destytojoDalykai[i].Pavadinimas}");
                }
                Console.Write("Pasirinkite dalyka: ");
                if (int.TryParse(Console.ReadLine(), out int dalykasIndeksas) && dalykasIndeksas > 0 && dalykasIndeksas <= destytojoDalykai.Count)
                {
                    Dalykas pasirinktasDalykas = destytojoDalykai[dalykasIndeksas - 1];
                    Console.WriteLine("Pasirinkite studenta:");
                    for (int i = 0; i < pasirinktaGrupe.Studentai.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {pasirinktaGrupe.Studentai[i].Vardas}");
                    }
                    Console.Write("Pasirinkite studenta: ");
                    if (int.TryParse(Console.ReadLine(), out int studentasIndeksas) && studentasIndeksas > 0 && studentasIndeksas <= pasirinktaGrupe.Studentai.Count)
                    {
                        Studentas pasirinktasStudentas = pasirinktaGrupe.Studentai[studentasIndeksas - 1];
                        if (pasirinktasStudentas.Pazymiai.TryGetValue(pasirinktasDalykas.Pavadinimas, out List<int> pazymiai) && pazymiai.Count > 0)
                        {
                            Console.WriteLine("Pazymiai:");
                            for (int i = 0; i < pazymiai.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. {pazymiai[i]}");
                            }
                            Console.Write("Pasirinkite pazymi, kuri norite pasalinti: ");
                            if (int.TryParse(Console.ReadLine(), out int pazymysIndeksas) && pazymysIndeksas > 0 && pazymysIndeksas <= pazymiai.Count)
                            {
                                int pasalintasPazymys = pazymiai[pazymysIndeksas - 1];
                                pazymiai.RemoveAt(pazymysIndeksas - 1);
                                Console.WriteLine($"Pazymys {pasalintasPazymys} pasalintas is studento '{pasirinktasStudentas.Vardas}' dalykui '{pasirinktasDalykas.Pavadinimas}'.");
                            }
                            else
                            {
                                Console.WriteLine("Netinkamas pazymio pasirinkimas");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Studentas '{pasirinktasStudentas.Vardas}' neturi pazymiu dalykui '{pasirinktasDalykas.Pavadinimas}'.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Netinkamas studento pasirinkimas");
                    }
                }
                else
                {
                    Console.WriteLine("Netinkamas dalyko pasirinkimas");
                }
            }
            else
            {
                Console.WriteLine("Netinkamas grupes pasirinkimas");
            }
        }
        private void KurtiGrupe()
        {
            Console.Write("Iveskite grupes pavadinima: ");
            string pavadinimas = Console.ReadLine();
            DatabaseHelper dbHelper = new DatabaseHelper();
            using (SQLiteConnection conn = dbHelper.GetConnection())
            {
                conn.Open();
                try
                {
                    string query = "INSERT INTO GrupeDalykas (GrupePavadinimas) VALUES (@Pavadinimas)";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Pavadinimas", pavadinimas);
                        cmd.ExecuteNonQuery();
                    }
                    grupes.Add(new Grupe { Pavadinimas = pavadinimas });
                    Console.WriteLine($"Grupe '{pavadinimas}' sukurta.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ivyko klaida: {ex.Message}");
                }
            }
        }
        private void SalintiGrupe()
        {
            Console.WriteLine("Esamos grupes:");
            DatabaseHelper dbHelper = new DatabaseHelper();
            using (SQLiteConnection conn = dbHelper.GetConnection())
            {
                conn.Open();
                try
                {
                    string query = "SELECT DISTINCT GrupePavadinimas FROM GrupeDalykas";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            List<string> grupesPavadinimai = new List<string>();
                            while (reader.Read())
                            {
                                string grupesPavadinimas = reader["GrupePavadinimas"].ToString();
                                grupesPavadinimai.Add(grupesPavadinimas);
                                Console.WriteLine($"{grupesPavadinimai.Count}. {grupesPavadinimas}");
                            }
                            Console.Write("Pasirinkite grupe, kuria norite pasalinti: ");
                            if (int.TryParse(Console.ReadLine(), out int pasirinkimas) && pasirinkimas > 0 && pasirinkimas <= grupesPavadinimai.Count)
                            {
                                string grupesPavadinimas = grupesPavadinimai[pasirinkimas - 1];
                                string deleteDalykaiQuery = "DELETE FROM GrupeDalykas WHERE GrupePavadinimas = @GrupePavadinimas";
                                using (SQLiteCommand deleteCmd = new SQLiteCommand(deleteDalykaiQuery, conn))
                                {
                                    deleteCmd.Parameters.AddWithValue("@GrupePavadinimas", grupesPavadinimas);
                                    deleteCmd.ExecuteNonQuery();
                                }
                                Console.WriteLine($"Grupe '{grupesPavadinimas}' ir visi jos rysiai su dalykais buvo pasalinti");
                            }
                            else
                            {
                                Console.WriteLine("Netinkamas pasirinkimas");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ivyko klaida: {ex.Message}");
                }
            }
        }
        private void UzpildytiDestytojus()
        {
            destytojai.Clear();
            DatabaseHelper dbHelper = new DatabaseHelper();
            using (SQLiteConnection conn = dbHelper.GetConnection())
            {
                conn.Open();
                try
                {
                    string query = "SELECT Vardas, Slaptazodis FROM Destytojas";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string vardas = reader["Vardas"].ToString();
                                string slaptazodis = reader["Slaptazodis"].ToString();
                                destytojai.Add(new Destytojas { Vardas = vardas, Slaptazodis = slaptazodis });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ivyko klaida: {ex.Message}");
                }
            }
        }
        private void PridetiDestytoja()
        {
            Console.Write("Iveskite destytojo varda: ");
            string vardas = Console.ReadLine();
            Console.Write("Iveskite destytojo pavarde: ");
            string pavarde = Console.ReadLine();
            DatabaseHelper dbHelper = new DatabaseHelper();
            using (SQLiteConnection conn = dbHelper.GetConnection())
            {
                conn.Open();
                try
                {
                    string query = "INSERT INTO Destytojas (Vardas, Slaptazodis) VALUES (@Vardas, @Slaptazodis)";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Vardas", vardas);
                        cmd.Parameters.AddWithValue("@Slaptazodis", pavarde);
                        cmd.ExecuteNonQuery();
                    }
                    destytojai.Clear();
                    string selectQuery = "SELECT Vardas, Slaptazodis FROM Destytojas";
                    using (SQLiteCommand selectCmd = new SQLiteCommand(selectQuery, conn))
                    {
                        using (SQLiteDataReader reader = selectCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string destytojasVardas = reader["Vardas"].ToString();
                                string destytojasSlaptazodis = reader["Slaptazodis"].ToString();
                                destytojai.Add(new Destytojas { Vardas = destytojasVardas, Slaptazodis = destytojasSlaptazodis });
                            }
                        }
                    }
                    Console.WriteLine($"Destytojas '{vardas} {pavarde}' pridetas");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ivyko klaida: {ex.Message}");
                }
            }
        }
        private void PridetiStudenta()
        {
            Console.Write("Iveskite studento varda: ");
            string vardas = Console.ReadLine();
            Console.Write("Iveskite studento pavarde: ");
            string pavarde = Console.ReadLine();
            DatabaseHelper dbHelper = new DatabaseHelper();
            using (SQLiteConnection conn = dbHelper.GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string query = "INSERT INTO Studentas (Vardas, Slaptazodis) VALUES (@Vardas, @Slaptazodis)";
                        using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Vardas", vardas);
                            cmd.Parameters.AddWithValue("@Slaptazodis", pavarde);
                            cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();
                        Console.WriteLine("Studentas pridetas sekmingai");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Ivyko klaida: {ex.Message}");
                    }
                }
            }
        }
        private void UzpildytiDalykus()
        {
            dalykai.Clear();
            DatabaseHelper dbHelper = new DatabaseHelper();
            using (SQLiteConnection conn = dbHelper.GetConnection())
            {
                conn.Open();
                try
                {
                    string query = "SELECT Pavadinimas FROM Dalykas";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string dalykasPavadinimas = reader["Pavadinimas"].ToString();
                                dalykai.Add(new Dalykas { Pavadinimas = dalykasPavadinimas });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ivyko klaida: {ex.Message}");
                }
            }
        }
        private void PridetiDalyka()
        {
            Console.Write("Iveskite dalyko pavadinima: ");
            string pavadinimas = Console.ReadLine();
            DatabaseHelper dbHelper = new DatabaseHelper();
            using (SQLiteConnection conn = dbHelper.GetConnection())
            {
                conn.Open();
                try
                {
                    string query = "INSERT INTO Dalykas (Pavadinimas) VALUES (@Pavadinimas)";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Pavadinimas", pavadinimas);
                        cmd.ExecuteNonQuery();
                    }
                    UzpildytiDalykus();
                    Console.WriteLine($"Dalykas '{pavadinimas}' pridetas");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ivyko klaida: {ex.Message}");
                }
            }
        }
        private void PriskirtiDestytojaDalykui()
        {
            UzpildytiDalykus();
            UzpildytiDestytojus();
            Console.WriteLine("Esami dalykai:");
            for (int i = 0; i < dalykai.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {dalykai[i].Pavadinimas}");
            }
            Console.Write("Pasirinkite dalyka: ");
            if (int.TryParse(Console.ReadLine(), out int pasirinkimas) && pasirinkimas > 0 && pasirinkimas <= dalykai.Count)
            {
                Dalykas pasirinktasDalykas = dalykai[pasirinkimas - 1];
                Console.WriteLine("Esami destytojai:");
                for (int i = 0; i < destytojai.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {destytojai[i].Vardas}");
                }
                Console.Write("Pasirinkite destytoja: ");
                if (int.TryParse(Console.ReadLine(), out int destytojoIndeksas) && destytojoIndeksas > 0 && destytojoIndeksas <= destytojai.Count)
                {
                    Destytojas pasirinktasDestytojas = destytojai[destytojoIndeksas - 1];
                    DatabaseHelper dbHelper = new DatabaseHelper();
                    using (SQLiteConnection conn = dbHelper.GetConnection())
                    {
                        conn.Open();
                        try
                        {
                            string query = "SELECT Dalykai FROM Destytojas WHERE Vardas = @Vardas AND Slaptazodis = @Slaptazodis";
                            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@Vardas", pasirinktasDestytojas.Vardas);
                                cmd.Parameters.AddWithValue("@Slaptazodis", pasirinktasDestytojas.Slaptazodis);
                                using (SQLiteDataReader reader = cmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        string dalykai = reader["Dalykai"].ToString();
                                        if (string.IsNullOrEmpty(dalykai))
                                        {
                                            dalykai = pasirinktasDalykas.Pavadinimas;
                                        }
                                        else
                                        {
                                            dalykai += "," + pasirinktasDalykas.Pavadinimas;
                                        }
                                        string updateQuery = "UPDATE Destytojas SET Dalykai = @Dalykai WHERE Vardas = @Vardas AND Slaptazodis = @Slaptazodis";
                                        using (SQLiteCommand updateCmd = new SQLiteCommand(updateQuery, conn))
                                        {
                                            updateCmd.Parameters.AddWithValue("@Dalykai", dalykai);
                                            updateCmd.Parameters.AddWithValue("@Vardas", pasirinktasDestytojas.Vardas);
                                            updateCmd.Parameters.AddWithValue("@Slaptazodis", pasirinktasDestytojas.Slaptazodis);
                                            updateCmd.ExecuteNonQuery();
                                        }
                                        Console.WriteLine($"Dalykas '{pasirinktasDalykas.Pavadinimas}' priskirtas destytojui '{pasirinktasDestytojas.Vardas}'");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ivyko klaida: {ex.Message}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Netinkamas destytojo pasirinkimas");
                }
            }
            else
            {
                Console.WriteLine("Netinkamas dalyko pasirinkimas");
            }
        }
        private void UzpildytiGrupes()
        {
            grupes.Clear();
            DatabaseHelper dbHelper = new DatabaseHelper();
            using (SQLiteConnection conn = dbHelper.GetConnection())
            {
                conn.Open();
                try
                {
                    string query = "SELECT DISTINCT GrupePavadinimas FROM GrupeDalykas";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string pavadinimas = reader["GrupePavadinimas"].ToString();
                                grupes.Add(new Grupe { Pavadinimas = pavadinimas });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ivyko klaida: {ex.Message}");
                }
            }
        }
        private void PriskirtiDalykaGrupei()
        {
            UzpildytiGrupes();
            if (grupes.Count == 0)
            {
                Console.WriteLine("Nera jokiu grupiu duomenu bazeje");
                return;
            }
            Console.WriteLine("Esamos grupes:");
            for (int i = 0; i < grupes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {grupes[i].Pavadinimas}");
            }
            Console.Write("Pasirinkite grupe: ");
            if (int.TryParse(Console.ReadLine(), out int grupeIndeksas) && grupeIndeksas > 0 && grupeIndeksas <= grupes.Count)
            {
                Grupe grupe = grupes[grupeIndeksas - 1];
                UzpildytiDalykus();
                if (dalykai.Count == 0)
                {
                    Console.WriteLine("Nera jokiu dalyku duomenu bazeje");
                    return;
                }
                Console.WriteLine("Esami dalykai:");
                for (int i = 0; i < dalykai.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {dalykai[i].Pavadinimas}");
                }
                Console.Write("Pasirinkite dalyka: ");
                if (int.TryParse(Console.ReadLine(), out int dalykasIndeksas) && dalykasIndeksas > 0 && dalykasIndeksas <= dalykai.Count)
                {
                    DatabaseHelper dbHelper = new DatabaseHelper();
                    using (SQLiteConnection conn = dbHelper.GetConnection())
                    {
                        conn.Open();
                        try
                        {
                            string query = "INSERT INTO GrupeDalykas (GrupePavadinimas, DalykasPavadinimas) VALUES (@GrupePavadinimas, @DalykasPavadinimas)";
                            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@GrupePavadinimas", grupe.Pavadinimas);
                                cmd.Parameters.AddWithValue("@DalykasPavadinimas", dalykai[dalykasIndeksas - 1].Pavadinimas);
                                cmd.ExecuteNonQuery();
                            }
                            Console.WriteLine($"Dalykas '{dalykai[dalykasIndeksas - 1].Pavadinimas}' priskirtas grupei '{grupe.Pavadinimas}'.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ivyko klaida: {ex.Message}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Netinkamas dalyko pasirinkimas");
                }
            }
            else
            {
                Console.WriteLine("Netinkamas grupes pasirinkimas");
            }
        }
        private void UzpildytiStudentus()
        {
            DatabaseHelper dbHelper = new DatabaseHelper();
            using (SQLiteConnection conn = dbHelper.GetConnection())
            {
                conn.Open();
                try
                {
                    string query = "SELECT Vardas, Slaptazodis FROM Studentas";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string vardas = reader["Vardas"].ToString();
                                string pavarde = reader["Slaptazodis"].ToString();
                                studentai.Add(new Studentas { Vardas = vardas, Slaptazodis = pavarde });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ivyko klaida: {ex.Message}");
                }
            }
        }
        private void PriskirtiStudentaGrupei()
        {
            UzpildytiGrupes();
            Console.WriteLine("Esamos grupes:");
            for (int i = 0; i < grupes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {grupes[i].Pavadinimas}");
            }
            Console.Write("Pasirinkite grupe: ");
            if (int.TryParse(Console.ReadLine(), out int grupeIndeksas) && grupeIndeksas > 0 && grupeIndeksas <= grupes.Count)
            {
                Grupe pasirinktaGrupe = grupes[grupeIndeksas - 1];
                UzpildytiStudentus();
                Console.WriteLine("Esami studentai:");
                for (int i = 0; i < studentai.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {studentai[i].Vardas}");
                }
                Console.Write("Pasirinkite studenta: ");
                if (int.TryParse(Console.ReadLine(), out int studentasIndeksas) && studentasIndeksas > 0 && studentasIndeksas <= studentai.Count)
                {
                    Studentas pasirinktasStudentas = studentai[studentasIndeksas - 1];
                    DatabaseHelper dbHelper = new DatabaseHelper();
                    using (SQLiteConnection conn = dbHelper.GetConnection())
                    {
                        conn.Open();
                        try
                        {
                            string query = "UPDATE Studentas SET GrupesPavadinimas = @GrupesPavadinimas WHERE Vardas = @Vardas AND Slaptazodis = @Slaptazodis";
                            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@GrupesPavadinimas", pasirinktaGrupe.Pavadinimas);
                                cmd.Parameters.AddWithValue("@Vardas", pasirinktasStudentas.Vardas);
                                cmd.Parameters.AddWithValue("@Slaptazodis", pasirinktasStudentas.Slaptazodis);
                                cmd.ExecuteNonQuery();
                            }
                            Console.WriteLine($"Studentas '{pasirinktasStudentas.Vardas}' priskirtas grupei '{pasirinktaGrupe.Pavadinimas}'");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ivyko klaida: {ex.Message}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Netinkamas studento pasirinkimas");
                }
            }
            else
            {
                Console.WriteLine("Netinkamas grupes pasirinkimas");
            }
        }
        private void PerziuretiSavoPazymius(Studentas studentas)// neveikia
        {
            Console.WriteLine("Jusu pazymiai:");
            if (studentas.Pazymiai.Count == 0)
            {
                Console.WriteLine("Neturite pazymiu");
            }
            else
            {
                foreach (var dalykas in studentas.Pazymiai)
                {
                    Console.WriteLine($"- {dalykas.Key}: {string.Join(", ", dalykas.Value)}");
                }
            }
        }
        public class Studentas
        {
            public string Vardas { get; set; }
            public string Slaptazodis { get; set; }
            public Dictionary<string, List<int>> Pazymiai { get; set; } = new Dictionary<string, List<int>>();
        }
        public class Destytojas
        {
            public string Vardas { get; set; }
            public string Slaptazodis { get; set; }
            public List<Dalykas> Dalykai { get; set; } = new List<Dalykas>();
        }
        public class Grupe
        {
            public string Pavadinimas { get; set; }
            public List<Dalykas> Dalykai { get; set; } = new List<Dalykas>();
            public List<Studentas> Studentai { get; set; } = new List<Studentas>();
            public void PristatytiStudenta(Studentas studentas)
            {
                Studentai.Add(studentas);
            }
        }
        public class Dalykas
        {
            public string Pavadinimas { get; set; }
            public List<Destytojas> Destytojai { get; set; } = new List<Destytojas>();
            public bool ArDalykasPriskirtas(Destytojas destytojas)
            {
                return Destytojai.Contains(destytojas);
            }
        }
    }
}