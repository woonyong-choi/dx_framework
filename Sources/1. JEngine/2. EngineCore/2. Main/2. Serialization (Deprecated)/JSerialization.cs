using J2y.JMath;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JSerialization
    //      새로운 Serialization을 만드는 이유는 다음과 같다.
    //          1. 범용 Serialization은 타입과 값을 모두 저장하기 때문에 용량이 크고 속도가 느리다.
    //          2. 클라이언트와 서버의 객체들은 비슷한 클래스를 사용하지만 정확히 같은 타입은 아니며(ex-LsfUser, LcUser) 어셈블리가 다르기 때문에 
    //             Serialization이 불가능하다.
    //          -> 1번의 경우는 많은 오픈소스가 존재하지만 2번의 이유로 직접 제작하도록 한다.
    //
    //
    // @JSerialization
    //      가장 기본적인 데이터 직렬화 방식(BinaryFormatter 기반)
    //
    // @FstSerialization
    //      Reflection을 이용하여 기본 데이터만을 Serialize & Deserialize하는 방식으로, null이나 추상타입에 대한 지원을 하지 않고, 최소한의 데이터만을 변환하는 방식
    //
    // @JNewSerialization : TypeHashing
    //      Fast와 유사한 알고리즘으로 동작하나 데이터의 형식(Type FullName)과 함께 데이터를 변환하여 null이나 추상 타입에 대한 제약이 없는 방식.단, 데이터의 형식이 차지하는 데이터 크기를 최소화하기 위한 Hashing과정이 추가됨
    //      (※ Hashing 결과물을 해석하기 위한 데이터는 별도의 파일로 저장 되며, 이에 대한 경로 지정 필요)
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++



    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JSerialization
    //
    //      @SerializationSurrogate
    //          Serialization을 위해서는 class에 [Serializable] Attribute 추가가 필요하다.
    //          그러나 이미 개발된 유니티의 Vector3, Quaternion를 Serialization하기 위해서는 SerializationSurrogate이용한 타입 변환 대리자 필요하다.
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region [SerializationSurrogate] Vector3
    //------------------------------------------------------------------------------------------------------------------------------------------------------
    sealed class Vector3SerializationSurrogate : ISerializationSurrogate
    {
        #region [Serialize] GetObjectData
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
        {
            var v3 = (Vector3)obj;
            info.AddValue("x", v3.x);
            info.AddValue("y", v3.y);
            info.AddValue("z", v3.z);
            //Debug.Log(v3);
        }
        #endregion

        #region [Deserialize] SetObjectData
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            var v3 = (Vector3)obj;
            v3.x = (float)info.GetValue("x", typeof(float));
            v3.y = (float)info.GetValue("y", typeof(float));
            v3.z = (float)info.GetValue("z", typeof(float));
            obj = v3;
            return obj;   // Formatters ignore this return value //Seems to have been fixed!
        }
        #endregion

    }
    #endregion

    #region [SerializationSurrogate] Quaternion
    //------------------------------------------------------------------------------------------------------------------------------------------------------
    sealed class QuaternionSerializationSurrogate : ISerializationSurrogate
    {
        #region [Serialize] GetObjectData
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
        {
            var q = (Quaternion)obj;
            info.AddValue("x", q.x);
            info.AddValue("y", q.y);
            info.AddValue("z", q.z);
            info.AddValue("w", q.w);
            //Debug.Log(v3);
        }
        #endregion

        #region [Deserialize] SetObjectData
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            var q = (Quaternion)obj;
            q.x = (float)info.GetValue("x", typeof(float));
            q.y = (float)info.GetValue("y", typeof(float));
            q.z = (float)info.GetValue("z", typeof(float));
            q.w = (float)info.GetValue("w", typeof(float));
            obj = q;
            return obj;   // Formatters ignore this return value //Seems to have been fixed!
        }
        #endregion

    }
    #endregion

    public static class JSerialization
    {

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //  [BinaryFormatter]
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private static BinaryFormatter s_binary_formatter;

        #region [BinaryFormatter] 생성 (+ SerializationSurrogate)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static BinaryFormatter MakeBinaryFormatter()
        {
            var bf = new BinaryFormatter();

            var ss = new SurrogateSelector();
            ss.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3SerializationSurrogate());
            ss.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), new QuaternionSerializationSurrogate());


            bf.SurrogateSelector = ss;
            return bf;
        }
        #endregion

        #region [BinaryFormatter] GET
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static BinaryFormatter GetBinaryFormatter()
        {
            if (null == s_binary_formatter)
                s_binary_formatter = MakeBinaryFormatter();

            return s_binary_formatter;
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //  [Serialization]
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Serialize] Stream/BinaryWriter
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Serialize(Stream serializationStream, object graph)
        {
            GetBinaryFormatter().Serialize(serializationStream, graph);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Serialize(BinaryWriter writer, object graph)
        {
            GetBinaryFormatter().Serialize(writer.BaseStream, graph);
        }
        #endregion

        #region [Deserialize] Stream/BinaryReader
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static object Deserialize(Stream serializationStream)
        {
            return GetBinaryFormatter().Deserialize(serializationStream);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static object Deserialize(BinaryReader reader)
        {
            return GetBinaryFormatter().Deserialize(reader.BaseStream);
        }
        #endregion

        #region [Deserialize] NetData
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static T Deserialize<T>(Stream serializationStream) where T : class
        {
            var obj = Deserialize(serializationStream);
            return obj as T;
        }
        #endregion

    }

}
