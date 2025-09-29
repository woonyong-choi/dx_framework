using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JCoroutines
    //		서버에서 코루틴 사용하기
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JCoroutines
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수/Property
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [변수] Routines
        private List<IEnumerator> _routines = new List<IEnumerator>();
        private List<float> _delays = new List<float>();
        #endregion

        #region [Property] Count / Running
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public int Count => _routines.Count;
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public bool Running => _routines.Count > 0;
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 업데이트
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [업데이트] 코루틴 실행
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Update(float deltaTime)
        {
            for (int i = 0; i < _routines.Count; i++)
            {
                if (_delays[i] > 0f)
                    _delays[i] -= deltaTime;
                else if (_routines[i] == null || !MoveNext(_routines[i], i))
                {
                    _routines.RemoveAt(i);
                    _delays.RemoveAt(i--);
                }
            }
        }
        #endregion

        #region [업데이트] [구현] MoveNext
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        bool MoveNext(IEnumerator routine, int index)
        {
            if (routine.Current is IEnumerator)
            {
                if (MoveNext((IEnumerator)routine.Current, index))
                    return true;

                _delays[index] = 0f;
            }

            bool result = routine.MoveNext();

            if (routine.Current is float)
                _delays[index] = (float)routine.Current;

            return result;
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 초기화
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Coroutine] Start
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Start(IEnumerator routine)
        {
            _routines.Add(routine);
            _delays.Add(0f);
        }
        #endregion

        #region [Coroutine] StopAll
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void StopAll()
        {
            while (_routines.Count > 0)
                _routines.RemoveAt(0);
            while (_delays.Count > 0)
                _delays.RemoveAt(0);
        }
        #endregion

        #region [Coroutine] Stop
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Stop(IEnumerator routine)
        {
            for (var i = 0; i < _routines.Count; ++i)
            {
                if (_routines[i] == routine)
                {
                    _routines.RemoveAt(i);
                    _delays.RemoveAt(i);
                    break;
                }
            }
        }
        #endregion

        #region [Coroutine] Pause
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static IEnumerator Pause(float time)
        {
            var watch = Stopwatch.StartNew();
            while (watch.Elapsed.TotalSeconds < time)
                yield return 0;
        }
        #endregion        
    }



    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JCo_routinesHelper (Sample)
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JCo_routinesHelper
    {
        #region [Sample] [변수] poem
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //"Death" by John Donne
        const string poem = "\"Death\" by John Donne\n\n" +
                            "Death be not proud, though some have called thee\n" +
                            "Mighty and dreadfull, for, thou art not so,\n" +
                            "For, those, whom thou think'st, thou dost overthrow,\n" +
                            "Die not, poore death, nor yet canst thou kill me.\n" +
                            "From rest and sleepe, which but thy pictures bee,\n" +
                            "Much pleasure, then from thee, much more must flow,\n" +
                            "And soonest our best men with thee doe goe,\n" +
                            "Rest of their bones, and soules deliverie.\n" +
                            "Thou art slave to Fate, Chance, kings, and desperate men,\n" +
                            "And dost with poyson, warre, and sicknesse dwell,\n" +
                            "And poppie, or charmes can make us sleepe as well,\n" +
                            "And better then thy stroake; why swell'st thou then;\n" +
                            "One short sleepe past, wee wake eternally,\n" +
                            "And death shall be no more; death, thou shalt die.";
        #endregion

        #region [Sample] ReadPoem
        //------------------------------------------------------------------------------------------------------------------------------------------------------               
        static IEnumerator ReadPoem(string poem)
        {
            //Read the poem letter by letter
            foreach (var letter in poem)
            {
                Console.Write(letter);
                switch (letter)
                {
                    //Pause for punctuation
                    case ',':
                    case ';':
                        yield return Pause(0.5f);
                        break;

                    //Long pause for full-stop
                    case '.':
                        yield return Pause(1);
                        break;

                    //Short pause for anything else
                    default:
                        yield return Pause(0.05f);
                        break;
                }
            }

            //Wait for user input to close
            Console.WriteLine("\nPress any key to End");
            Console.ReadLine();
        }

        public static IEnumerator Pause(float time)
        {
            var watch = Stopwatch.StartNew();
            while (watch.Elapsed.TotalSeconds < time)
                yield return 0;
        }
        #endregion

    }

}
