
namespace SisyphusLab
{
    public abstract class StateContext<TState, TController, TSelfContext> 
        where TState : IState<TController>
    {

        public TState CurrentState { get; }

        public TController _controller;
        public TController Controller { get => _controller; }

        //Constructor
        public StateContext(TController controller)
        {
            _controller = controller;
        }
        public void Transition() {
            CurrentState.Handle(_controller);
        }
        public void Transition(TState state) {
        
        }
    }

}
