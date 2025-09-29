using J2y.JMath;
using System;
using System.Collections.Generic;

namespace J2y
{

    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JUtil
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public static partial class JUtil
    {

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // mathematics
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Math] Lerp
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        // [0, 1] -> [0, 1](Smooth)
        public static float SmoothLerp2(float t)
        {
            float percentage = 0.5f + Mathf.Sin(t * Mathf.PI - (Mathf.PI * 0.5f)) * 0.5f;
            //percentage = Mathf.Clamp01(percentage);
            return percentage;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static float SmoothLerp(float t)
        {
            return Mathf.Clamp01(Mathf.Sin(t * 90f));
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        // Mathf.LerpAngle 확장 [target_angle 에 이론상 영원히 도달하지 못하는 알고리즘 개선]
        public static float LerpAngle(float current_angle, float target_angle, float rotation_speed, float delta_time)
        {
            var result = current_angle;

            var delta_angle = Mathf.DeltaAngle(current_angle, target_angle);
            if (delta_angle == 0f)
                return result;

            var large_angle = delta_angle;
            if (delta_angle > 0f)
            {
                large_angle += 45f;
                if (large_angle > 180f)
                    large_angle = 360f - large_angle;
            }
            else
            {
                large_angle -= 45f;
                if (large_angle < -180f)
                    large_angle = -360f - large_angle;
            }

            result = Mathf.LerpAngle(current_angle, current_angle + large_angle, rotation_speed * delta_time);
            var current_delta_angle = Mathf.DeltaAngle(result, target_angle);

            if ((delta_angle > 0f && current_delta_angle < 0f) || (delta_angle < 0f && current_delta_angle > 0f))
                result = current_angle + delta_angle;


            return result;
        }

        public static float InverseLerp(float a, float b, float value)
        {
            if (a != b)
                return ((value - a) / (b - a));
            else
                return 0.0f;
        }
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        // sin[-1, 1] -> sin[0, 1]
        public static float Sin01(float t)
        {
            return Mathf.Sin(t) * 0.5f + 0.5f;
        }
        #endregion

        #region [Math] SphereCollisionDetect
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        // sphere1과 sphere2의 충돌 검사를 한다.
        // 리턴값이 트루일 경우 t초에 충돌한다.
        public static bool SphereCollisionDetect(Vector3 v1, Vector3 p1, float r1,
            Vector3 v2, Vector3 p2, float r2, ref float t)
        {
            Vector3 s = p1 - p2;        // vector between the centers of each sphere
            Vector3 v = v1 - v2;        // relative velocity between spheres
            float r = r1 + r2;

            float c1 = Vector3.Dot(s, s) - r * r; // if negative, they overlap
            if (c1 < 0.0) // if true, they already overlap
            {
                // This is bad ... we need to correct this by moving them a tiny fraction from each other
                //a->pos +=
                t = 0.0f;
                return true;
            }

            float a1 = Vector3.Dot(v, v);
            if (a1 < 0.00001f)
                return false; // does not move towards each other

            float b1 = Vector3.Dot(v, s);
            if (b1 >= 0.0)
                return false; // does not move towards each other

            float d1 = b1 * b1 - a1 * c1;
            if (d1 < 0.0)
                return false; // no real roots ... no collision

            t = (-b1 - Mathf.Sqrt(d1)) / a1;

            return true;
        }
        #endregion

        #region [Math] Swap/Max/Distance
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        // 이런 함수 없나??	
        public static void Swap<T>(ref T x, ref T y)
        {
            T temp;
            temp = x; x = y; y = temp;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static float Distance2d(Vector3 a, Vector3 b)
        {
            return Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.z - b.z) * (a.z - b.z));
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static int GetMax(int[] array, ref int index, int cheak = 99)
        {
            int max = 0;
            var tmp = index;
            index = 0;

            for (int i = 0; i < array.Length; i++)
            {
                if (max < array[i] && i != cheak)
                {
                    max = array[i];
                    index = i;
                }
            }
            return max;
        }
        #endregion

        #region [Math] Vector3
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vector3 MakeDirection(Vector3 p1, Vector3 p2)
        {
            return Vector3.Normalize(p2 - p1);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vector3 MakeDirectionXZ(Vector3 p1, Vector3 p2)
        {
            return Vector3.Normalize(new Vector3(p2.x - p1.x, 0f, p2.z - p1.z));
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vector3 MakeDirectionXZ(Vector3 p, bool normalize = false)
        {
            p = new Vector3(p.x, 0f, p.z);
            if (normalize)
                p.Normalize();
            return p;
        }
        #endregion

        #region [Convert] EulerToVector / VectorToEuler
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        static public Vector3 EulerToVector(float yaw, float pitch)
        {
            return Quaternion.Euler(pitch - 90f, yaw, 0f) * Vector3.forward;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        static public Vector2 VectorToEuler(Vector3 position)
        {
            position.Normalize();

            float yaw = Vector2.Angle(Vector2.up, new Vector2(position.x, position.z).normalized);
            if (position.x < 0f)
                yaw = 360f - yaw;

            var pitch = Mathf.Acos(position.y) / Mathf.PI * 180f;

            return new Vector2(yaw, pitch);
        }
        #endregion

        #region [유틸] Frame <-> Time
        public static float FrameToTime(int frame)
        {
            return (float)frame / 30f;
        }
        public static int TimeToFrame(float time)
        {
            return (int)(time * 30f);
        }
        #endregion

        #region [유틸] Color <-> Int
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static Color IntToColor(int c)
        //{
        //    var r = (float)((c >> 16) & 0xff);
        //    var g = (float)((c >> 8) & 0xff);
        //    var b = (float)(c & 0xff);
        //    return new Color(r / 255f, g / 255f, b / 255f);
        //}
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static Color32 IntToColor32(int c)
        //{
        //    var r = (byte)((c >> 16) & 0xff);
        //    var g = (byte)((c >> 8) & 0xff);
        //    var b = (byte)(c & 0xff);
        //    return new Color32(r, g, b, 255);
        //}
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static int ColorToInt(Color32 c)
        //{
        //    var ci = (c.r >> 16) | (c.g >> 16) | (c.b);
        //    return ci;
        //}
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static int ColorToInt(Color c)
        //{
        //    return ColorToInt((Color32)c);
        //}
        #endregion

        #region [KeyValuePair] Make
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static KeyValuePair<K, U> MakePair<K, U>()
        {
            return new KeyValuePair<K, U>();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static KeyValuePair<K, U> MakePair<K, U>(K key, U value)
        {
            return new KeyValuePair<K, U>(key, value);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static KeyValuePair<string, U> MakePair<U>(string key) where U : new()
        {
            return new KeyValuePair<string, U>(key, new U());
        }
        #endregion


        #region [수학] (Line, Point) Distance
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static float DistanceToLine(Vector3 p1, Vector3 p2, Vector3 point)
        {
            var dist1 = Vector3.Dot((p2 - p1).normalized, point - p1);
            var dist2 = Vector3.Dot((p1 - p2).normalized, point - p2);
            return Mathf.Min(dist1, dist2);
        }
        #endregion


        #region [유틸] InFront
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static bool InFront(TransformGroup transform, Vector3 pos)
        //{
        //          var heading = pos - view.position;
        //	var dot = Vector3.Dot(heading, view.forward);
        //	return dot > 0f;
        //}
        #endregion

        #region [수학] Angle360
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static float Angle360(Vector3 from, Vector3 to)
        {
            return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
        }
        #endregion


        #region [Math] RestrictAngle
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Restricts the angle between -360 and 360 degrees.
        /// </summary>
        /// <param name="angle">The angle to restrict.</param>
        /// <returns>An angle between -360 and 360 degrees.</returns>
        public static float RestrictAngle(float angle)
        {
            if (angle < -360)
            {
                angle += 360;
            }
            if (angle > 360)
            {
                angle -= 360;
            }
            return angle;
        }
        #endregion

        #region [Math] RestrictInnerAngle
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Restricts the angle between -180 and 180 degrees.
        /// </summary>
        /// <param name="angle">The angle to restrict.</param>
        /// <returns>An angle between -180 and 180 degrees.</returns>
        public static float RestrictInnerAngle(float angle)
        {
            if (angle < -180)
            {
                angle += 360;
            }
            if (angle > 180)
            {
                angle -= 360;
            }
            return angle;
        }
        #endregion

        #region [Math] RestrictAngleBetween
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Restricts the angle between the firstAmount and secondAmount.
        /// </summary>
        /// <param name="angle">The angle to restrict.</param>
        /// <param name="firstAmount">The first amount to restrict the angle by.</param>
        /// <param name="secondAmount">The second amount to restrict the angle by.</param>
        /// <returns></returns>
        public static float RestrictAngleBetween(float currentAngle, float angle, float firstAmount, float secondAmount)
        {
            var lowerAngle = RestrictInnerAngle(currentAngle + firstAmount);
            var upperAngle = RestrictInnerAngle(currentAngle + secondAmount);
            if (upperAngle < lowerAngle)
            {
                upperAngle += 360;
            }
            // Keep the angle in the same restricted angle to ease the smoothing.
            if (angle < upperAngle - 360)
            {
                angle += 360;
            }
            else if (angle > lowerAngle + 360)
            {
                angle -= 360;
            }
            return Mathf.Clamp(angle, lowerAngle, upperAngle);
        }
        #endregion

        #region [Math] ClampAngle
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Clamp the angle between the min and max angle values.
        /// </summary>
        /// <param name="angle">The angle to be clamped.</param>
        /// <param name="min">The minimum angle value.</param>
        /// <param name="max">The maximum angle value.</param>
        /// <returns></returns>
        public static float ClampAngle(float angle, float min, float max)
        {
            return Mathf.Clamp(RestrictAngle(angle), min, max);
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 확률
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        static System.Random random = new System.Random();

        #region [랜덤] Values (Unity3D)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void RandomValues(int[] recordTime, int min, int max)
        {
            for (var i = 0; i < recordTime.Length; ++i)
                recordTime[i] = RandomRange(min, max);
        }
        public static Vector3 RandomRange(Vector3 center, Vector3 size)
        {
            return center + RandomVector3(size);
        }
        public static Vector3 RandomRange(Vector3 center, float size_x, float size_z)
        {
            return center + RandomVector3(size_x, size_z);
        }
        public static Vector3 RandomVector3(Vector3 size)
        {
            return new Vector3(
               ((float)random.NextDouble() - 0.5f) * size.x,
               ((float)random.NextDouble() - 0.5f) * size.y,
               ((float)random.NextDouble() - 0.5f) * size.z
            );
        }
        public static Vector3 RandomVector3(float size)
        {
            return new Vector3(
               ((float)random.NextDouble() - 0.5f) * size,
               ((float)random.NextDouble() - 0.5f) * size,
               ((float)random.NextDouble() - 0.5f) * size
            );
        }
        public static Vector3 RandomVector3(float size_x, float size_z)
        {
            var random = new System.Random();
            var v3 = new Vector3(
               ((float)random.NextDouble() - 0.5f) * size_x,
               0f,
               ((float)random.NextDouble() - 0.5f) * size_z
            );
            return v3;
        }
        #endregion

        #region [랜덤] Values (System)
        static System.Random _random = new System.Random((int)DateTime.Now.Ticks);

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static float RandomRange(float min, float max)
        {
            if (min == max)
                return min;
            var r = min + (float)_random.NextDouble() * (max - min);
            return r;
        }
        public static int RandomRange(int min, int max)
        {
            if (min == max)
                return min;
            int r = min + _random.Next() % (max - min);
            return r;
        }


        #endregion

        #region [랜덤] Shuffle
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Shuffle<T>(T[] array)
        {
            Shuffle(array, array.Length * 2);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Shuffle<T>(T[] array, int count)
        {
            int arrayLen = array.Length;
            Shuffle(array, arrayLen, count);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Shuffle<T>(T[] array, int arrayLen, int count)
        {
            while (count > 1)
            {
                int k1 = RandomRange(0, arrayLen); // 0 <= k < n.
                int k2 = RandomRange(0, arrayLen); // 0 <= k < n.
                count--;
                T temp = array[k1];
                array[k1] = array[k2];
                array[k2] = temp;
            }
        }
        #endregion

    }

}
