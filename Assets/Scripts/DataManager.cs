using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        instance = null;
    }
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

    // WAV 데이터를 AudioClip으로 변환
    public AudioClip WavToAudioClip(byte[] wavData)
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
        int npcId = GameManager.instance.currentNPC;

        Role currentRole = ActingLineTriggerManager.instance.currentRole;
        int roleIndex = currentRole == Role.Player ? 0 : 1;

        int npcLine = roleIndex == 0 ? ActingLineTriggerManager.instance.playerLineIndex : ActingLineTriggerManager.instance.npcLineIndex;

        GameManager.instance.npcCurrentLine[npcId] = npcLine;
        GameManager.instance.npcCurrentRole[npcId] = roleIndex;

        PlayerPrefs.SetInt($"NPC{npcId}_Line", npcLine);
        PlayerPrefs.SetInt($"NPC{npcId}_Role", roleIndex);
        PlayerPrefs.Save();

        Debug.Log($"[SaveCurrentData] NPC ID: {npcId}, Saved Line: {npcLine}");
    }
    public void OnNpcFinished(int npcId)
    {
        GameManager.instance.npcFinished[npcId] = 1;//1은 끝났다는 의미 => 재생이 가능하다.
        PlayerPrefs.SetInt($"NPC{npcId}_Finished", 1);
    }
    public void OnNpcNewGame(int npcId)
    {
        GameManager.instance.npcFinished[npcId] = 0;//0은 진행 중이거나 시작 전이라는 의미 => 재생이 불가능하다.
        PlayerPrefs.SetInt($"NPC{npcId}_Finished", 0);
    }
    public void LoadGameData()
    {
        for (int i = 0; i < 4; i++)
        {
            GameManager.instance.npcCurrentLine[i] = PlayerPrefs.GetInt($"NPC{i}_Line", 0);
            GameManager.instance.npcCurrentRole[i] = PlayerPrefs.GetInt($"NPC{i}_Role", 0);
            GameManager.instance.npcFinished[i] = PlayerPrefs.GetInt($"NPC{i}_Finished", 0);
            Debug.Log($"[LoadGameData] NPC : {i}, Loaded Line : {GameManager.instance.npcCurrentLine[i]}, Loaded Role: {GameManager.instance.npcCurrentRole[i]}");
        }
    }
    public void NewGame()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("ExistData", 1);
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
    public AudioClip[] GetNPCClips(int npcId, int actId)
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>($"NPC{npcId}/Act{actId}");
        return clips;
    }
    public AudioClip[] GetRecoredClips(int npcIndex, int actIndex)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, $"NPC{npcIndex}/Act{actIndex}");

        if (!Directory.Exists(folderPath))
        {
            Debug.LogError($"Folder not found: {folderPath}");
            return null;
        }

        // 폴더 내의 모든 wav 파일 가져오기
        string[] files = Directory.GetFiles(folderPath, "*.wav");

        if (files.Length == 0)
        {
            Debug.LogError($"No .wav files found in: {folderPath}");
            return null;
        }
        List<byte[]> fileDataList = new();
        foreach (string file in files)
        {
            byte[] fileData = File.ReadAllBytes(file);
            fileDataList.Add(fileData);
        }
        AudioClip[] recordedClips = fileDataList.Select(item => DataManager.instance.WavToAudioClip(item)).ToArray();
        return recordedClips;
    }
    [ContextMenu("DeleteAllPrefs")]
    public void DeleteAllPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
