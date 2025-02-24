namespace SisyphusLab
{
    public interface IState<TController> 
    {
        void Handle();
        void Exit();
    }

    public abstract class State<TController>
    {
        public TController Controller { get; private set; }
        
        public State(TController controller)
        {
            Controller = controller;
        }
        public State(TController controller, object param)
        {
            Controller = controller;
        }
    }
}
