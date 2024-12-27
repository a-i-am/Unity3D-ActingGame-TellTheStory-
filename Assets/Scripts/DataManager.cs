using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    private void Awake()
    {
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
    }
    public void SaveRecordedAudio(byte[] wavData, string filePath, string fileName)
    {
        // 경로 생성
        string directoryPath = Path.Combine(Application.persistentDataPath, filePath); // 폴더 경로
        string fileFullPath = Path.Combine(directoryPath, fileName); // 파일 경로

        // 디렉토리 확인 및 생성
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // 파일 저장
        File.WriteAllBytes(fileFullPath, wavData);

        Debug.Log($"Audio file saved at: {fileFullPath}");
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
        int npcId = GameManager.Instance.currentNPC;

        Role currentRole = ActingLineTriggerManager.instance.currentRole;
        int roleIndex = currentRole == Role.Player ? 0 : 1;

        int npcLine = roleIndex == 0 ? ActingLineTriggerManager.instance.playerLineIndex : ActingLineTriggerManager.instance.npcLineIndex;

        GameManager.Instance.npcCurrentLine[npcId] = npcLine;
        GameManager.Instance.npcCurrentRole[npcId] = roleIndex;

        PlayerPrefs.SetInt($"NPC{npcId}_Line", npcLine);
        PlayerPrefs.SetInt($"NPC{npcId}_Role", roleIndex);
        PlayerPrefs.Save();

        Debug.Log($"[SaveCurrentData] NPC ID: {npcId}, Saved Line: {npcLine}");
    }

    public void LoadGameData()
    {
        for (int i = 0; i < 4; i++)
        {
            GameManager.Instance.npcCurrentLine[i] = PlayerPrefs.GetInt($"NPC{i}_Line", 0);
            GameManager.Instance.npcCurrentRole[i] = PlayerPrefs.GetInt($"NPC{i}_Role", 0);
            Debug.Log($"[LoadGameData] NPC : {i}, Loaded Line : {GameManager.Instance.npcCurrentLine[i]}, Loaded Role: {GameManager.Instance.npcCurrentRole[i]}");
        }
    }
    public void NewGame()
    {
        PlayerPrefs.DeleteAll();

        string path = Application.persistentDataPath;

        if (Directory.Exists(path))
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            // 모든 파일 삭제
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            }

            // 모든 폴더 삭제
            foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
            {
                directory.Delete(true); // true: 하위 파일/폴더까지 삭제
            }

            Debug.Log("Application.persistentDataPath의 모든 데이터가 삭제되었습니다.");
        }
        else
        {
            Debug.LogWarning("Application.persistentDataPath 경로가 존재하지 않습니다.");
        }
    }
}
