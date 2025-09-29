using CLIInterface.Math3D;
using System;

namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // Easing
    //
    //  https://easings.net/ko#
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public partial struct Easingf
    {
        private static float c1 = 1.70158f;
        private static float c2 = c1 * 1.525f;
        private static float c3 = c1 + 1f;
        private static float c4 = (2f * (float)Math.PI) / 3f;
        private static float c5 = (2f * (float)Math.PI) / 4.5f;

        public static float InSine(float a, float b, float t) => Mathf.Lerp(a, b, 1 - Mathf.Cos((t * Mathf.PI) / 2));
        public static float OutSine(float a, float b, float t) => Mathf.Lerp(a, b, Mathf.Sin((t * Mathf.PI) / 2));
        public static float InOutSine(float a, float b, float t) => Mathf.Lerp(a, b, -(Mathf.Cos(Mathf.PI * t) - 1) / 2);

        public static float InExpo(float a, float b, float t) => Mathf.Lerp(a, b, t == 0f ? 0f : Mathf.Pow(2f, 10f * t - 10f));
        public static float OutExpo(float a, float b, float t) => Mathf.Lerp(a, b, t == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * t));
        public static float InOutExpo(float a, float b, float t) => Mathf.Lerp(a, b, (t == 0f ? 0f : t == 1f ? 1f : t < 0.5 ? Mathf.Pow(2, 20 * t - 10) / 2 : (2 - Mathf.Pow(2, -20 * t + 10)) / 2));

        public static float InBack(float a, float b, float t) => Mathf.Lerp(a, b, c3 * t * t * t - c1 * t * t);
        public static float OutBack(float a, float b, float t) => Mathf.Lerp(a, b, 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f));
        public static float InOutBack(float a, float b, float t) => Mathf.Lerp(a, b, t < 0.5f ? (Mathf.Pow(2f * t, 2f) * ((c2 + 1f) * 2f * t - c2)) / 2f : (Mathf.Pow(2f * t - 2f, 2f) * ((c2 + 1f) * (t * 2f - 2f) + c2) + 2f) / 2f);

        public static float InElastic(float a, float b, float t) => Mathf.Lerp(a, b, t == 0f ? 0 : (t == 1 ? 1 : -Mathf.Pow(2f, 10f * t - 10f) * Mathf.Sign((t * 10f - 10.75f) * c4)));
        public static float OutElastic(float a, float b, float t) => Mathf.Lerp(a, b, t == 0f ? 0 : (t == 1 ? 1 : Mathf.Pow(2f, -10f * t) * Mathf.Sign((t * 10f - 0.75f) * c4) + 1f));
        public static float InOutElastic(float a, float b, float t)
        {
            return Mathf.Lerp(
                a, b,
                t == 0f
                ? 0
                : (t == 1
                ? 1
                : (t < 0.5f
                ? -(Mathf.Pow(2f, 20f * t - 10f) * Mathf.Sin(20f * t - 11.125f) * c5) / 2f
                : (Mathf.Pow(2f, -20f * t + 10f) * Mathf.Sin(20f * t - 11.125f) * c5) / 2f + 1f)));
        }
    }
}
