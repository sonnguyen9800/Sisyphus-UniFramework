namespace SisyphusFramework.Data
{
    public interface IVisualizer
    {
        public void ShowData();

    }
    public interface IVisualizerItem
    {
        public T ExposeData<T>();
        public string GetName();
        public string GetKey();
        public string[] GetColumes();

    }
}