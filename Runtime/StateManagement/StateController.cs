
namespace SisyphusLab
{
    public interface IStateController<TContext> 
    {
        TContext State { get; }
    }
}
