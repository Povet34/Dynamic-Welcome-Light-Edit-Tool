using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;
using Mono.Data.Sqlite;
using Common.Utility;

namespace Neofect.BodyChecker.DB
{
    public class LocalDBMngr : AbstractSingleton<LocalDBMngr>
    {
        private enum eLocalDBType
        {
            None = -1,
            Account,
            Patient,
        }

        public string MyUniqueKey { get {return SystemInfo.deviceUniqueIdentifier;}}

        private const string DUMMY_KEY = "0000";

        private SqliteConnection localDBConnection;

        private void Start()
        {
            StartCoroutine(CreateAccountDB());
        }

        private void OnApplicationQuit()
        {
            CloseLocalDBConnection();
        }

        #region Account DB : --------------------------------------------------
        private const string ACCOUNT_FILE_NAME = "account.db";
        private const string ACCOUNT_FOLDER_NAME = "\\Account";

        public class AccountInfo
        {
            private string id;
            public string Id { get { return id; } set { id = value; } }

            private string password;
            public string Password { get { return password; } set { password = value; } }

            public AccountInfo() { }
            public AccountInfo(string id, string password)
            {
                this.id = id;
                this.password = password;
            }
        }

        public AccountInfo AuthAccount()
        {
            AccountInfo accountInfo = null;
            string query = $"select * from AccountInfo";
            ExcuteQueryAtLocalDB(eLocalDBType.Account, query, (IDataReader dataReader) =>
            {
                while (dataReader.Read())
                {
                    var r = new AccountInfo();
                    r.Id = dataReader.IsDBNull(0) ? string.Empty : dataReader.GetString(0);
                    r.Password = dataReader.IsDBNull(1) ? string.Empty : dataReader.GetString(1);
                    accountInfo = r;
                }
            });
            return accountInfo;
        }

        public void InsertAccountInfo(AccountInfo info)
        {
            string query = $"insert into " +
                        $"AccountInfo(ID, PASSWORD) " +
                        $"values('{info.Id}', '{info.Password}')";
            ExcuteQueryAtLocalDB(eLocalDBType.Account, query, null);
        }

        public void UpdateAccountInfo(AccountInfo info)
        {
            string query = $"update AccountInfo set " +
                        $"ID = '{info.Id}', " +
                        $"PASSWORD = '{info.Password}' ";
            ExcuteQueryAtLocalDB(eLocalDBType.Account, query, null);
        }

        private IEnumerator CreateAccountDB()
        {
            // db 저장할 폴더 생성.
            PathUtility.CreateFolder(PathUtility.GetLocalDBFolder(ACCOUNT_FOLDER_NAME));

            // db 파일 복사
            var filePath = GetLocalDBFilePath(eLocalDBType.Account);
            if (File.Exists(filePath) == false)
            {
                File.Copy(Application.streamingAssetsPath + "\\" + ACCOUNT_FILE_NAME, GetLocalDBFilePath(eLocalDBType.Account));
                ChangeLocalDBPassword(DUMMY_KEY, MyUniqueKey, eLocalDBType.Account);
            }

            yield return null;
        }
        #endregion

        private void ExcuteQueryAtLocalDB(eLocalDBType localDBType, string query, Action<IDataReader> action)
        {
            try
            {
                OpenLocalDBConnetion(localDBType);
                using (var dbCommand = localDBConnection.CreateCommand())
                {
                    dbCommand.CommandText = query;
                    using (var dataReader = dbCommand.ExecuteReader())
                        action?.Invoke(dataReader);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        private void OpenLocalDBConnetion(eLocalDBType localDBType)
        {
            if (localDBConnection == null)
            {
                try
                {
                    localDBConnection = new SqliteConnection(GetConnectionUrl(localDBType));
                    localDBConnection.SetPassword(MyUniqueKey);
                    localDBConnection.Open();
                }
                catch
                {
                    File.Copy(Application.streamingAssetsPath + "\\" + ACCOUNT_FILE_NAME, GetLocalDBFilePath(eLocalDBType.Account), true);
                    ChangeLocalDBPassword(DUMMY_KEY, MyUniqueKey, eLocalDBType.Account);
                    CloseLocalDBConnection();
                }
            }
        }

        private void CloseLocalDBConnection()
        {
            if (localDBConnection != null)
            {
                localDBConnection.Close();
                localDBConnection = null;
            }
        }

        private void ChangeLocalDBPassword(string before, string after, eLocalDBType localDBType)
        {
            using (var dbConnection = new SqliteConnection(GetConnectionUrl(localDBType)))
            {
                dbConnection.SetPassword(before);
                dbConnection.Open();
                dbConnection.ChangePassword(after);
                dbConnection.Close();
            }
        }

        private string GetConnectionUrl(eLocalDBType localDBType)
        {
            switch (localDBType)
            {
                case eLocalDBType.Account: return "URI=file:" + GetLocalDBFilePath(eLocalDBType.Account);
                default: return string.Empty;
            }
        }

        private string GetLocalDBFilePath(eLocalDBType localDBType)
        {
            switch (localDBType)
            {
                case eLocalDBType.Account: return $"{PathUtility.GetLocalDBFolder(ACCOUNT_FOLDER_NAME)}\\{ACCOUNT_FILE_NAME}";
                default: return string.Empty;
            }
        }
    }
}