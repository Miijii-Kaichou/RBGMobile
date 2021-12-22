using Extensions;
using System;
using System.Collections;

namespace UnityEngine
{
    [Serializable]
    public class Alarm
    {
        /// <summary>
        /// A class that represents a timer entry.
        /// </summary>

        public class Timer
        {
            /// <summary>
            /// Return the timer's current time
            /// </summary>
            public float CurrentTime { get; internal set; }

            /// <summary>
            /// Reset the time on this timer
            /// </summary>
            public float ResetTime { get; internal set; }

            /// <summary>
            /// Set the duration for this timer.
            /// </summary>
            public float SetDuration { get; internal set; }

            /// <summary>
            /// Has the timer started?
            /// </summary>
            public bool TimeStarted { get; internal set; }

            /// <summary>
            /// Do you want this timers to stop once it's hit its duration time?
            /// </summary>
            public bool StopOnFinish { get; internal set; }

            /// <summary>
            /// A Callback Method for when the timer has finished
            /// </summary>
            public FinishedTimeCallback TimerEvent { get; internal set; }

            /// <summary>
            /// The flow of time (1 for normal time, -1 for counting down);
            /// </summary>
            public int Flow { get; internal set; }

            /// <summary>
            /// Timer Constructor
            /// </summary>
            Timer()
            {
                CurrentTime = 0;
                ResetTime = 0;
                SetDuration = 0;
                TimeStarted = false;
                StopOnFinish = false;
                TimerEvent = null;
                Flow = 1;
            }

            /// <summary>
            /// Is this timer counting down?
            /// </summary>
            public bool IsCountDown
            {
                get
                {
                    return Flow == -1;
                }
                set
                {
                    if (value == true)
                        Flow = -1;
                    else
                        Flow = 1;
                }
            }

            /// <summary>
            /// Create a new Timer
            /// </summary>
            /// <returns></returns>
            public static Timer New()
            {
                return new Timer();
            }
        }

        internal void UpdateDuration(int index, float currentDuration)
        {
            registeredTimers[index].SetDuration = currentDuration;
        }

        #region Public Members
        public static Alarm timer;

        /// <summary>
        /// All registered timers for this alarm
        /// </summary>
        public Timer[] registeredTimers;

        public delegate void FinishedTimeCallback();

        /// <summary>
        /// Get a timer from this alarm that hasn't been used yet.
        /// </summary>
        public int Avaliable
        {
            get
            {
                for (int index = 0; index < size; index++)
                {
                    if (!registeredTimers[index].TimeStarted)
                    {
                        return index;
                    }
                }

                return -1;
            }
        }


        #endregion

        #region Private Members
        private int size;
        readonly private bool debugEnabled = false;
        readonly private bool showFloatingPoint = false;
        #endregion
        bool timeCycleInit = false;

        bool useUnscaledTime = false;

        /// <summary>
        /// Create an instance of an alarm.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="debug"></param>
        /// <param name="floatingPoint"></param>
        public Alarm(int size, bool debug = false, bool floatingPoint = false)
        {
            SetSize(size);
            debugEnabled = debug;
            showFloatingPoint = floatingPoint;
            switch (debug)
            {
                case false:
                    Initialize(GetSize());
                    break;
#if UNITY_EDITOR
                case true:
                    Initialize(GetSize());
                    Debug.Log("Timer(s) Initiated.");
                    Debug.Log("Timer size is " + GetSize() + ".");
                    break;
#endif //UNITY_EDITOR
            }
        }


