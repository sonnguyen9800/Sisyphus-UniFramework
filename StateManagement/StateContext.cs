
namespace SisyphusLab
{
    public abstract class StateContext<TState, TController, TSelfContext> 
        where TState : IState<TSelfContext>
    {

        public TState CurrentState { get; }

        public TController Controller { get; set; }

        public void Transition() {}
        public void Transition(TState state) { }
    }

}
