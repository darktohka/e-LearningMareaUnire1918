
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace eLearningMareaUnire1918 {
    public static class Database {

        private static readonly string createUtilizatori = @"CREATE TABLE [Utilizatori](
            [IdUtilizator] INT IDENTITY(1, 1) NOT NULL,
            [NumePrenumeUtilizator] TEXT NOT NULL,
            [ParolaUtilizator] VARCHAR(100) NOT NULL,
            [EmailUtilizator] VARCHAR(100) NOT NULL,
            [ClasaUtilizator] VARCHAR(100) NOT NULL
        )";
        private static readonly string createItemi = @"CREATE TABLE [Itemi](
            [IdItem] INT IDENTITY(1, 1) NOT NULL,
            [TipItem] INT NOT NULL,
            [EnuntItem] TEXT NOT NULL,
            [Raspuns1Item] TEXT,
            [Raspuns2Item] TEXT,
            [Raspuns3Item] TEXT,
            [Raspuns4Item] TEXT,
            [RaspunsCorectItem] TEXT NOT NULL
        )";
        private static readonly string createEvaluari = @"CREATE TABLE [Evaluari](
            [IdEvaluare] INT IDENTITY(1, 1) NOT NULL,
            [IdElev] INT NOT NULL,
            [DataEvaluare] DATETIME NOT NULL,
            [NotaEvaluare] INT NOT NULL
        )";
        private static List<Item> items = new List<Item>();

        public static List<Item> GetItems() {
            return items;
        }

        public static string GetDatabasePath() {
            return Path.Combine(Environment.CurrentDirectory, "eLearning1918.mdf");
        }

        public static string GetDefaultPath() {
            return Path.Combine(Environment.CurrentDirectory, "date.txt");
        }

        public static string BuildString(Dictionary<string, string> dict) {
            StringBuilder builder = new StringBuilder();

            foreach (KeyValuePair<string, string> pair in dict) {
                builder.Append(pair.Key);
                builder.Append("=");
                builder.Append(pair.Value);
                builder.Append(";");
            }

            return builder.ToString();
        }

        public static string GetConnString() {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Server", "(LocalDB)\\v11.0");
            dict.Add("AttachDbFilename", GetDatabasePath());
            dict.Add("Integrated Security", "True");
            return BuildString(dict);
        }

        public static bool RunQuery(SqlConnection conn, string query, bool ignoreExceptions) {
            using (SqlCommand cmd = conn.CreateCommand()) {
                cmd.CommandText = query;

                if (ignoreExceptions) {
                    try {
                        cmd.ExecuteNonQuery();
                        return true;
                    } catch {
                        return false;
                    }
                } else {
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
        }

        public static bool AuthenticateUser(string email, string password) {
            using (SqlConnection conn = new SqlConnection(GetConnString())) {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand()) {
                    cmd.CommandText = "SELECT IdUtilizator, NumePrenumeUtilizator, ClasaUtilizator FROM [Utilizatori] WHERE EmailUtilizator=@email AND ParolaUtilizator=@password";
                    cmd.Parameters.AddWithValue("email", email);
                    cmd.Parameters.AddWithValue("password", password);

                    DataTable table = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(table);

                    if (table.Rows.Count > 0) {
                        DataRow row = table.Rows[0];
                        int id = row.Field<int>("IdUtilizator");
                        string name = row.Field<string>("NumePrenumeUtilizator");
                        string clasa = row.Field<string>("ClasaUtilizator");

                        ClientInfo.SetClientId(id);
                        ClientInfo.SetClientName(name);
                        ClientInfo.SetClientClass(clasa);
                        return true;
                    } else {
                        return false;
                    }
                }
            }
        }

        public static void CreateEvaluare(int id, DateTime time, int points) {
            using (SqlConnection conn = new SqlConnection(GetConnString())) {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand()) {
                    cmd.CommandText = "INSERT INTO [Evaluari](IdElev, DataEvaluare, NotaEvaluare) VALUES(@id, @time, @points)";
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.Parameters.AddWithValue("time", time);
                    cmd.Parameters.AddWithValue("points", points);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void LoadItems() {
            items.Clear();

            using (SqlConnection conn = new SqlConnection(GetConnString())) {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand()) {
                    cmd.CommandText = "SELECT IdItem, TipItem, EnuntItem, Raspuns1Item, Raspuns2Item, Raspuns3Item, Raspuns4Item, RaspunsCorectItem FROM [Itemi]";
                    DataTable table = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                    adapter.Fill(table);

                    foreach (DataRow row in table.Rows) {
                        int id = row.Field<int>("IdItem");
                        int type = row.Field<int>("TipItem");
                        string description = row.Field<string>("EnuntItem");
                        string first = row.Field<string>("Raspuns1Item");
                        string second = row.Field<string>("Raspuns2Item");
                        string third = row.Field<string>("Raspuns3Item");
                        string fourth = row.Field<string>("Raspuns4Item");
                        string correct = row.Field<string>("RaspunsCorectItem");
                        Item item = new Item(id, type, description, first, second, third, fourth, correct);
                        items.Add(item);
                    }
                }
            }
        }
        public static List<Note> LoadNotes(int id) {
            List<Note> notes = new List<Note>();

            using (SqlConnection conn = new SqlConnection(GetConnString())) {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand()) {
                    cmd.CommandText = "SELECT DataEvaluare, NotaEvaluare FROM [Evaluari] WHERE IdElev=@id";
                    cmd.Parameters.AddWithValue("id", id);

                    DataTable table = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                    adapter.Fill(table);

                    foreach (DataRow row in table.Rows) {
                        DateTime time = row.Field<DateTime>("DataEvaluare");
                        int nota = row.Field<int>("NotaEvaluare");
                        Note note = new Note(nota, time);
                        notes.Add(note);
                    }
                }
            }

            return notes;
        }

        public static double MediaNotelor(string className) {
            // Step 1. Get everybody from class.
            // Step 2. Get every single person's grades and add them together
            // Step 3. Get average

            double noteSum = 0;
            int allNotes = 0;

            using (SqlConnection conn = new SqlConnection(GetConnString())) {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand()) {
                    cmd.CommandText = "SELECT IdUtilizator FROM [Utilizatori] WHERE ClasaUtilizator=@clasa";
                    cmd.Parameters.AddWithValue("clasa", className);
                    DataTable table = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(table);

                    foreach (DataRow row in table.Rows) {
                        int id = row.Field<int>("IdUtilizator");

                        using (SqlCommand cmd2 = conn.CreateCommand()) {
                            cmd2.CommandText = "SELECT NotaEvaluare FROM [Evaluari] WHERE IdElev=@id";
                            cmd2.Parameters.AddWithValue("id", id);
                            DataTable table2 = new DataTable();
                            SqlDataAdapter adapter2 = new SqlDataAdapter(cmd2);
                            adapter2.Fill(table2);

                            foreach (DataRow row2 in table2.Rows) {
                                int nota = row2.Field<int>("NotaEvaluare");
                                noteSum += nota;
                                allNotes++;
                            }
                        }
                    }
                }
            }

            if (allNotes == 0) {
                return 0;
            } else {
                return noteSum / allNotes;
            }
        }

        public static void LoadDatabase() {
            string path = GetDatabasePath();
            string defaultPath = GetDefaultPath();

            if (!File.Exists(defaultPath)) {
                File.WriteAllText(defaultPath, Properties.Resources.date, Encoding.UTF8);
            }

            if (!File.Exists(path)) {
                using (SqlConnection conn = new SqlConnection("Server=(LocalDB)\\v11.0")) {
                    conn.Open();
                    RunQuery(conn, "DROP DATABASE [eLearning1918]", true);
                    RunQuery(conn, "CREATE DATABASE [eLearning1918] ON (NAME='eLearning1918', FILENAME='" + path + "')", false);
                }

                using (SqlConnection conn = new SqlConnection(GetConnString())) {
                    conn.Open();
                    RunQuery(conn, createEvaluari, false);
                    RunQuery(conn, createItemi, false);
                    RunQuery(conn, createUtilizatori, false);

                    string databaseType = null;

                    foreach (string line in File.ReadAllLines(defaultPath)) {
                        if (line.EndsWith(":")) {
                            databaseType = line.Substring(0, line.Length - 1);
                        } else if (databaseType != null) {
                            using (SqlCommand cmd = conn.CreateCommand()) {
                                string[] line2 = line.Split(new char[] { ';' });

                                if (databaseType.Equals("Utilizatori")) {
                                    cmd.CommandText = "INSERT INTO [Utilizatori](NumePrenumeUtilizator, ParolaUtilizator, EmailUtilizator, ClasaUtilizator) VALUES(@nume, @parola, @email, @clasa)";
                                    cmd.Parameters.AddWithValue("nume", line2[0]);
                                    cmd.Parameters.AddWithValue("parola", line2[1]);
                                    cmd.Parameters.AddWithValue("email", line2[2]);
                                    cmd.Parameters.AddWithValue("clasa", line2[3]);
                                } else if (databaseType.Equals("Itemi")) {
                                    cmd.CommandText = "INSERT INTO [Itemi](TipItem, EnuntItem, Raspuns1Item, Raspuns2Item, Raspuns3Item, Raspuns4Item, RaspunsCorectItem) VALUES(@tip, @enunt, @one, @two, @three, @four, @corect)";
                                    cmd.Parameters.AddWithValue("tip", line2[0]);
                                    cmd.Parameters.AddWithValue("enunt", line2[1]);
                                    cmd.Parameters.AddWithValue("one", line2[2]);
                                    cmd.Parameters.AddWithValue("two", line2[3]);
                                    cmd.Parameters.AddWithValue("three", line2[4]);
                                    cmd.Parameters.AddWithValue("four", line2[5]);
                                    cmd.Parameters.AddWithValue("corect", line2[6]);
                                } else if (databaseType.Equals("Evaluari")) {
                                    cmd.CommandText = "INSERT INTO [Evaluari](IdElev,DataEvaluare, NotaEvaluare) VALUES(@elev, @data, @nota)";
                                    cmd.Parameters.AddWithValue("elev", line2[0]);
                                    cmd.Parameters.AddWithValue("data", line2[1]);
                                    cmd.Parameters.AddWithValue("nota", line2[2]);
                                }

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }

            LoadItems();
        }
    }
}
