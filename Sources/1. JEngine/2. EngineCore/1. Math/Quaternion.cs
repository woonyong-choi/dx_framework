using System;


namespace J2y
{
    namespace JMath
    {

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //
        // Quaternion
        //
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        internal partial struct Quaternion : IEquatable<Quaternion>
        {
            const float radToDeg = (float)(180.0 / Math.PI);
            const float degToRad = (float)(Math.PI / 180.0);

            public const float kEpsilon = 0.000001F;


            // X component of the Quaternion. Don't modify this directly unless you know quaternions inside out.
            public float x;
            // Y component of the Quaternion. Don't modify this directly unless you know quaternions inside out.
            public float y;
            // Z component of the Quaternion. Don't modify this directly unless you know quaternions inside out.
            public float z;
            // W component of the Quaternion. Don't modify this directly unless you know quaternions inside out.
            public float w;

            // Access the x, y, z, w components using [0], [1], [2], [3] respectively.
            public float this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0: return x;
                        case 1: return y;
                        case 2: return z;
                        case 3: return w;
                        default:
                            throw new IndexOutOfRangeException("Invalid Quaternion index!");
                    }
                }

                set
                {
                    switch (index)
                    {
                        case 0: x = value; break;
                        case 1: y = value; break;
                        case 2: z = value; break;
                        case 3: w = value; break;
                        default:
                            throw new IndexOutOfRangeException("Invalid Quaternion index!");
                    }
                }
            }

            // Constructs new Quaternion with given x,y,z,w components.
            public Quaternion(float x, float y, float z, float w) { this.x = x; this.y = y; this.z = z; this.w = w; }
            public Quaternion(Vector3 v, float w) { this.x = v.x; this.y = v.y; this.z = v.z; this.w = w; }

            // Set x, y, z and w components of an existing Quaternion.
            public void Set(float newX, float newY, float newZ, float newW)
            {
                x = newX;
                y = newY;
                z = newZ;
                w = newW;
            }

            static readonly Quaternion identityQuaternion = new Quaternion(0F, 0F, 0F, 1F);

            // The identity rotation (RO). This quaternion corresponds to "no rotation": the object
            public static Quaternion identity
            {
                get
                {
                    return identityQuaternion;
                }
            }

            // Combines rotations /lhs/ and /rhs/.
            public static Quaternion operator *(Quaternion lhs, Quaternion rhs)
            {
                return new Quaternion(
                    lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y,
                    lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z,
                    lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x,
                    lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z);
            }

            // Rotates the point /point/ with /rotation/.
            public static Vector3 operator *(Quaternion rotation, Vector3 point)
            {
                float x = rotation.x * 2F;
                float y = rotation.y * 2F;
                float z = rotation.z * 2F;
                float xx = rotation.x * x;
                float yy = rotation.y * y;
                float zz = rotation.z * z;
                float xy = rotation.x * y;
                float xz = rotation.x * z;
                float yz = rotation.y * z;
                float wx = rotation.w * x;
                float wy = rotation.w * y;
                float wz = rotation.w * z;

                Vector3 res;
                res.x = (1F - (yy + zz)) * point.x + (xy - wz) * point.y + (xz + wy) * point.z;
                res.y = (xy + wz) * point.x + (1F - (xx + zz)) * point.y + (yz - wx) * point.z;
                res.z = (xz - wy) * point.x + (yz + wx) * point.y + (1F - (xx + yy)) * point.z;
                return res;
            }

            // *undocumented*

            // Is the dot product of two quaternions within tolerance for them to be considered equal?
            private static bool IsEqualUsingDot(float dot)
            {
                // Returns false in the presence of NaN values.
                return dot > 1.0f - kEpsilon;
            }

            // Are two quaternions equal to each other?
            public static bool operator ==(Quaternion lhs, Quaternion rhs)
            {
                return IsEqualUsingDot(Dot(lhs, rhs));
            }

            // Are two quaternions different from each other?
            public static bool operator !=(Quaternion lhs, Quaternion rhs)
            {
                // Returns true in the presence of NaN values.
                return !(lhs == rhs);
            }

            // The dot product between two rotations.
            public static float Dot(Quaternion a, Quaternion b)
            {
                return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
            }

            public void SetLookRotation(Vector3 view)
            {
                Vector3 up = Vector3.up;
                SetLookRotation(view, up);
            }

            // Creates a rotation with the specified /forward/ and /upwards/ directions.
            public void SetLookRotation(Vector3 view, Vector3 up)
            {
                this = LookRotation(view, up);
            }

            // Returns the angle in degrees between two rotations /a/ and /b/.
            public static float Angle(Quaternion a, Quaternion b)
            {
                float dot = Dot(a, b);
                return IsEqualUsingDot(dot) ? 0.0f : Mathf.Acos(Mathf.Min(Mathf.Abs(dot), 1.0F)) * 2.0F * Mathf.Rad2Deg;
            }

            // Makes euler angles positive 0/360 with 0.0001 hacked to support old behaviour of QuaternionToEuler
            private static Vector3 Internal_MakePositive(Vector3 euler)
            {
                float negativeFlip = -0.0001f * Mathf.Rad2Deg;
                float positiveFlip = 360.0f + negativeFlip;

                if (euler.x < negativeFlip)
                    euler.x += 360.0f;
                else if (euler.x > positiveFlip)
                    euler.x -= 360.0f;

                if (euler.y < negativeFlip)
                    euler.y += 360.0f;
                else if (euler.y > positiveFlip)
                    euler.y -= 360.0f;

                if (euler.z < negativeFlip)
                    euler.z += 360.0f;
                else if (euler.z > positiveFlip)
                    euler.z -= 360.0f;

                return euler;
            }

            public Vector3 eulerAngles
            {
                get
                {
                    return Quaternion.ToEulerRad(this) * radToDeg;
                }
                set
                {
                    this = Quaternion.FromEulerRad(value * degToRad);
                }
            }
            public float LengthSquared
            {
                get
                {
                    return x * x + y * y + z * z + w * w;
                }
            }
            public Vector3 xyz
            {
                set
                {
                    x = value.x;
                    y = value.y;
                    z = value.z;
                }
                get
                {
                    return new Vector3(x, y, z);
                }
            }

            public static Quaternion AngleAxis(float angle, Vector3 axis)
            {
                return Quaternion.AngleAxis(angle, ref axis);
            }
            private static Quaternion AngleAxis(float degress, ref Vector3 axis)
            {
                if (axis.sqrMagnitude == 0.0f)
                    return identity;

                Quaternion result = identity;
                var radians = degress * degToRad;
                radians *= 0.5f;
                axis.Normalize();
                axis = axis * (float)System.Math.Sin(radians);
                result.x = axis.x;
                result.y = axis.y;
                result.z = axis.z;
                result.w = (float)System.Math.Cos(radians);

                return Normalize(result);
            }
            public static Quaternion Euler(float x, float y, float z) { return FromEulerRad(new Vector3(x, y, z) * Mathf.Deg2Rad); }
            public static Quaternion Euler(Vector3 euler) { return FromEulerRad(euler * Mathf.Deg2Rad); }
            public void ToAngleAxis(out float angle, out Vector3 axis) { ToAxisAngleRad(this, out axis, out angle); angle *= Mathf.Rad2Deg; }
            public void SetFromToRotation(Vector3 fromDirection, Vector3 toDirection) { this = FromToRotation(fromDirection, toDirection); }
            public static Quaternion FromToRotation(Vector3 fromDirection, Vector3 toDirection)
            {
                return RotateTowards(LookRotation(fromDirection), LookRotation(toDirection), float.MaxValue);
            }

            private static Vector3 ToEulerRad(Quaternion rotation)
            {
                float sqw = rotation.w * rotation.w;
                float sqx = rotation.x * rotation.x;
                float sqy = rotation.y * rotation.y;
                float sqz = rotation.z * rotation.z;
                float unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
                float test = rotation.x * rotation.w - rotation.y * rotation.z;
                Vector3 v;

                if (test > 0.4995f * unit)
                { // singularity at north pole
                    v.y = 2f * Mathf.Atan2(rotation.y, rotation.x);
                    v.x = Mathf.PI / 2;
                    v.z = 0;
                    return NormalizeAngles(v * Mathf.Rad2Deg);
                }
                if (test < -0.4995f * unit)
                { // singularity at south pole
                    v.y = -2f * Mathf.Atan2(rotation.y, rotation.x);
                    v.x = -Mathf.PI / 2;
                    v.z = 0;
                    return NormalizeAngles(v * Mathf.Rad2Deg);
                }
                Quaternion q = new Quaternion(rotation.w, rotation.z, rotation.x, rotation.y);
                v.y = (float)System.Math.Atan2(2f * q.x * q.w + 2f * q.y * q.z, 1 - 2f * (q.z * q.z + q.w * q.w));     // Yaw
                v.x = (float)System.Math.Asin(2f * (q.x * q.z - q.w * q.y));                             // Pitch
                v.z = (float)System.Math.Atan2(2f * q.x * q.y + 2f * q.z * q.w, 1 - 2f * (q.y * q.y + q.z * q.z));      // Roll
                return NormalizeAngles(v * Mathf.Rad2Deg);
            }
            private static Vector3 NormalizeAngles(Vector3 angles)
            {
                angles.x = NormalizeAngle(angles.x);
                angles.y = NormalizeAngle(angles.y);
                angles.z = NormalizeAngle(angles.z);
                return angles;
            }
            private static float NormalizeAngle(float angle)
            {
                while (angle > 360)
                    angle -= 360;
                while (angle < 0)
                    angle += 360;
                return angle;
            }
            private static Quaternion FromEulerRad(Vector3 euler)
            {
                var yaw = euler.x;
                var pitch = euler.y;
                var roll = euler.z;
                float rollOver2 = roll * 0.5f;
                float sinRollOver2 = (float)System.Math.Sin((float)rollOver2);
                float cosRollOver2 = (float)System.Math.Cos((float)rollOver2);
                float pitcPointer2 = pitch * 0.5f;
                float sinPitcPointer2 = (float)System.Math.Sin((float)pitcPointer2);
                float cosPitcPointer2 = (float)System.Math.Cos((float)pitcPointer2);
                float yawOver2 = yaw * 0.5f;
                float sinYawOver2 = (float)System.Math.Sin((float)yawOver2);
                float cosYawOver2 = (float)System.Math.Cos((float)yawOver2);
                Quaternion result;
                result.x = cosYawOver2 * cosPitcPointer2 * cosRollOver2 + sinYawOver2 * sinPitcPointer2 * sinRollOver2;
                result.y = cosYawOver2 * cosPitcPointer2 * sinRollOver2 - sinYawOver2 * sinPitcPointer2 * cosRollOver2;
                result.z = cosYawOver2 * sinPitcPointer2 * cosRollOver2 + sinYawOver2 * cosPitcPointer2 * sinRollOver2;
                result.w = sinYawOver2 * cosPitcPointer2 * cosRollOver2 - cosYawOver2 * sinPitcPointer2 * sinRollOver2;
                return result;

            }
            private static void ToAxisAngleRad(Quaternion q, out Vector3 axis, out float angle)
            {
                if (System.Math.Abs(q.w) > 1.0f)
                    q.Normalize();
                angle = 2.0f * (float)System.Math.Acos(q.w); // angle
                float den = (float)System.Math.Sqrt(1.0 - q.w * q.w);
                if (den > 0.0001f)
                {
                    axis = q.xyz / den;
                }
                else
                {
                    // This occurs when the angle is zero. 
                    // Not a problem: just set an arbitrary normalized axis.
                    axis = new Vector3(1, 0, 0);
                }
            }

            public static Quaternion Slerp(Quaternion a, Quaternion b, float t)
            {
                return Quaternion.Slerp(ref a, ref b, t);
            }
            private static Quaternion Slerp(ref Quaternion a, ref Quaternion b, float t)
            {
                if (t > 1) t = 1;
                if (t < 0) t = 0;
                return SlerpUnclamped(ref a, ref b, t);
            }
            public static Quaternion RotateTowards(Quaternion from, Quaternion to, float maxDegreesDelta)
            {
                float num = Quaternion.Angle(from, to);
                if (num == 0f)
                {
                    return to;
                }
                float t = Math.Min(1f, maxDegreesDelta / num);
                return Quaternion.SlerpUnclamped(from, to, t);
            }
            public static Quaternion SlerpUnclamped(Quaternion a, Quaternion b, float t)
            {
                return Quaternion.SlerpUnclamped(ref a, ref b, t);
            }
            private static Quaternion SlerpUnclamped(ref Quaternion a, ref Quaternion b, float t)
            {
                // if either input is zero, return the other.
                if (a.LengthSquared == 0.0f)
                {
                    if (b.LengthSquared == 0.0f)
                    {
                        return identity;
                    }
                    return b;
                }
                else if (b.LengthSquared == 0.0f)
                {
                    return a;
                }


                float cosHalfAngle = a.w * b.w + Vector3.Dot(a.xyz, b.xyz);

                if (cosHalfAngle >= 1.0f || cosHalfAngle <= -1.0f)
                {
                    // angle = 0.0f, so just return one input.
                    return a;
                }
                else if (cosHalfAngle < 0.0f)
                {
                    b.xyz = -b.xyz;
                    b.w = -b.w;
                    cosHalfAngle = -cosHalfAngle;
                }

                float blendA;
                float blendB;
                if (cosHalfAngle < 0.99f)
                {
                    // do proper slerp for big angles
                    float halfAngle = (float)System.Math.Acos(cosHalfAngle);
                    float sinHalfAngle = (float)System.Math.Sin(halfAngle);
                    float oneOverSinHalfAngle = 1.0f / sinHalfAngle;
                    blendA = (float)System.Math.Sin(halfAngle * (1.0f - t)) * oneOverSinHalfAngle;
                    blendB = (float)System.Math.Sin(halfAngle * t) * oneOverSinHalfAngle;
                }
                else
                {
                    // do lerp if angle is really small.
                    blendA = 1.0f - t;
                    blendB = t;
                }

                Quaternion result = new Quaternion(blendA * a.xyz + blendB * b.xyz, blendA * a.w + blendB * b.w);
                if (result.LengthSquared > 0.0f)
                    return Normalize(result);
                else
                    return identity;
            }

            public static Quaternion LookRotation(Vector3 forward, Vector3 upwards)
            {
                return Quaternion.LookRotation(ref forward, ref upwards);
            }
            public static Quaternion LookRotation(Vector3 forward)
            {
                Vector3 up = Vector3.up;
                return Quaternion.LookRotation(ref forward, ref up);
            }
            private static Quaternion LookRotation(ref Vector3 forward, ref Vector3 up)
            {

                forward = Vector3.Normalize(forward);
                Vector3 right = Vector3.Normalize(Vector3.Cross(up, forward));
                up = Vector3.Cross(forward, right);
                var m00 = right.x;
                var m01 = right.y;
                var m02 = right.z;
                var m10 = up.x;
                var m11 = up.y;
                var m12 = up.z;
                var m20 = forward.x;
                var m21 = forward.y;
                var m22 = forward.z;


                float num8 = (m00 + m11) + m22;
                var quaternion = new Quaternion();
                if (num8 > 0f)
                {
                    var num = (float)System.Math.Sqrt(num8 + 1f);
                    quaternion.w = num * 0.5f;
                    num = 0.5f / num;
                    quaternion.x = (m12 - m21) * num;
                    quaternion.y = (m20 - m02) * num;
                    quaternion.z = (m01 - m10) * num;
                    return quaternion;
                }
                if ((m00 >= m11) && (m00 >= m22))
                {
                    var num7 = (float)System.Math.Sqrt(((1f + m00) - m11) - m22);
                    var num4 = 0.5f / num7;
                    quaternion.x = 0.5f * num7;
                    quaternion.y = (m01 + m10) * num4;
                    quaternion.z = (m02 + m20) * num4;
                    quaternion.w = (m12 - m21) * num4;
                    return quaternion;
                }
                if (m11 > m22)
                {
                    var num6 = (float)System.Math.Sqrt(((1f + m11) - m00) - m22);
                    var num3 = 0.5f / num6;
                    quaternion.x = (m10 + m01) * num3;
                    quaternion.y = 0.5f * num6;
                    quaternion.z = (m21 + m12) * num3;
                    quaternion.w = (m20 - m02) * num3;
                    return quaternion;
                }
                var num5 = (float)System.Math.Sqrt(((1f + m22) - m00) - m11);
                var num2 = 0.5f / num5;
                quaternion.x = (m20 + m02) * num2;
                quaternion.y = (m21 + m12) * num2;
                quaternion.z = 0.5f * num5;
                quaternion.w = (m01 - m10) * num2;
                return quaternion;
            }
            public static Quaternion Inverse(Quaternion rotation)
            {
                float lengthSq = rotation.LengthSquared;
                if (lengthSq != 0.0)
                {
                    float i = 1.0f / lengthSq;
                    return new Quaternion(rotation.xyz * -i, rotation.w * i);
                }
                return rotation;
            }

            public static Quaternion Normalize(Quaternion q)
            {
                float mag = Mathf.Sqrt(Dot(q, q));

                if (mag < Mathf.Epsilon)
                    return Quaternion.identity;

                return new Quaternion(q.x / mag, q.y / mag, q.z / mag, q.w / mag);
            }

            public void Normalize()
            {
                this = Normalize(this);
            }

            public Quaternion normalized
            {
                get { return Normalize(this); }
            }

            // used to allow Quaternions to be used as keys in hash tables
            public override int GetHashCode()
            {
                return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2) ^ (w.GetHashCode() >> 1);
            }

            // also required for being able to use Quaternions as keys in hash tables
            public override bool Equals(object other)
            {
                if (!(other is Quaternion)) return false;

                return Equals((Quaternion)other);
            }

            public bool Equals(Quaternion other)
            {
                return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && w.Equals(other.w);
            }
        }
    }
}

