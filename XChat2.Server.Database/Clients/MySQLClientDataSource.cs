using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace XChat2.Server.Database.Clients
{
    public class MySQLClientDataSource : IClientDataSource, IDisposable
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
        public bool Exists(string username)
        {
            using (MySqlCommand cmd = new MySqlCommand("SELECT id, username, password, email FROM users WHERE username = ?username", _connection))
            {
                cmd.Parameters.AddWithValue("?username", username);
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
        public ClientInformation GetByUsername(string name)
        {
            using (MySqlCommand cmd = new MySqlCommand("SELECT id, username, password, email FROM users WHERE username = ?username", _connection))
            {
                cmd.Parameters.AddWithValue("?username", name);
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

        public IEnumerable<ClientInformation> GetMultipleByUsername(string name)
        {
            using (MySqlCommand cmd = new MySqlCommand("SELECT id, username, password, email FROM users WHERE username LIKE '%?username%'", _connection))
            {
                cmd.Parameters.AddWithValue("?username", name);
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
        public void AddContact(uint user1, uint user2)
        {
            using (MySqlCommand cmd = new MySqlCommand("INSERT INTO contacts (user1, user2) VALUES (?user1, ?user2)", _connection))
            {
                if (user1 <= user2)
                {
                    cmd.Parameters.AddWithValue("?user1", user1);
                    cmd.Parameters.AddWithValue("?user2", user2);
                }
                else if (user1 > user2)
                {
                    cmd.Parameters.AddWithValue("?user1", user2);
                    cmd.Parameters.AddWithValue("?user2", user1);
                }

                cmd.ExecuteNonQuery();
            }
        }
        public void RemoveContact(uint user1, uint user2)
        {
            using (MySqlCommand cmd = new MySqlCommand("DELETE FROM contacts WHERE (user1 = ?user1 AND user2 = ?user2) OR (user1 = ?user2 AND user2 = ?user1)", _connection))
            {
                cmd.Parameters.AddWithValue("?user1", user1);
                cmd.Parameters.AddWithValue("?user2", user2);

                cmd.ExecuteNonQuery();
            }
        }


        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
