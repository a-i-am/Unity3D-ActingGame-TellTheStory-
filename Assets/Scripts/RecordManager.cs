using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RecordManager : MonoBehaviour
{
    public static RecordManager instance;

    [Header("STT Settings")]
    private static readonly string ClientId = Environment.GetEnvironmentVariable("NAVER_CLOVA_CLIENT_ID");
    private static readonly string ClientSecret = Environment.GetEnvironmentVariable("NAVER_CLOVA_CLIENT_SECRET");
    private const string API_URL = "https://naveropenapi.apigw.ntruss.com/recog/v1/stt?lang=Kor";
    private AudioClip recordedClip;

    public Action<string, AudioClip> onSttResult;

    [Header("Waveform Settings")]
    private float amplitudeSize = 15f;
    private float width = 8f;
    private int vertexCount = 400;
    public GameObject arrow;

    private Mesh mesh;
    private MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    private Vector3[] vertices;
    private int[] triangles;
    private Coroutine arrowCoroutine;
    private float stepSize;
    private float recordingDuration;
    private float minY = 0.03f;
    private float maxY = 2.5f;

    private GameManager gameManager;
    private bool stopFlag;
    private string sttResult;

    private void Awake()
    {
        instance = this;
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        InitWaveformMesh();
        meshRenderer.enabled = false;
    }
    private void Start()
    {
        gameManager = GameManager.instance;
    }
    public void InitWaveformMesh()
    {
        mesh = new Mesh();
        meshFilter.mesh = mesh;

        vertices = new Vector3[vertexCount * 2];
        for (int i = 0; i < vertexCount; i++)
        {
            float x = i * (width / (vertexCount - 1));
            vertices[i] = new Vector3(x, 0, 0);
            vertices[i + vertexCount] = new Vector3(x, 0, 0);
        }

        triangles = new int[(vertexCount - 1) * 6];
        for (int i = 0; i < vertexCount - 1; i++)
        {
            int topLeft = i;
            int topRight = i + 1;
            int bottomLeft = i + vertexCount;
            int bottomRight = i + 1 + vertexCount;

            triangles[i * 6] = topLeft;
            triangles[i * 6 + 1] = bottomLeft;
            triangles[i * 6 + 2] = topRight;

            triangles[i * 6 + 3] = topRight;
            triangles[i * 6 + 4] = bottomLeft;
            triangles[i * 6 + 5] = bottomRight;
        }

        stepSize = width / (vertexCount - 1);
    }

    public void StartRecording(float duration)
    {
        SoundManager.instance.TurnOffBGM();
        meshRenderer.enabled = true;
        stopFlag = false;
        InitWaveformMesh();
        recordingDuration = duration;
        StopAllCoroutines();
        StartCoroutine(RecordingCoroutine(duration));
    }

    private IEnumerator RecordingCoroutine(float duration)
    {
        recordedClip = Microphone.Start(null, false, Mathf.CeilToInt(duration), 44100);
        while (!(Microphone.GetPosition(null) > 0)) yield return null;

        InitArrow();
        arrowCoroutine = StartCoroutine(UpdateArrowPosition());

        float startTime = Time.time;
        while (Time.time - startTime < duration && !stopFlag)
        {
            float elapsedTime = Time.time - startTime;
            ProcessWaveformSamples(elapsedTime);
            yield return null;
        }

        Microphone.End(null);
        StopCoroutine(arrowCoroutine);
        MoveArrow(1.0f);

        byte[] currentWavData = ConvertAudioClipToWav(recordedClip);
        yield return StartCoroutine(SendSpeechRecognitionRequest(currentWavData));
        onSttResult?.Invoke(sttResult, recordedClip);
        DataManager.instance.SaveRecordedAudio(currentWavData, $"NPC{gameManager.currentNPC}/Act{gameManager.currentAct}", $"Line{gameManager.npcCurrentLine[gameManager.currentNPC] + 1}.wav");
        sttResult = null;
    }

    private IEnumerator SendSpeechRecognitionRequest(byte[] audioData)
    {
        if (string.IsNullOrWhiteSpace(ClientId) || string.IsNullOrWhiteSpace(ClientSecret))
        {
            Debug.LogError("NAVER CLOVA credentials are not configured.");
            yield break;
        }

        using UnityWebRequest request = new UnityWebRequest(API_URL, "POST");
        request.SetRequestHeader("X-NCP-APIGW-API-KEY-ID", ClientId);
        request.SetRequestHeader("X-NCP-APIGW-API-KEY", ClientSecret);
        request.SetRequestHeader("Content-Type", "application/octet-stream");
        request.uploadHandler = new UploadHandlerRaw(audioData);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("STT 성공: " + request.downloadHandler.text);


            sttResult = request.downloadHandler.text
                .Replace("{\"text\":\"", "")
                .Replace("\"}", "");


        }
        else
        {
            Debug.LogError("STT 실패: " + request.error);
        }
    }
    public void StopRecording()
    {
        SoundManager.instance.TurnOnBGM();
        stopFlag = true;


        int micPosition = Microphone.GetPosition(null);
        if (micPosition > 0 && recordedClip != null)
        {
            recordedClip = TrimAudioClip(recordedClip, micPosition);
        }
    }

    private AudioClip TrimAudioClip(AudioClip clip, int length)
    {
        int channels = clip.channels;
        int sampleRate = clip.frequency;


        float[] samples = new float[length * channels];
        clip.GetData(samples, 0);


        AudioClip trimmedClip = AudioClip.Create(clip.name + "_Trimmed", length, channels, sampleRate, false);
        trimmedClip.SetData(samples, 0);

        return trimmedClip;
    }


    private void ProcessWaveformSamples(float elapsedTime)
    {
        int micPosition = Microphone.GetPosition(null);
        int totalSamples = recordedClip.samples;

        int sampleStart = Mathf.FloorToInt((elapsedTime / recordingDuration) * totalSamples);
        int sampleEnd = Mathf.FloorToInt(((elapsedTime + (1f / vertexCount)) / recordingDuration) * totalSamples);

        if (sampleEnd > totalSamples) sampleEnd = totalSamples;
        int sampleCount = sampleEnd - sampleStart;
        if (sampleCount <= 0) return;

        float[] samples = new float[sampleCount];
        recordedClip.GetData(samples, sampleStart);

        float averageAmplitude = samples.Length > 0 ? Mathf.Abs(samples.Average()) : 0;
        int vertexIndex = Mathf.FloorToInt((elapsedTime / recordingDuration) * (vertexCount - 1));
        UpdateVertex(vertexIndex, averageAmplitude * amplitudeSize);
        UpdateWaveformMesh();
    }

    private void UpdateVertex(int index, float amplitude)
    {
        float x = index * stepSize;
        float y = Mathf.Max(amplitude, minY);
        y = Mathf.Min(y, maxY);
        vertices[index] = new Vector3(x, y, 0);
        vertices[index + vertexCount] = new Vector3(x, -y, 0);
    }

    private void UpdateWaveformMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    private void InitArrow()
    {
        if (arrow != null)
        {
            arrow.transform.position = new Vector3(0, arrow.transform.position.y, arrow.transform.position.z);
        }
    }

    private IEnumerator UpdateArrowPosition()
    {
        float startTime = Time.time;

        while (Time.time - startTime < recordingDuration)
        {
            float elapsedTime = Time.time - startTime;
            float progress = Mathf.Clamp01(elapsedTime / recordingDuration);
            MoveArrow(progress);
            yield return null;
        }

        MoveArrow(1.0f);
    }

    private void MoveArrow(float progress)
    {
        if (arrow != null)
        {
            float xOffset = progress * width;
            arrow.transform.position = new Vector3(xOffset, arrow.transform.position.y, arrow.transform.position.z);
        }
    }
    private byte[] ConvertAudioClipToWav(AudioClip clip)
    {
        using MemoryStream stream = new MemoryStream();
        int headerSize = 44;
        int sampleCount = clip.samples * clip.channels;
        int sampleRate = clip.frequency;
        int bytesPerSample = 2;
        int fileSize = headerSize + sampleCount * bytesPerSample;

        stream.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"), 0, 4);
        stream.Write(System.BitConverter.GetBytes(fileSize - 8), 0, 4);
        stream.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"), 0, 4);
        stream.Write(System.Text.Encoding.ASCII.GetBytes("fmt "), 0, 4);
        stream.Write(System.BitConverter.GetBytes(16), 0, 4);
        stream.Write(System.BitConverter.GetBytes((short)1), 0, 2);
        stream.Write(System.BitConverter.GetBytes((short)clip.channels), 0, 2);
        stream.Write(System.BitConverter.GetBytes(clip.frequency), 0, 4);
        stream.Write(System.BitConverter.GetBytes(clip.frequency * clip.channels * bytesPerSample), 0, 4);
        stream.Write(System.BitConverter.GetBytes((short)(clip.channels * bytesPerSample)), 0, 2);
        stream.Write(System.BitConverter.GetBytes((short)(bytesPerSample * 8)), 0, 2);
        stream.Write(System.Text.Encoding.ASCII.GetBytes("data"), 0, 4);
        stream.Write(System.BitConverter.GetBytes(sampleCount * bytesPerSample), 0, 4);

        float[] samples = new float[sampleCount];
        clip.GetData(samples, 0);
        foreach (var sample in samples)
        {
            short intSample = (short)(sample * 32767);
            stream.Write(System.BitConverter.GetBytes(intSample), 0, 2);
        }

        return stream.ToArray();
    }
}
