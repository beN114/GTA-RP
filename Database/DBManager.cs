﻿using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using GTA_RP.Misc;
using GTA_RP.Database;
using IniParser;
using IniParser.Model;

namespace GTA_RP
{
    class DBManager : Singleton<DBManager>
    {
        public DBManager() { }

        private string databaseName = string.Empty;
        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        public string Password { get; set; }
        private MySqlConnection connection = null;
        public MySqlConnection Connection
        {
            get { return connection; }
        }

        /// <summary>
        /// Executes a raw query
        /// </summary>
        /// <param name="query">Query string</param>
        /// <returns>MysqlCommand</returns>
        public static MySqlCommand SimpleQuery(String query)
        {
            DBManager.Instance().IsConnect();
            return new MySqlCommand(query, DBManager.Instance().connection);
        }


        /// <summary>
        /// Checks if table is empty
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <returns>True if empty, false if not empty</returns>
        public static Boolean IsTableEmpty(String tableName)
        {
            bool isEmpty = true;
            string queryString = "SELECT * FROM " + tableName;
            DBManager.SelectQuery(queryString, (MySqlDataReader reader) =>
            {
                isEmpty = !reader.HasRows;
            }).Execute();
            return isEmpty;
        }

        
        /// <summary>
        /// Database query wrapper for select statement
        /// </summary>
        /// <param name="code">Code block to run</param>
        /// <param name="query">Query string</param>
        /// <param name="values">Key value pairs</param>
        public static SelectQuery SelectQuery(String query, Action<MySqlDataReader> code)
        {
            DBManager.Instance().IsConnect();
            return new SelectQuery(query, code);
        }

        /// <summary>
        /// Database query wrapper for update statement
        /// </summary>
        /// <param name="query">Query string</param>
        /// <returns>Query object</returns>
        public static Query UpdateQuery(String query)
        {
            DBManager.Instance().IsConnect();
            return new NormalQuery(query);
        }

        /// <summary>
        /// Database query wrapper for insert statement
        /// </summary>
        /// <param name="query">Query string</param>
        /// <returns>Query object</returns>
        public static Query InsertQuery(String query)
        {
            DBManager.Instance().IsConnect();
            return new NormalQuery(query);
        }

        /// <summary>
        /// Database query wrapper for delete statement
        /// </summary>
        /// <param name="query">Query string</param>
        /// <returns>Query object</returns>
        public static Query DeleteQuery(String query)
        {
            return InsertQuery(query);

        }
        /// <summary>
        /// Loads connection info from configuration file
        /// </summary>
        /// <returns>Connection string</returns>
        private String LoadConnectionInfo()
        {
            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.ReadFile("resources/GTA-RP/Config/Config.ini");
            string dbName = data["database"]["dbname"];
            string username = data["database"]["username"];
            string password = data["database"]["password"];
            string server = data["database"]["server"];
            return "Server=" + server + "; database=" + dbName + "; UID=" + username + "; password=" + password;
        }
        
        /// <summary>
        /// Checks if database connection is open
        /// </summary>
        /// <returns>Returns true if connection is open or if it has been opened</returns>
        public bool IsConnect()
        {
            bool result = true;
            if (Connection == null)
            {
                if (String.IsNullOrEmpty(databaseName)) result = false;
                string connstring = string.Format(LoadConnectionInfo(), databaseName);
                connection = new MySqlConnection(connstring);
                connection.Open();
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Closes the database connection
        /// </summary>
        public void Close()
        {
            connection.Close();
        }
    }
}
