using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace restore_mysql_db
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                DoRestore();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private static void DoRestore()
        {
            int count = 0;
            string folder = "D:\\backup_folder";
            string connstr = "server=localhost;user=root;pwd=;";
            string[] files = Directory.GetFiles(folder);


            using (MySqlConnection conn = new MySqlConnection(connstr))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    conn.Open();
                    cmd.Connection = conn;

                    foreach (string file in files)
                    {
                        string db = Path.GetFileNameWithoutExtension(file);

                        cmd.CommandText = "create database if not exists `" + db + "`";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = $"use `{db}`";
                        cmd.ExecuteNonQuery();

                        using (MySqlBackup mb = new MySqlBackup(cmd))
                        {
                            mb.ImportFromFile(file);
                        }

                        count++;
                    }

                    conn.Close();
                }
            }
        }
    }
}
