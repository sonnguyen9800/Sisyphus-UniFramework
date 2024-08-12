namespace SisyphusLab
{
    public interface IState<TController> 
    {
        void Handle(TController context);
    }
}
