
namespace SisyphusFramework
{
    public interface IStateController<TContext> 
    {
        TContext State { get; }
    }
}
