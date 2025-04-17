using System.Collections.Generic;

namespace SisyphusFramework.GUI.Popup
{
    
    public enum State
    {
        Ready,
        Push,
        Pop
    }
    public delegate void PoppedDelegate(string name);
    public delegate void PushedDelegate(Popup popup);
    public delegate void ScreenDelegate (Popup popup);
    public abstract class QueuedAction
    {
        public string PopupName;
    }
    public class QueuedActionPush : QueuedAction
    {
        public PopupParamData Data;
        public string PrefabName;
        public PushedDelegate Callback;

        public override string ToString()
        {
            return string.Format("[Push] {0}", PrefabName);
        }
    }


    public class QueuedActionPop : QueuedAction
    {
        public PoppedDelegate callback;

        public override string ToString()
        {
            return string.Format("[Pop] {0}", PopupName);
        }
    }
        public class PopupParamData
        {
            private Dictionary<string, object> _data;

            public PopupParamData ()
            {
                _data = new Dictionary<string, object>();
            }

            public PopupParamData (int capacity)
            {
                _data = new Dictionary<string, object>(capacity);
            }

            public void Add(string key, object data)
            {
                _data.Add(key, data);
            }

            public T Get<T>(string key)
            {
                object datum = Get(key);

                try
                {
                    return (T)datum;
                }
                catch
                {
                    throw new System.Exception(string.Format("[BlitzyUI.Screen.Data] Could not cast data object '{0}' to type '{1}'", key, typeof(T).Name));
                }
            }

            public object Get (string key)
            {
                object datum;

                if (!_data.TryGetValue(key, out datum))
                    throw new System.Exception(string.Format("[BlitzyUI.Screen.Data] No object found for key '{0}'", key));

                return datum;
            }

            public bool TryGet (string key, out object datum)
            {
                return _data.TryGetValue(key, out datum);
            }

            public bool TryGet<T> (string key, out T datum)
            {
                object datumObj;

                if (_data.TryGetValue(key, out datumObj))
                {
                    try
                    {
                        datum = (T)datumObj;
                        return true;
                    }
                    catch
                    {
                        throw new System.Exception(string.Format("[BlitzyUI.Screen.Data] Could not cast data object '{0}' to type '{1}'", key, typeof(T).Name));
                    }
                }

                datum = default(T);
                return false;
            }
        }
}