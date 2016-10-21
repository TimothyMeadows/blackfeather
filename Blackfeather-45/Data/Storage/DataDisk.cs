using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Blackfeather.Extention;

namespace Blackfeather.Data.Storage
{
    public class DataDisk : IDisposable
    {
        private SQLiteConnection _connection;

        public DataDiskMaster Master { get; private set; }

        public DataDisk(string path, bool overwrite = false)
        {
            if (overwrite)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }

            if (!File.Exists(path))
            {
                SQLiteConnection.CreateFile(path);
            }

            _connection = new SQLiteConnection($"Data Source={path};Compress=False;");
            _connection.Open();

            Master = new DataDiskMaster();
            var master = ExecuteQuery("SELECT label, size, indexed FROM 'master';");
            if (master.Tables.Count != 1 || master.Tables[0].Rows.Count != 1)
            {
                return;
            }

            Master.Label = master.Tables[0].Rows[0]["label"].ToString();
            Master.Size = Convert.ToInt32(master.Tables[0].Rows[0]["size"].ToString());
            Master.Indexed = Convert.ToBoolean(Convert.ToInt16(master.Tables[0].Rows[0]["indexed"].ToString()));
        }

        public void Format(DataDiskFormat format, bool compress = true, DataDiskEncryption encryption = null)
        {
            ExecuteNonQuery("DROP TABLE IF EXISTS 'master';");
            ExecuteNonQuery("CREATE TABLE 'master' (label TEXT NOT NULL, size INTEGER NOT NULL, indexed NUMERIC NOT NULL);");
            ExecuteNonQuery("DROP TABLE IF EXISTS 'directories';");
            ExecuteNonQuery("CREATE TABLE 'directories' (id INTEGER PRIMARY KEY AUTOINCREMENT, uid TEXT NOT NULL, parent INTEGER, name TEXT NOT NULL, created NUMERIC NOT NULL, modified NUMERIC, accessed NUMERIC);");
            ExecuteNonQuery("DROP TABLE IF EXISTS 'files';");
            ExecuteNonQuery("CREATE TABLE 'files' (id INTEGER PRIMARY KEY AUTOINCREMENT, uid TEXT NOT NULL, parent INTEGER, name TEXT NOT NULL, data BLOB, created NUMERIC NOT NULL, modified NUMERIC, accessed NUMERIC, FOREIGN KEY(parent) REFERENCES directories(id));");

            ExecuteNonQuery("INSERT INTO 'master' (label, size, indexed) VALUES (@label, @size, @indexed)", 
                new SQLiteParameter("@label", format.Label),
                new SQLiteParameter("@size", format.Size),
                new SQLiteParameter("@indexed", format.Indexed)
            );

            Master = new DataDiskMaster
            {
                Label = format.Label,
                Size = format.Size,
                Indexed = format.Indexed
            };

            if (!format.Indexed)
            {
                return;
            }

            ExecuteNonQuery("DROP INDEX IF EXISTS 'directory_uid_idx';");
            ExecuteNonQuery("CREATE UNIQUE INDEX 'directory_uid_idx' ON 'directories' (id, uid);");
            ExecuteNonQuery("DROP INDEX IF EXISTS 'directory_parent_idx';");
            ExecuteNonQuery("CREATE INDEX 'directory_parent_idx' ON 'directories' (id, parent);");
            ExecuteNonQuery("DROP INDEX IF EXISTS 'directory_parent_name_idx';");
            ExecuteNonQuery("CREATE INDEX 'directory_parent_name_idx' ON 'directories' (id, parent, name);");
            ExecuteNonQuery("DROP INDEX IF EXISTS 'file_uid_idx';");
            ExecuteNonQuery("CREATE UNIQUE INDEX 'file_uid_idx' ON 'files' (id, uid);");
            ExecuteNonQuery("DROP INDEX IF EXISTS 'file_uid_name_idx';");
            ExecuteNonQuery("CREATE INDEX 'file_uid_name_idx' ON 'files' (id, uid, name);");
        }

        public bool DirectoryExist(string path)
        {
            //var parts = ExtractPath(path);
            //var dir = ExecuteQuery("SELECT uid FROM 'directories' WHERE name = @name", new SQLiteParameter("@name", name));
            return false;
        }

        public void DirectoryCreate(string path)
        {
            var timeStamp = DateTime.UtcNow.ToUnixTime();
            var directory = new DataDiskDirectory()
            {
                Uid = Guid.NewGuid().ToString(),
                Name = "",
                Parent = "",
                Created = timeStamp,
                Modified = timeStamp,
                Accessed = timeStamp
            };
        }

        private List<string> ExtractPath(string path)
        {
            if (!path.Contains('/'))
            {
                return new List<string>(new []{path});
            }

            var pathParts = path.Split('/');
            return pathParts.ToList();
        }

        private void TraversePath(string path, string parent = null)
        {
            // ...
        }

        private void ExecuteNonQuery(string query)
        {
            var nonQuery = _connection.CreateCommand();
            nonQuery.CommandText = query;
            nonQuery.ExecuteNonQuery();
            nonQuery.Dispose();
        }

        private void ExecuteNonQuery(string query, params SQLiteParameter[] parameters)
        {
            var nonQuery = _connection.CreateCommand();
            nonQuery.CommandText = query;
            nonQuery.Parameters.AddRange(parameters);
            nonQuery.ExecuteNonQuery();
            nonQuery.Dispose();
        }

        private DataSet ExecuteQuery(string query)
        {
            var returnQuery = new SQLiteDataAdapter(query, _connection);
            var ds = new DataSet();

            ds.Reset();
            returnQuery.Fill(ds);
            returnQuery.Dispose();

            return ds;
        }

        private DataSet ExecuteQuery(string query, params SQLiteParameter[] parameters)
        {
            var returnQuery = new SQLiteDataAdapter(query, _connection);
            var ds = new DataSet();

            ds.Reset();
            returnQuery.SelectCommand.Parameters.AddRange(parameters);
            returnQuery.Fill(ds);
            returnQuery.Dispose();

            return ds;
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
            _connection = null;
        }
    }
}