using System.Collections;
using System.Collections.Generic;

namespace Raincrow.StateMachines
{
    public class StateMachine<T> : IStateMachine<T>
    {
        private readonly Dictionary<System.Type, IState<T>> _states;
        private readonly T _context;
        private System.Type _currentStateType;
        private bool _enabled;        

        public StateMachine(T context, IState<T>[] states)
        {
            _context = context;

            _states = new Dictionary<System.Type, IState<T>>();

            for (int i = 0; i < states.Length; i++)
            {
                _states.Add(states[i].GetType(), states[i]);
            }
        }

        public IEnumerator Start<R>() where R : IState<T>
        {
            _currentStateType = typeof(R);
            yield return _states[_currentStateType].Enter(this, _context);

            _enabled = true;
        }

        public IEnumerator ChangeState<R>() where R : IState<T>
        {
            System.Type nextStateType = typeof(R);

            if (_enabled && nextStateType != _currentStateType) // cannot change to same state
            {
                _enabled = false;
                yield return _states[_currentStateType].Exit(this, _context);

                _currentStateType = nextStateType;
                yield return _states[_currentStateType].Enter(this, _context);

                _enabled = true;
            }
        }

        public IEnumerator UpdateState()
        {
            if (_enabled)
            {
                yield return _states[_currentStateType].Update(this, _context);
            }            
        }

        public IEnumerator Stop()
        {
            if (_enabled)
            {
                _enabled = false;

                yield return _states[_currentStateType].Exit(this, _context);

                _currentStateType = null;
            }
        }
    }

    public interface IStateMachine<T>
    {        
        IEnumerator Start<R>() where R : IState<T>;
        IEnumerator UpdateState();
        IEnumerator ChangeState<R>() where R: IState<T>;
        IEnumerator Stop();
    }

    public interface IState<T>
    {
        string Name { get; }
        IEnumerator Enter(IStateMachine<T> stateMachine, T context);
        IEnumerator Update(IStateMachine<T> stateMachine, T context);
        IEnumerator Exit(IStateMachine<T> stateMachine, T context);
    }
}