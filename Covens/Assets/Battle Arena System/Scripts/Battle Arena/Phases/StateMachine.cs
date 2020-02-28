using System.Collections;
using System.Collections.Generic;

namespace Raincrow.StateMachines
{
    public class StateMachine : IStateMachine
    {
        private readonly Dictionary<System.Type, IState> _states;
        private System.Type _currentStateType;
        private bool _enabled;

        public StateMachine(IState[] states)
        {
            _states = new Dictionary<System.Type, IState>();

            for (int i = 0; i < states.Length; i++)
            {
                _states.Add(states[i].GetType(), states[i]);
            }
        }

        public IEnumerator Start<R>() where R : IState
        {
            _currentStateType = typeof(R);
            yield return _states[_currentStateType].Enter(this);

            _enabled = true;
        }

        public IEnumerator ChangeState<R>() where R : IState
        {
            System.Type nextStateType = typeof(R);

            if (_enabled && nextStateType != _currentStateType) // cannot change to same state
            {
                _enabled = false;
                yield return _states[_currentStateType].Exit(this);

                _currentStateType = nextStateType;
                yield return _states[_currentStateType].Enter(this);

                _enabled = true;
            }
        }

        public IEnumerator UpdateState()
        {
            if (_enabled)
            {
                yield return _states[_currentStateType].Update(this);
            }            
        }

        public IEnumerator Stop()
        {
            if (_enabled)
            {
                _enabled = false;

                yield return _states[_currentStateType].Exit(this);

                _currentStateType = null;
            }
        }
    }

    public interface IStateMachine
    {        
        IEnumerator Start<R>() where R : IState;
        IEnumerator UpdateState();
        IEnumerator ChangeState<R>() where R: IState;
        IEnumerator Stop();
    }

    public interface IState
    {
        string Name { get; }
        IEnumerator Enter(IStateMachine stateMachine);
        IEnumerator Update(IStateMachine stateMachine);
        IEnumerator Exit(IStateMachine stateMachine);
    }
}