using System.Collections;

namespace Raincrow.StateMachines
{
    public interface IStateMachine<T>
    {
        IEnumerator Start(IState<T> initialState);        
        void UpdateState(IState<T> state, float deltaTime);
        IEnumerator ChangeState(IState<T> state);
        IEnumerator Stop();
    }
}
