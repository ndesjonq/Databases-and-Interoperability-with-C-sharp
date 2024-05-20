using System;
using MySqlConnector;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;


namespace DuckHunt
{
    public class DatabaseManager
    {
        private static MySqlConnection cn;
        private static string connection_string = "server=127.0.0.1;port=3307;uid=root;password=1234";
        private string db_name;

        public DatabaseManager(string name)
        {
            db_name = name;
            cn = new MySqlConnection(connection_string);

            CreateGameDatabase();
        }

        public void CreateGameDatabase()
        {
            cn.Open();

            MySqlCommand cmd = cn.CreateCommand();

            cmd.CommandText = "CREATE DATABASE IF NOT EXISTS " + db_name + ";";

            // YYYY-MM-DD HH:MM:SS
            cmd.CommandText += "USE " + db_name + ";" +
                    "CREATE TABLE IF NOT EXISTS hunter_plays (id VARCHAR(200) PRIMARY KEY, player_id VARCHAR(25), score INT, date VARCHAR(25));" +
                    "CREATE TABLE IF NOT EXISTS duck_plays (id VARCHAR(200) PRIMARY KEY, player_id VARCHAR(25), score INT, date VARCHAR(25));" +
                    "CREATE TABLE IF NOT EXISTS player_table (player_id VARCHAR(25) PRIMARY KEY, password VARCHAR(25));";

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            cn.Close();
        }

        public void AddPlayer(string player_id, string password)
        {
            cn.Open();

            MySqlCommand cmd = cn.CreateCommand();

            cmd.CommandText = "USE " + db_name + "; INSERT INTO player_table VALUES (\"" +  player_id+ "\", \"" + password +"\");";

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            cn.Close();
        }

        public void AddScore(string player_type, string player_id, int score, string date)
        {
            cn.Open();

            MySqlCommand cmd = cn.CreateCommand();

            cmd.CommandText = "USE " + db_name + "; INSERT INTO " + player_type + "_plays VALUES (\"" + GenerateId(player_id + "" + score + "" + date) + "\", \"" + player_id + "\", " + score + ", \"" + date + "\");";

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            cn.Close();
        }

        public static string GenerateId(string str)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Compute the hash
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(str));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public bool CheckIfPlayerExists(string player_id)
        {
            int count = 0;

            cn.Open();

            MySqlCommand cmd = cn.CreateCommand();

            cmd.CommandText = "USE " + db_name + "; SELECT COUNT(*) FROM player_table WHERE player_id = \"" + player_id + "\"";

            try
            {
                cmd.ExecuteNonQuery();

                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            cn.Close();

            return count > 0;
        }

        public bool CheckIfPasswordIsCorrect(string player_id, string password)
        {
            int count = 0;

            cn.Open();

            MySqlCommand cmd = cn.CreateCommand();

            cmd.CommandText = "USE " + db_name + "; SELECT COUNT(*) FROM player_table WHERE player_id = \"" + player_id + "\" AND password = \"" + password + "\"";

            try
            {
                cmd.ExecuteNonQuery();

                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            cn.Close();

            return count > 0;
        }

        public (string, string) GetLeaderBoards()   
        {
            cn.Open();

            string hunter_leaderboard = "";
            string duck_leaderboard = "";

            try
            {
                Dictionary<string, Dictionary<string, Dictionary<string, string>>> leaderboards = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

                MySqlCommand cmd = new MySqlCommand("USE " + db_name + "; SELECT id, player_id, score, date FROM hunter_plays ORDER BY score DESC", cn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    leaderboards["hunter"] = new Dictionary<string, Dictionary<string, string>>();

                    while (reader.Read())
                    {
                        string id = reader.GetString("id");
                        string player_id = reader.GetString("player_id");
                        string score = reader.GetInt32("score").ToString();
                        string date = reader.GetString("date");
                        leaderboards["hunter"][id] = new Dictionary<string, string>();
                        leaderboards["hunter"][id]["player_id"] = player_id;
                        leaderboards["hunter"][id]["score"] = score;
                        leaderboards["hunter"][id]["date"] = date;
                    }

                    reader.Close();
                }

                cmd = new MySqlCommand("USE " + db_name + "; SELECT id, player_id, score, date FROM duck_plays ORDER BY score DESC", cn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    leaderboards["duck"] = new Dictionary<string, Dictionary<string, string>>();

                    while (reader.Read())
                    {
                        string id = reader.GetString("id");
                        string player_id = reader.GetString("player_id");
                        string score = reader.GetInt32("score").ToString();
                        string date = reader.GetString("date");
                        leaderboards["duck"][id] = new Dictionary<string, string>();
                        leaderboards["duck"][id]["player_id"] = player_id;
                        leaderboards["duck"][id]["score"] = score;
                        leaderboards["duck"][id]["date"] = date;
                    }

                    reader.Close();
                }

                string temp_string;

                foreach (var kvp1 in leaderboards)
                {
                    temp_string = "";

                    foreach (var kvp2 in kvp1.Value)
                    {
                        temp_string += kvp2.Value["player_id"] + " - " + kvp2.Value["score"] + "pts - " + kvp2.Value["date"] + "\n";
                    }

                    if (kvp1.Key == "hunter")
                    {
                        hunter_leaderboard = temp_string;
                    }
                    else
                    {
                        duck_leaderboard = temp_string;
                    }
                }
            }
            catch (Exception ex)
            {
                (hunter_leaderboard, duck_leaderboard) = (ex.Message, ex.Message);
            }

            cn.Close();

            return (hunter_leaderboard, duck_leaderboard);
        }

        public string GetAllPlayerIds(bool show_passwords = false)
        {
            cn.Open();

            string players_list = "";

            try
            {
                string query = $"USE {db_name}; SELECT player_id {(show_passwords ? ", password" : "")} FROM player_table";

                using (MySqlCommand cmd = new MySqlCommand(query, cn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            players_list += $"{reader.GetString("player_id")} {(show_passwords ? $" (pw : {reader.GetString("password")})": "")} \n";
                        }

                        

                    }
                }
            }
            catch (Exception ex)
            {
                players_list = ex.Message;
            }

            cn.Close();

            return players_list;
        }
    }
}
