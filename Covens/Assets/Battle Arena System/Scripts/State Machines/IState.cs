using System.Collections;

namespace Raincrow.StateMachines
{
    public interface IState<T>
    {
        IEnumerator Enter(T context);
        void Update(T context, float deltaTime);
        IEnumerator Exit(T context);
    }
}
