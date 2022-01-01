using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

#region Extensions
namespace Extensions
{
    /// <summary>
    /// Despite it says constants, it harbors both
    /// constant values, and read-only values
    /// </summary>
    public static class Constants
    {
        #region Constants
        public const int MIN_MIXER_ATTENUATION = -80;
        public const int DEFAULT_MIXER_ATTENUATION = 0;
        public const int MAX_MIXER_ATTENUATION = 20;
        public const int HUNDRED = 100;
        public const int ZERO = 0;
        public const int ONE = 1;
        public const float HALF = 0.5f;
        public const float QUARTER = 0.25f;
        public const bool ALLOW = true, ENABLE = true, YES = true, ACTIVE = true;
        public const bool PROHIBIT = false, DISABLE = false, NO = false, INACTIVE = false;
        public const char SEMICOLON = ';';
        public const char SPACE_CHAR = ' ';
        public const object NULL_OBJ = null;
        public const float HIGH_REVERB_LEVEL = 1000.00f;
        public const float LOW_REVERB_LEVEL = 500.00f;
        public const float LOW_PITCH = 0.975f;
        public const float NORMAL_PITCH = 1f;
        public const int TWO = 2;
        public const string EMPTY = "";
        public const string UNKNOWN = "???";
        #endregion
    }

    public static class Convenience
    {
        /// <summary>
        /// Will return a value if method used was successful.
        /// otherwise, return a fallback value if unsuccessful.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method">Method to validated</param>
        /// <param name="fallback">Value to fallback to when method falls.</param>
        /// <remarks>The fallback value should be treated as the default values of a specified type.</remarks>
        /// <returns></returns>
        public static T GetIfSuccessful<T>(Func<T> method, T fallback, bool giveReason = Constants.PROHIBIT)
        {
            try
            {
                T attempt = method.Invoke();
                return attempt;
            }
            catch (Exception e)
            {
                if (giveReason)
                    Debug.LogError($"REASON FOR UNSUCCESSFUL GET OPERATION: {e.Message}");

                return fallback;
            }
        }

        public static void Nullify(this object obj) => obj = null;

        public static T GetIfSuccessful<T>(Func<T> method, Condition condition, T fallback, bool giveReason = Constants.PROHIBIT)
        {
            T attempt = GetIfSuccessful(method, fallback, giveReason);
            if (condition.WasMet)
                return attempt;

            return fallback;
        }

        public static T OnlyIfNull<T>(this T obj)
        {
            if (obj != null) return (T)obj;
            return default;
        }

        public static Sign ToSign(this int value)
        {
            return value < 0 ? Sign.Negative : value > 0 ? Sign.Positive : Sign.Zero;
        }

        public static int ToAbsolute(this int value)
        {
            return value < 0 ? value * -1 : value;
        }

        public static int DigitLength(this int value)
        {

            return value.ToString().Length;
        }

        public static int DigitPlace(this int value)
        {
            if (value < 0) return 0;

            int placement = 1;
            const int TEN = 10;
            for (int i = 0; i < value.DigitLength(); i++)
            {
                placement *= i == 0 ? placement : TEN;
            }

            return placement;
        }

        public static void Load(this UnityEngine.Object scene)
        {
            if (scene.GetType() == typeof(Scene))
                GameSceneManager.LoadScene(scene.CastTo<Scene>().buildIndex);
        }

        public static int ZeroBased(this int value)
        {
            return value - 1;
        }

        public static float ZeroBased(this float value)
        {
            return value - 1f;
        }

        public static double ZeroBased(this double value)
        {
            return value - 1d;
        }

        public static int OneBased(this int value)
        {
            return value + 1;
        }

        public static float OneBased(this float value)
        {
            return value + 1f;
        }

        public static double OneBased(this double value)
        {
            return value + 1d;
        }

        public static bool ToBool(this int value)
        {
            return value == 1 ? true : false;
        }

        public static bool ToBool(this object obj)
        {
            return (bool)obj;
        }

        public static bool ToBool(this string obj)
        {
            return obj.ToLower() == "false" ? false : true;
        }

        public static bool Give(this bool obj, ref bool sharedObject)
        {
            sharedObject = obj;
            return sharedObject;
        }

        public static bool Is(this object _, Type character)
        {
            return _.GetType().Equals(character);
        }

        public static int ToInt(this string _)
        {
            return Convert.ToInt32(_);
        }

        public static T CastTo<T>(this object _)
        {
            T data = default;
            try
            {
                data = (T)_;
                return data;
            }
            catch
            {
                Debug.LogError($"Invalid casting of object {_}");
                return default;
            }
        }

        public static T[] GrabComponents<T>(this UnityEngine.Component[] _)
        {
            try
            {
                T[] data = new T[_.Length];
                for (int i = 0; i < _.Length; i++)
                {
                    data[i] = _[i].GetComponent<T>();
                }
                return data;
            }
            catch
            {
                return default;
            }
        }

        public static T[] GrabComponents<T>(this UnityEngine.GameObject[] _)
        {
            try
            {
                T[] data = new T[_.Length];
                for (int i = 0; i < _.Length; i++)
                {
                    data[i] = _[i].GetComponent<T>();
                }
                return data;
            }
            catch
            {
                return default;
            }
        }

        public static int Next(this ref int _)
        {
            _++;
            return _;
        }
    }

