using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MonoGameToolkit
{
    public class Coroutines
    {
        private int _maxRoutinesConcurrent = int.MaxValue;
        public int MaxRoutinesConcurrent
        {
            get { return _maxRoutinesConcurrent; }
            set { _maxRoutinesConcurrent = value; }
        }

        public int CoroutineCount { get { return routines.Count; } }

        private List<KeyValuePair<string, List<IEnumerator>>?> routines = new List<KeyValuePair<string, List<IEnumerator>>?>();
        
        /// <summary>
        /// Runs a coroutine, use id to group coroutines together to allow for easy stopping certain coroutines.
        /// </summary>
        /// <param name="routine"></param>
        /// <param name="id"></param>
        public void Run(IEnumerator routine, string id = default(string))
        {
            KeyValuePair<string, List<IEnumerator>>? kvp = routines.SingleOrDefault(x => x.Value.Key == id);
            if (kvp.HasValue)
            {
                kvp.Value.Value.Add(routine);
            }
            else
            {
                List<IEnumerator> routineList = new List<IEnumerator>();
                routineList.Add(routine);
                routines.Add(new KeyValuePair<string, List<IEnumerator>>(id, routineList));
            }
        }
        
        public void Stop(string id)
        {
            KeyValuePair<string, List<IEnumerator>>? kvp = routines.SingleOrDefault(x => x.Value.Key == id);
            if(kvp.HasValue)
                routines.Remove(kvp);
        }

        public void StopAll()
        {
            routines.Clear();
        }

        internal void Update()
        {

            for (int i = 0; i < routines.Count; i++)
            {
                for (int j = 0; j < (routines[i].Value.Value.Count < MaxRoutinesConcurrent ? routines[i].Value.Value.Count : MaxRoutinesConcurrent); j++)
                {
                    if (routines[i].Value.Value[j].Current is IEnumerator)
                        if (MoveNext((IEnumerator)routines[i].Value.Value[j].Current))
                            continue;
                    if (!routines[i].Value.Value[j].MoveNext())
                        routines[i].Value.Value.RemoveAt(j--);
                }
            }

        } 

        private bool MoveNext(IEnumerator routine)
        {
            if (routine.Current is IEnumerator)
                if (MoveNext((IEnumerator)routine.Current))
                    return true;
            return routine.MoveNext();
        }
    }
}