        /// <summary>
        /// The cycle for counting up or down.
        /// </summary>
        /// <returns></returns>
        IEnumerator TimerCycle()
        {
            if (timeCycleInit == false)
            {
                timeCycleInit = true;
            }

            while (true)
            {
                RunTimers();
                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// Run all timers on this alarm {whether they've started or not).
        /// </summary>
        private void RunTimers()
        {
            #region Run Timers
            ////Current time, when initialized, is set to 0. At the start of the game, assign the value of current time to reset time.
            for (int index = 0; index < GetSize(); index++)
            {
                Timer timeEntry = registeredTimers[index];

                if (timeEntry.TimeStarted == true)
                {
                    timeEntry.CurrentTime += (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) * timeEntry.Flow;
#if UNITY_EDITOR
                    if (debugEnabled)
                    {
                        if (showFloatingPoint == false)
                            Debug.Log("Timer[" + index + "]; Current time: " + timeEntry.CurrentTime);
                        else
                            Debug.Log("Timer[" + index + "]; Current time: " + timeEntry.CurrentTime);
                    }

#endif //UNITY_EDITOR
                    //If the time is passed the set duration, and we are not counting down
                    if (timeEntry.CurrentTime >= registeredTimers[index].SetDuration && !timeEntry.IsCountDown)
                    {
                        //Reset the timer, and tell it to stop running one's it's been resetted
                        SetToZero(index, timeEntry.StopOnFinish);

#if UNITY_EDITOR
                        //If the player enabled debug when creating this instance, return this to the console
                        if (debugEnabled) Debug.Log("Timer[" + index + "] returned a value of 1!");
#endif //UNITY_EDITOR

                        //Check if there's a timer event. If there is, call the event
                        if (timeEntry.TimerEvent != null)
                        {
                            timeEntry.TimerEvent.Invoke();
                        }
                    }

                    //However, if we are less than or equal to zero, and the timer is counting down
                    else if (timeEntry.CurrentTime <= 0 && timeEntry.IsCountDown)
                    {
                        //We want to reset it and check if we want it to continue running or not after the reset
                        SetToZero(index, timeEntry.StopOnFinish);

                        //If there's a timer event for this timer, invoke or call that event.
                        if (timeEntry.TimerEvent != null)
                        {
                            timeEntry.TimerEvent.Invoke();
                        }
                    }
                }

                // If the timer hasn't been started at all, make sure to keep it at a zero
                else
                {
                    registeredTimers[index].CurrentTime = 0f;
                }
            }
            #endregion
        }

        /// <summary>
        /// Discard the object from all it's registered time entries.
        /// </summary>
        internal void Discard()
        {
            registeredTimers = null;
            size = default;
            TimerCycle().Stop();
        }

        ///<summary>
        ///Activate a timer by index
        ///</summary>
        public void Start(int index, float duration, bool isCountDown)
        {
            //Starts the timer
            registeredTimers[index].TimeStarted = true;
            registeredTimers[index].SetDuration = duration;
            registeredTimers[index].CurrentTime = isCountDown ? duration : 0f;
            registeredTimers[index].IsCountDown = isCountDown;

            //Start Alarm Cycles
            if (timeCycleInit == false)
                TimerCycle().Start();


#if UNITY_EDITOR
            if (debugEnabled) Debug.Log("Timer[" + index + "]; Start!");
#endif //UNITY_EDITOR

        }

        ///<summary>
        ///Activate a timer by index
        ///</summary>
        public void Start(int index)
        {
            //Start the timer. There is no set duration, so have
            //the duration set to Infinity.
            Start(index, Mathf.Infinity, false);
        }

        ///<summary>
        ///Activate a timer by index
        ///</summary>
        public void Start(int index, float duration)
        {
            //Set the timer. This is not a count down,
            //So just run the timer until it times out.
            Start(index, duration, false);
        }

        /// <summary>
        /// Activate a timer by index,
        /// and continue to update it's current time
        /// </summary>
        /// <param name="index"></param>
        /// <param name="currentTime"></param>
        public void Start(int index, ref float duration)
        {
            Start(index, duration, false);
        }

        /// <summary>
        /// Stops a timer if it's running
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float Stop(int index)
        {
            //Starts the timer
            registeredTimers[index].TimeStarted = false;

#if UNITY_EDITOR
            if (debugEnabled) Debug.Log("Timer[" + index + "]; Stop!");
#endif //UNITY_EDITOR
            return registeredTimers[index].CurrentTime;
        }

        /// <summary>
        /// Set a timer to zero
        /// </summary>
        public void SetToZero(int index, bool stop = false)
        {
            registeredTimers[index].CurrentTime = 0f;
            if (stop == true)
            {
                registeredTimers[index].TimeStarted = false;

#if UNITY_EDITOR
                if (debugEnabled) Debug.Log("Timer [" + index + "]; stopped");
#endif //UNITY_EDITOR
            }
        }

        /// <summary>
        /// Set's a timer for a set duration of time.
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="index"></param>
        /// <param name="stop"></param>
        /// <param name="timerEvent"></param>
        public void SetFor(float duration, int index, bool stop = false, FinishedTimeCallback timerEvent = null)
        {
            if (registeredTimers[index].TimeStarted == false)
            {
                Start(index, duration);

                //Configure the time for setting for a set duration
                registeredTimers[index].StopOnFinish = stop;
                registeredTimers[index].TimerEvent = timerEvent;

            }
        }

        /// <summary>
        /// Set's a timer for a set duration of time.
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="index"></param>
        /// <param name="stop"></param>
        /// <param name="timerEvent"></param>
        public void SetFor(ref float duration, int index, bool stop = false, FinishedTimeCallback timerEvent = null)
        {
            if (registeredTimers[index].TimeStarted == false)
            {
                Start(index, ref duration);

                //Configure the time for setting for a set duration
                registeredTimers[index].StopOnFinish = stop;
                registeredTimers[index].TimerEvent = timerEvent;

            }
        }

        /// <summary>
        /// Have the timer count down from a set duration.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="duration"></param>
        /// <param name="timerEvent"></param>
        public void CountDown(int index, float duration, FinishedTimeCallback timerEvent = null)
        {
            if (registeredTimers[index].TimeStarted == false)
            {
                Start(index, duration, true);

                registeredTimers[index].CurrentTime = registeredTimers[index].SetDuration;
                registeredTimers[index].StopOnFinish = true;
                registeredTimers[index].TimerEvent = timerEvent;

            }
        }

        /// <summary>
        /// Initialize all Pre-Defined Timers
        /// </summary>
        Alarm Initialize(int _size = 12)
        {
            //If this instance of a timer is null (or non-existant),
            //have this become our instance.
            if (timer == null)
            {
                timer = this;
            }

            #region Initiate Timers
            registeredTimers = new Timer[_size];

            //Current time, when initialized, is set to 0. At the start of the game, assign the value of current time to reset time.
            for (int index = 0; index < _size; index++)
            {
                registeredTimers[index] = Timer.New();
                registeredTimers[index].ResetTime = registeredTimers[index].CurrentTime;
            }
            #endregion

            return timer;
        }

        /// <summary>
        /// Set the size of a timer.
        /// </summary>
        /// <param name="_size"></param>
        void SetSize(int _size)
        {
            size = _size;
        }


        /// <summary>
        /// Return the set size of a created timer.
        /// </summary>
        /// <returns></returns>
        public int GetSize()
        {
            return size;
        }

        /// <summary>
        /// Set the Count-Up / Count-Down timer values to not rely on the engine's time scale (or the engine's flow of time).
        /// </summary>
        public void UseUnscaledTime() => useUnscaledTime = true;

        internal void SetAllZero(bool stop)
        {
            for (int index = 0; index < size; index++)
            {
                SetToZero(index, stop);
            }
        }
    }
}