namespace SisyphusLab.Composites
{
    public interface IBehavior
    {
        public void Execute(object param = null);
    }

    public interface IBehavior<T>
    where T : struct
    {
        public void Execute(T param);

    }
}