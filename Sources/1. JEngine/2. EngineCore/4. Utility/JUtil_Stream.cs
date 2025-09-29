using System;
using System.IO;

//using Random = UnityEngine.Random;
using System.Xml.Serialization;

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
        // Serialize
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Serialize] XML
        //------------------------------------------------------------------------------------------------------------------------------------------------------	
        public static void Serialize(string filename, System.Object o)
        {
            var writer = new XmlSerializer(o.GetType());
            var wfile = new System.IO.StreamWriter(filename);
            writer.Serialize(wfile, o);
            wfile.Close();
        }
        #endregion

        #region [Deserialize] XML
        //------------------------------------------------------------------------------------------------------------------------------------------------------	
        public static T Deserialize<T>(string filename)
        {
            var reader = new XmlSerializer(typeof(T));
            var file = new StreamReader(filename);
            var obj = (T)reader.Deserialize(file);
            file.Close();
            return obj;
        }
        #endregion

        #region EMail 전송
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void SendEMail(string mail_sender, string mail_receiver, string subject, string body)
        {
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            message.To.Add(mail_receiver);
            message.Subject = subject;
            message.From = new System.Net.Mail.MailAddress(mail_sender);
            message.Body = body;
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("yoursmtphost");
            smtp.Send(message);
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [StreamHelper]
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [StreamHelper] SerializableStream
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static byte[] MakeSerializableStream(Action<MemoryStream> fun)
        {
            //var formatter = new BinaryFormatter();
            byte[] result_buffer;
            using (var stream = new MemoryStream(1024 * 1024))
            {
                fun(stream);
                result_buffer = stream.GetBuffer();
                //Console.WriteLine("serializable:" + stream.position);
            }
            return result_buffer;
        }
        #endregion

        #region [StreamHelper] DeserializableStream
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void MakeDeserializableStream(byte[] buffer, Action<MemoryStream> fun)
        {
            //var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream(buffer))
            {
                fun(stream);
                //Console.WriteLine("serializable:" + stream.position);
            }
        }
        #endregion

        #region [StreamHelper] SerializableStream
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static byte[] MakeSerializableWriter(Action<BinaryWriter> fun)
        {
            //var formatter = new BinaryFormatter();
            byte[] result_buffer;
            using (var stream = new MemoryStream(1024 * 1024))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    fun(writer);
                }

                result_buffer = stream.GetBuffer();
            }
            return result_buffer;
        }
        #endregion

        #region [StreamHelper] DeserializableStream
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void MakeDeserializableReader(byte[] buffer, Action<BinaryReader> fun)
        {
            //var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream(buffer))
            {
                using (var reader = new BinaryReader(stream))
                {
                    fun(reader);
                }
                //Console.WriteLine("serializable:" + stream.position);
            }
        }
        #endregion

    }



}
