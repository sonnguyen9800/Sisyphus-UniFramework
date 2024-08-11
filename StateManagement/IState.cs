namespace SisyphusLab
{
    public interface IState<TContext> 
    {
        void Handle(TContext context);
    }
}
