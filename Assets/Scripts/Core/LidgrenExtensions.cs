
using UnityEngine;

namespace Lidgren.Network.Extensions
{

    public static class NetBufferExtensions
    {
        public static void ExtendedWrite(this NetBuffer msg, Vector3 value)
        {
            msg.Write(value.x);
            msg.Write(value.y);
            msg.Write(value.z);
        }

        public static void ExtendedWrite(this NetBuffer msg, Quaternion value)
        {
            msg.Write(value.x);
            msg.Write(value.y);
            msg.Write(value.z);
            msg.Write(value.w);
        }

        public static Vector3 ReadVector3(this NetBuffer msg)
        {
            Vector3 value = new();
            value.x = msg.ReadFloat();
            value.y = msg.ReadFloat();
            value.z = msg.ReadFloat();
            return value;
        }

        public static Quaternion ReadQuaternion(this NetBuffer msg)
        {
            Quaternion value = new();
            value.x = msg.ReadFloat();
            value.y = msg.ReadFloat();
            value.z = msg.ReadFloat();
            value.w = msg.ReadFloat();
            return value;
        }

    }
}