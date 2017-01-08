using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using GreedyNameSpace;
using System.Text.RegularExpressions;
using Networking.Data;
using System;

namespace Networking {

    public static class Serializer {
        public static byte[] Seal<T>(Protocols protocol, T instance) {
            string integrated;
            if (typeof(T) == typeof(string) || typeof(T) == typeof(int))
                integrated = protocol + "/" + instance;
            else
                integrated = protocol + "/" + JsonUtility.ToJson(instance);
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream()) {
                bf.Serialize(ms, integrated);
                return ms.ToArray();
            }
        }

        public static byte[] Seal(Protocols protocol) {
            string integrated = protocol + "/";
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream()) {
                bf.Serialize(ms, integrated);
                return ms.ToArray();
            }
        }

        public static Decipher UnSeal(int tail, byte[] binary_data) {
            Stream stream = new MemoryStream(binary_data);
            BinaryFormatter f = new BinaryFormatter();
            string msg = f.Deserialize(stream).ToString();
            string[] msg_array = Regex.Split(msg, "/");
            return new Decipher(msg_array[0], msg_array[1], tail);
        }

        public static byte[] Serialize<T>(T instace) {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream()) {
                bf.Serialize(ms, instace);
                return ms.ToArray();
            }
        }

        public static T DeSerialize<T>(byte[] arrBytes) {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            T instance = (T)binForm.Deserialize(memStream);
            return instance;
        }

        public static int SizeInBytes(string msg) {
            return System.Text.ASCIIEncoding.Unicode.GetByteCount(msg);
        }


        //public static byte[] ObjectToByteArray(object obj) {
        //    if (obj == null)
        //        return null;
        //    BinaryFormatter bf = new BinaryFormatter();
        //    using (MemoryStream ms = new MemoryStream()) {
        //        bf.Serialize(ms, obj);
        //        return ms.ToArray();
        //    }
        //}

        //public static T ByteArrayToObject<T>(byte[] arrBytes) {
        //    MemoryStream memStream = new MemoryStream();
        //    BinaryFormatter binForm = new BinaryFormatter();
        //    memStream.Write(arrBytes, 0, arrBytes.Length);
        //    memStream.Seek(0, SeekOrigin.Begin);
        //    T obj = (T)binForm.Deserialize(memStream);
        //    return obj;
        //}
    }


}
