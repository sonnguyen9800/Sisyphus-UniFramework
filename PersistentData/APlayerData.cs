using System;
using Gley.AllPlatformsSave;

namespace SisyphusLab.PersistentData
{
    public abstract class APlayerData<T> where T : class, new()
    {
        private readonly string _savePath;
        private string _modelKey;
        public T Data { get; set; }
        bool encrypt = false;

        protected APlayerData(string savePath, string modelKey)
        {
            this._savePath = savePath;
            this._modelKey = modelKey;
        }
        public void SaveData()
        {
            if (string.IsNullOrEmpty(_modelKey) || string.IsNullOrEmpty(_savePath))
            {
                throw new Exception("No ModelKey or SavePath");
            }
            Gley.AllPlatformsSave.API.Save<T>(Data, _savePath, DataSavedCallback, encrypt);
        }

        private void DataSavedCallback(SaveResult result, string message)
        {
            if (result == SaveResult.Error)
            {
                throw new Exception("Error saving data $" + message);
            }
            
            
        }
    }
}