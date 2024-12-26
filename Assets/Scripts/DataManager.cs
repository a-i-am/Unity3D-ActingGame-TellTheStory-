using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    private void Awake()
    {
        Debug.Log("Persistent Data Path: " + Application.persistentDataPath);
        if (instance)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        var source = gameObject.AddComponent<AudioSource>();
        var clip = LoadAudioClipFromWav("Test.wav");
        source.PlayOneShot(clip);
    }
    public void SaveRecordedAudio(byte[] wavData, string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllBytes(path, wavData);
    }
    public AudioClip LoadAudioClipFromWav(string fileName)
    {
        // 파일 경로 생성
        string path = Path.Combine(Application.persistentDataPath, fileName);

        if (!File.Exists(path))
        {
            Debug.LogError($"파일이 존재하지 않습니다: {path}");
            return null;
        }

        // WAV 데이터를 읽어들임
        byte[] wavData = File.ReadAllBytes(path);

        // WAV 데이터를 AudioClip으로 변환
        AudioClip audioClip = WavToAudioClip(wavData);
        if (audioClip != null)
        {
            Debug.Log($"WAV 파일 로드 성공: {path}");
        }
        else
        {
            Debug.LogError("WAV 파일 로드 실패.");
        }

        return audioClip;
    }

    // WAV 데이터를 AudioClip으로 변환
    private AudioClip WavToAudioClip(byte[] wavData)
    {
        // 파일 최소 길이 확인
        if (wavData.Length < 44)
        {
            Debug.LogError("WAV 파일이 너무 짧습니다.");
            return null;
        }

        // 채널 수와 샘플 레이트 읽기
        int channels = System.BitConverter.ToInt16(wavData, 22);
        int sampleRate = System.BitConverter.ToInt32(wavData, 24);

        // "data" 청크 위치 탐색
        int dataStartIndex = -1;
        for (int i = 0; i < wavData.Length - 8; i++)
        {
            if (wavData[i] == 'd' && wavData[i + 1] == 'a' && wavData[i + 2] == 't' && wavData[i + 3] == 'a')
            {
                dataStartIndex = i + 8; // "data" 청크 이후 데이터 시작
                break;
            }
        }

        if (dataStartIndex == -1)
        {
            Debug.LogError("WAV 파일에서 'data' 청크를 찾을 수 없습니다.");
            return null;
        }

        // 데이터 크기 계산
        int dataSize = wavData.Length - dataStartIndex;
        if (dataSize <= 0)
        {
            Debug.LogError("WAV 파일에 유효한 데이터가 없습니다.");
            return null;
        }

        // 샘플 수 계산 (16비트 기준)
        int sampleCount = dataSize / 2;

        // 샘플 데이터를 float 배열로 변환
        float[] samples = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            short sample = System.BitConverter.ToInt16(wavData, dataStartIndex + (i * 2));
            samples[i] = sample / 32768f; // 정규화
        }

        // AudioClip 생성
        AudioClip audioClip = AudioClip.Create("LoadedWavClip", sampleCount / channels, channels, sampleRate, false);
        audioClip.SetData(samples, 0);

        return audioClip;
    }
    public void SaveCurrentData()
    {
    
    }
    public void LoadGameData()
    {
    
    }
}