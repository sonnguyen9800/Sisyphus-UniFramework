namespace SisyphusFramework.Data
{
    public interface IPlayModel
    {
        public byte GetId();
        public void SetId(byte id);
        byte this[byte index] { get; set; }
    }
    
    public interface IPlayData<T> where T : struct
    {
        public T GetId();
        public void SetId(T id);
        T this[T index] { get; set; }
        
        
    }
}