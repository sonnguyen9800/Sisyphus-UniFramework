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

        protected APlayerData(string savePath, string modelKey, bool encrypt = true)
        {
            this._savePath = savePath;
            this._modelKey = modelKey;
            this.encrypt = encrypt;
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
        
        //called from scene using built in unity events
        public void LoadData()
        {
            Gley.AllPlatformsSave.API.Load<T>(_savePath, DataWasLoaded, encrypt);
        }


        private void DataWasLoaded(T data, SaveResult result, string message)
        {

            if (result == SaveResult.EmptyData || result == SaveResult.Error)
            {
                Data = new T();
                throw new Exception("Empty Data, Create new Data");
            }

            if (result == SaveResult.Success)
            {
                Data = data;
            }
        }

    }
}