    /// <summary>
    /// Global Coroutine Extension
    /// </summary>
    /// <remarks>For best practices, only used this extension for classes not deriving from monobehviours, or cannot call "StartCoroutine"</remarks>
    public static class Coroutine
    {
        public static void Start(this IEnumerator enumerator)
        {
            if (enumerator == null) return;

            CoroutineHandler.Execute(enumerator);
        }

        public static void Stop(this IEnumerator enumerator)
        {
            if (enumerator == null) return;

            CoroutineHandler.Halt(enumerator);
        }

        public static void StopAll()
        {
            CoroutineHandler.ClearRoutines();
        }
    }

    [ImmutableObject(true), Serializable]
    public struct Var<T>
    {
        public T Value;

        public Var(T value)
        {
            Value = value;
        }

        public static Var<T> operator +(Var<T> a, Var<T> b)
        {
            return (a + b);
        }

        public static Var<T> operator -(Var<T> a, Var<T> b)
        {
            return (a - b);
        }

        public static implicit operator Var<T>(T value)
        {
            return new Var<T>(value);
        }
    }

    public static class Dictionary
    {
        public static K GetKey<K, V>(this Dictionary<K, V> keyValuePairs, V value)
        {
            foreach (KeyValuePair<K, V> keyValuePair in keyValuePairs)
            {
                if (value.ToString() == keyValuePair.Value.ToString())
                    return keyValuePair.Key;
            }
            return default;
        }

        public static V GetValue<K, V>(this Dictionary<K, V> keyValuePairs, K key)
        {
            foreach (KeyValuePair<K, V> keyValuePair in keyValuePairs)
            {
                if (key.ToString() == keyValuePair.Key.ToString())
                    return keyValuePair.Value;
            }
            return default;
        }
    }

    public static class Boolean
    {
        public static int AsNumericValue(this bool boolean) => boolean ? 1 : 0;
        public static void Set(this bool _, bool value) { _ = value; }
        public static bool Not(this bool _) => !_;
    }

    public static class Array
    {
        public static string[] ToStringArray(this int[] _)
        {
            return ToStringArray(_);
        }

        public static string[] ToStringArray(this float[] _)
        {
            return ToStringArray(_);
        }

        public static string[] ToStringArray(this double[] _)
        {
            return ToStringArray(_);
        }

        public static string[] ToStringArray(this object[] _)
        {
            string[] stringArray = new string[_.Length];
            for (int i = 0; i < _.Length; i++)
            {
                stringArray[i] = _[i].ToString();
            }
            return stringArray;
        }

        public static string[] ToStringArray(this object[] _, char delimiter, int index)
        {
            string[] stringArray = new string[_.Length];
            for (int i = 0; i < _.Length; i++)
            {
                stringArray[i] = _[i].ToString().Split(delimiter)[index];
            }
            return stringArray;
        }

        public static int Sum(this int[] _)
        {
            int value = 0;
            for(int i = 0; i < _.Length; i++)
            {
                value += _[i];
            }

            return value;
        }

        #region Unity Specific
        public static string[] ToStringArray(this UnityEngine.Object[] _)
        {
            string[] stringArray = new string[_.Length];
            for (int i = 0; i < _.Length; i++)
            {
                stringArray[i] = _[i].ToString();
            }
            return stringArray;
        }

        public static string[] ToStringArray(this Resolution[] _, string removeString)
        {
            string[] stringArray = new string[_.Length];
            for (int i = 0; i < _.Length; i++)
            {
                stringArray[i] = _[i].ToString().Replace(removeString, Constants.EMPTY);
            }
            return stringArray;

        }
        #endregion
    }

    public static class String
    {

        public static string TryConcat(this string _, params string[] strings)
        {
            StringBuilder stringBuilder = new StringBuilder();
            try
            {
                foreach (string _string in strings)
                {
                    stringBuilder = stringBuilder.AppendLine(_string);
                }
                return stringBuilder.ToString();
            }
            catch (IOException e)
            {
                Debug.Log(string.Format("Failed to concatenate: {0}", e.Message));
                return string.Empty;
            }
        }

        public static int AsNumericalValue(this string _)
        {
            return Convert.ToInt32(_);
        }

        public static string QuestionMark(this string _)
        {
            _ = "???";
            return _;
        }

        /// <summary>
        /// Generate a number based on a string.
        /// Also known as the String Numeric Value (SNV)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GenerateSNV(this string str)
        {
            //Get length of string
            int length = str.Length;

            //Collect ASCII code from string
            int[] asciiList = new int[length];


            //Iterate trhough string, and assign acsii values
            //Start with the length, and increment each iteration
            for (int limit = 0; limit < length; limit++)
            {
                for (int i = 0; i < length - limit; i++)
                {
                    var increment = (asciiList[i] + (i + 1)) + (str[0] + str[length - 1]);
                    asciiList[i] = (increment + (str[i] + ((length - limit) + 1)));
                }
            }

            //Add all asciiList values
            return asciiList.Sum();
        }
    }

    public static class File
    {
        public static void TryCopy(string sourceName, string destination)
        {
            try
            {
                System.IO.File.Copy(sourceName, destination);
            }
            catch (IOException e)
            {
                Debug.Log(string.Format("Failed to Copy File Info: {0}", e.Message));
                return;
            }
        }

    }

}
#endregion
