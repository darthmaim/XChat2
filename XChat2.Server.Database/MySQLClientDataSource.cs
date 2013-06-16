using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace XChat2.Server.Database
{
    public class MySQLClientDataSource : IClientDataSource
    {
        private string _connectionString;
        private MySqlConnection _connection;

        public MySQLClientDataSource(string server, string database, string user, string password)
        {
            StringBuilder sb = new StringBuilder();
            MySqlConnectionStringBuilder.AppendKeyValuePair(sb, "SERVER", server);
            MySqlConnectionStringBuilder.AppendKeyValuePair(sb, "DATABASE", database);
            MySqlConnectionStringBuilder.AppendKeyValuePair(sb, "UID", user);
            MySqlConnectionStringBuilder.AppendKeyValuePair(sb, "PASSWORD", password);

            _connectionString = sb.ToString();
        }        

        public void Open()
        {
            _connection = new MySqlConnection(_connectionString);
            _connection.Open();
        }

        public void Close()
        {
            _connection.Close();
            _connection.Dispose();
        }

        public bool Exists(uint id)
        {
            using (MySqlCommand cmd = new MySqlCommand("SELECT id, username, password, email FROM users WHERE id = ?id", _connection))
            {
                cmd.Parameters.AddWithValue("?id", id);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                    return reader.HasRows;
            
            }
        }

        public ClientInformation GetByID(uint uid)
        {
            using (MySqlCommand cmd = new MySqlCommand("SELECT id, username, password, email FROM users WHERE id = ?id", _connection))
            {
                cmd.Parameters.AddWithValue("?id", uid);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        uint id = reader.GetUInt32("id");
                        string username = reader.GetString("username");
                        string password = reader.GetString("password");
                        string email = reader.GetString("email");

                        return new ClientInformation()
                        {
                            UserID = id,
                            Username = username,
                            Password = password,
                            Email = email
                        };
                    }
                }
            }

            throw new Exception("Unknwon error :(");
        }

        public IEnumerable<ClientInformation> GetAll()
        {
            using (MySqlCommand cmd = new MySqlCommand("SELECT id, username, password, email FROM users", _connection))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        uint id = reader.GetUInt32("id");
                        string username = reader.GetString("username");
                        string password = reader.GetString("password");
                        string email = reader.GetString("email");

                        yield return new ClientInformation()
                        {
                            UserID = id,
                            Username = username,
                            Password = password,
                            Email = email
                        };
                    }
                }
            }
        }

        public void Update(ClientInformation data)
        {
            using (MySqlCommand cmd = new MySqlCommand("UPDATE users SET username = ?username, password = ?password, email = ?email WHERE id = ?id"))
            {
                cmd.Parameters.AddWithValue("?id", data.UserID);
                cmd.Parameters.AddWithValue("?username", data.Username);
                cmd.Parameters.AddWithValue("?password", data.Password);
                cmd.Parameters.AddWithValue("?email", data.Email);

                cmd.ExecuteNonQuery();
            }
        }

        public IEnumerable<uint> GetContacts(uint id)
        {
            using (MySqlCommand cmd = new MySqlCommand("SELECT user1, user2 FROM contacts WHERE user1 = ?id OR user2 = ?id", _connection))
            {
                cmd.Parameters.AddWithValue("?id", id);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        uint user1 = reader.GetUInt32("user1");
                        uint user2 = reader.GetUInt32("user2");

                        if (user1 == id)
                            yield return user2;
                        else
                            yield return user1;
                    }
                }
            }
        }
    }
}
