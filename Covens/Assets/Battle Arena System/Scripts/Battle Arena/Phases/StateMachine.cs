using System.Collections;

namespace Raincrow.StateMachines
{
    public class StateMachine<T> : IStateMachine<T>
    {
        private readonly IState<T>[] _states;
        private readonly T _context;
        private int _currentStateIndex = -1;
        private bool _enabled;

        public StateMachine(T context, IState<T>[] states)
        {
            _context = context;
            _states = states;
        }

        public IEnumerator Start(IState<T> initialState)
        {
            int nextStateIndex = System.Array.IndexOf(_states, initialState);
            _currentStateIndex = nextStateIndex;
            yield return _states[_currentStateIndex].Enter(_context);

            _enabled = true;
        }

        public IEnumerator ChangeState(IState<T> state)
        {
            _enabled = false;

            int nextStateIndex = System.Array.IndexOf(_states, state);
            yield return _states[_currentStateIndex].Exit(_context);

            _currentStateIndex = nextStateIndex;
            yield return _states[_currentStateIndex].Enter(_context);

            _enabled = true;
        }

        public void UpdateState(IState<T> state, float deltaTime)
        {
            if (_enabled)
            {
                _states[_currentStateIndex].Update(_context, deltaTime);
            }            
        }

        public IEnumerator Stop()
        {
            _enabled = false; 

            yield return _states[_currentStateIndex].Exit(_context);
            _currentStateIndex = -1;
        }
    }

    public interface IStateMachine<T>
    {
        IEnumerator Start(IState<T> initialState);
        void UpdateState(IState<T> state, float deltaTime);
        IEnumerator ChangeState(IState<T> state);
        IEnumerator Stop();
    }

    public interface IState<T>
    {
        IEnumerator Enter(T context);
        void Update(T context, float deltaTime);
        IEnumerator Exit(T context);
    }
}