using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using XChat2.Common.Configuration;

namespace XChat2.Server.Clients
{
    [Obsolete]
    class ClientDB
    {
        private static string ConnectionString;

        internal ClientDB(Config config)
        {
            if (ConnectionString == null)
            {
                StringBuilder sb = new StringBuilder();
                MySqlConnectionStringBuilder.AppendKeyValuePair(sb, "SERVER", config["mysql"].get<string>("server").Value);
                MySqlConnectionStringBuilder.AppendKeyValuePair(sb, "DATABASE", config["mysql"].get<string>("database").Value);
                MySqlConnectionStringBuilder.AppendKeyValuePair(sb, "UID", config["mysql"].get<string>("uid").Value);
                MySqlConnectionStringBuilder.AppendKeyValuePair(sb, "PASSWORD", config["mysql"].get<string>("password").Value);

                ConnectionString = sb.ToString();
            }
        }

        internal IEnumerable<Client> LoadAll()
        {
            using(MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                using(MySqlCommand cmd = new MySqlCommand("SELECT id, username, password, email FROM users", connection))
                {
                    using(MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            uint id = reader.GetUInt32("id");
                            string username = reader.GetString("username");
                            string password = reader.GetString("password");
                            string email = reader.GetString("email");

                            yield return new Client(id, username, password, email);
                        }
                    }
                }
            }
        }

        internal class Contact
        {
            private uint _client1;
            private uint _client2;
            internal uint Client1
            {
                get { return _client1; }
            }
            internal uint Client2
            {
                get { return _client2; }
            }

            internal Contact(uint client1, uint client2)
            {
                _client1 = client1;
                _client2 = client2;
            }
        }

        internal IEnumerable<Contact> LoadContacts()
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                using (MySqlCommand cmd = new MySqlCommand("SELECT user1, user2 FROM contacts", connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            uint client1 = reader.GetUInt32("user1");
                            uint client2 = reader.GetUInt32("user2");

                            yield return new Contact(client1, client2);
                        }
                    }
                }
            }
        }

        internal void SaveContacts(uint id, IEnumerable<uint> contacts)
        {
            this.RemoveContacts(id);

            if (contacts.Count() == 0)
                return;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                StringBuilder commandBuilder = new StringBuilder();
                commandBuilder.Append("INSERT INTO contacts (user1, user2) VALUES ");

                for (int i = 0; i < contacts.Count(); i++)
                {
                    uint contact = contacts.ElementAt(i);
                    commandBuilder.AppendLine(string.Format("({0}, {1}){2}", id < contact ? id : contact, id > contact ? id : contact, i < contacts.Count() - 1 ? "," : ""));

                }
                
                using (MySqlCommand cmd = new MySqlCommand(commandBuilder.ToString(), connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void RemoveContacts(uint id)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                StringBuilder commandBuilder = new StringBuilder();

                using (MySqlCommand cmd = new MySqlCommand("DELETE FROM contacts WHERE user1 = ?user OR user2 = ?user", connection))
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("?user", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
