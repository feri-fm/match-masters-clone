using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static class Extensions
{
    public static Vector3 NormalizeEulerAngles(this Vector3 eulerAngles)
    {
        return new Vector3(
            eulerAngles.x > 180 ? 360 - eulerAngles.x : eulerAngles.x,
            eulerAngles.y > 180 ? 180 - eulerAngles.y : eulerAngles.y,
            eulerAngles.z > 180 ? 360 - eulerAngles.z : eulerAngles.z);
    }

    public static string[] ToStrings(this IEnumerable enumerable)
    {
        List<string> re = new List<string>();
        foreach (var item in enumerable)
            re.Add(item.ToString());
        return re.ToArray();
    }

    public static Texture2D ToTexture2D(this RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGBA64, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }

    public static T DeepCopy<T>(this T other)
    {
        if (other == null) return default(T);
        using (MemoryStream ms = new MemoryStream())
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, other);
            ms.Position = 0;
            return (T)formatter.Deserialize(ms);
        }
    }

    public static void ShowExplorer(this string path)
    {
        Application.OpenURL("file://" + path);
    }
    public static bool Validate(this GameObject obj, Filter filter)
    {
        return Validate(obj, filter.layerMask, filter.tags);
    }
    public static bool Validate(this GameObject obj, int layer, string[] tags)
    {
        return layer == (layer | (1 << obj.layer)) || tags.Contains(obj.tag);
    }
    public static bool Validate(this GameObject obj, string[] tags)
    {
        return tags.Contains(obj.tag);
    }
    public static bool Validate(this GameObject obj, int layer)
    {
        return layer == (layer | (1 << obj.layer));
    }

    public static float Average(this float[] numbers)
    {
        float re = 0;
        foreach (var num in numbers)
            re += num;
        return re / numbers.Length;
    }

    public static float Map(this float value, float min, float max, float newMin = 0, float newMax = 1)
    {
        return (value - min) / (max - min) * (newMax - newMin) + newMin;
    }

    public static float MoveValue(this float value, float target, float speed, float smooth)
    {
        return Mathf.MoveTowards(value, Mathf.Lerp(value, target, smooth * Time.timeScale), speed * Time.timeScale);
    }
    public static Vector3 MoveValue(this Vector3 value, Vector3 target, float speed, float smooth)
    {
        return Vector3.MoveTowards(value, Vector3.Lerp(value, target, smooth * Time.timeScale), speed * Time.timeScale);
    }
    public static Vector2 MoveValue(this Vector2 value, Vector2 target, float speed, float smooth)
    {
        return Vector2.MoveTowards(value, Vector2.Lerp(value, target, smooth * Time.timeScale), speed * Time.timeScale);
    }
    public static Quaternion MoveValue(this Quaternion value, Quaternion target, float speed, float smooth)
    {
        return Quaternion.RotateTowards(value, Quaternion.Lerp(value, target, smooth * Time.timeScale), speed * Time.timeScale);
    }

    public static T Random<T>(this T[] array)
    {
        return array[UnityEngine.Random.Range(0, array.Length)];
    }
    public static T Random<T>(this List<T> array)
    {
        return array[UnityEngine.Random.Range(0, array.Count)];
    }

    public static void ApplyDirectionDrag(this Rigidbody2D rigidbody, Vector2 direction, float velocityMultiplier)
    {
        var delta = rigidbody.velocity * (1 - velocityMultiplier);
        delta.x *= Mathf.Abs(direction.x);
        delta.y *= Mathf.Abs(direction.y);
        rigidbody.velocity -= delta;
    }

    public static Vector2 Rotate(this Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

    public static float Round(this float number, float round = 10)
    {
        return Mathf.Round(number * round) / round;
    }

    public static string f(this string str, params object[] args)
    {
        return string.Format(str, args);
    }
    private static char[] english = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
    private static char[] farsi = new char[] { '۰', '۱', '۲', '۳', '۴', '۵', '۶', '۷', '۸', '۹' };
    public static string FaNum(this string str)
    {
        var result = str;
        for (int i = 0; i < english.Length; i++)
        {
            result = result.Replace(english[i], farsi[i]);
        }
        return result;
    }
    public static string EnNum(this string str)
    {
        var result = str;
        for (int i = 0; i < farsi.Length; i++)
        {
            result = result.Replace(farsi[i], english[i]);
        }
        return result;
    }

    public static Vector3 RandomVector(this float value)
    {
        return new Vector3(UnityEngine.Random.Range(-value, value), UnityEngine.Random.Range(-value, value), UnityEngine.Random.Range(-value, value));
    }

    public static T FromJson<T>(this string json, T fallback = default(T))
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Error = (se, ev) => { ev.ErrorContext.Handled = true; }
            });
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            return fallback;
        }
    }
    public static string ToJson(this object obj, Formatting formatting = Formatting.None)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = formatting
        };
        return JsonConvert.SerializeObject(obj, settings);
    }

    public static T JsonCopy<T>(this T obj)
    {
        return obj.ToJson().FromJson<T>();
    }

    public static string Beautify(this string json)
    {
        JToken parsedJson = JToken.Parse(json);
        return parsedJson.ToString(Formatting.Indented);
    }

    public static float Random(this Vector2 vector)
    {
        return UnityEngine.Random.Range(vector.x, vector.y);
    }
    public static int Random(this Vector2Int vector)
    {
        return UnityEngine.Random.Range(vector.x, vector.y);
    }

    public static float Lerp(this Vector2 vector, float t)
    {
        return t.Map(0, 1, vector.x, vector.y);
    }

    public static T[] Shuffle<T>(this T[] arr)
    {
        var shArr = new T[arr.Length];
        arr.CopyTo(shArr, 0);
        for (int i = 0; i < shArr.Length; i++)
        {
            var r = UnityEngine.Random.Range(i, shArr.Length);
            var temp = shArr[i];
            shArr[i] = shArr[r];
            shArr[r] = temp;
        }
        return shArr;
    }
    public static List<T> Shuffle<T>(this List<T> arr)
    {
        return arr.ToArray().Shuffle().ToList();
    }

    public static T Clamp<T>(this T[] arr, int index) => arr[Mathf.Clamp(index, 0, arr.Length - 1)];

    public static DateTime UTCToDateTime(this string text)
    {
        try
        {
            // text = "2022-08-24T09:05:01.742Z";
            var year = int.Parse(text.Substring(0, 4));
            var month = int.Parse(text.Substring(5, 2));
            var day = int.Parse(text.Substring(8, 2));
            var hour = int.Parse(text.Substring(11, 2));
            var minute = int.Parse(text.Substring(14, 2));
            var second = int.Parse(text.Substring(17, 2));
            return new DateTime(year, month, day, hour, minute, second);
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            return new DateTime();
        }
    }

    public static TimeSpan FromMillisecondsToTimeSpan(this double milliseconds)
    {
        return System.TimeSpan.FromMilliseconds(milliseconds);
    }

    public static void AddContains<T>(this List<T> list, T item)
    {
        if (!list.Contains(item))
            list.Add(item);
    }
}

[System.Serializable]
public class Filter
{
    public LayerMask layerMask;
    public string[] tags = new string[] { "Player" };
}