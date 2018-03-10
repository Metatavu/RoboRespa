using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using NAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using MediaToolkit;
using MediaToolkit.Model;

public class ResponsiveVoiceSynthesizer : MonoBehaviour
{
    public AudioSource audioSource;
    private bool finished = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator startDownload(string text)
    {
        var escaped = WWW.EscapeURL(text);
        Debug.Log("Starting request to responsivevoice: " + text);

        using (WWW response = new WWW("https://code.responsivevoice.org/getvoice.php?t=" + escaped + "&tl=fi&sv=g2&vn=&pitch=0.5&rate=0.5&vol=1"))
        {
            yield return response;

            Debug.Log("Gotten response from responsivevoice: " + response.error);

            string mp3Path = @"C:\Users\Ilmo Euro\Desktop\foobar\foobar.mp3";

            Directory.CreateDirectory(Path.GetDirectoryName(mp3Path));
            File.WriteAllBytes(mp3Path, response.bytes);

            string wavPath = @"C:\Users\Ilmo Euro\Desktop\foobar\foobar.wav";

            var mp3File = new MediaFile { Filename = mp3Path };
            var wavFile = new MediaFile { Filename = wavPath };

            using (var engine = new Engine())
            {
                finished = false;
                engine.Convert(mp3File, wavFile);
            }

            yield return new WaitForSeconds(2f);

            using (WWW clipWww = new WWW("file:///" + wavPath))
            {
                Debug.Log("Playing clip");
                var clip = clipWww.GetAudioClip();

                audioSource.clip = clip;
                audioSource.Play();
            }
        }
    }

    public void Speak(string text)
    {
        StartCoroutine(startDownload(text));
    }
}
