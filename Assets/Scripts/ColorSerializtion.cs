using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;

public class ColorSerialization
{
    public static byte[] SerializeColor(object targetObject)
    {
        Color color = (Color)targetObject;
        Quaternion colorToQuaterinon = new Quaternion(color.r, color.g, color.b, color.a);
        byte[] bytes = Protocol.Serialize(colorToQuaterinon);
        return bytes;
    }
    public static object DeserializeColor(byte[] bytes)
    {
        Quaternion quaterinon = (Quaternion)Protocol.Deserialize(bytes);
        Color color = new Color(quaterinon.x, quaterinon.y, quaterinon.z, quaterinon.w);
        return color;
    }
}
