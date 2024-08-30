
namespace SisyphusLab
{
    public abstract class StateContext<TState, TController, TSelfContext> 
        where TState : IState<TController>
    {

        public TState CurrentState { get; private set; }
        public TController Controller { get => _controller; }

        private TController _controller;

        //Constructor
        public StateContext(TController controller, TState originalState)
        {
            _controller = controller;
            CurrentState = originalState;
        }

        public void Transition(TState state) {
            CurrentState.Exit(_controller);
            CurrentState = state;
            CurrentState.Handle(_controller);
        }
    }

}
