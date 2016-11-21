#region Directives

using System.Collections;
using System.Linq;
using UnityEngine;

#endregion

public class testMic : MonoBehaviour
{
    public string Device;

    public AudioClip StreamClip;

    public AudioSource Src;

    // Use this for initialization
    IEnumerator Start()
    {

        foreach (var device in Microphone.devices.Where(device => device.Contains("Yeti")))
        {
            Device = device;
            break;
        }

        Src.clip = Microphone.Start(deviceName: Device, loop: true, lengthSec: 30,
            frequency: 44100);

        yield return new WaitForEndOfFrame();
       
        Src.Play();
    }

}