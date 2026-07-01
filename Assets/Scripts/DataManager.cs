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

        string directoryPath = Path.Combine(Application.persistentDataPath, filePath);
        string fileFullPath = Path.Combine(directoryPath, fileName);


        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }


        File.WriteAllBytes(fileFullPath, wavData);

        Debug.Log($"Audio file saved at: {fileFullPath}");
    }


    public AudioClip WavToAudioClip(byte[] wavData)
    {

        if (wavData.Length < 44)
        {
            Debug.LogError("WAV 파일이 너무 짧습니다.");
            return null;
        }


        int channels = System.BitConverter.ToInt16(wavData, 22);
        int sampleRate = System.BitConverter.ToInt32(wavData, 24);


        int dataStartIndex = -1;
        for (int i = 0; i < wavData.Length - 8; i++)
        {
            if (wavData[i] == 'd' && wavData[i + 1] == 'a' && wavData[i + 2] == 't' && wavData[i + 3] == 'a')
            {
                dataStartIndex = i + 8;
                break;
            }
        }

        if (dataStartIndex == -1)
        {
            Debug.LogError("WAV 파일에서 'data' 청크를 찾을 수 없습니다.");
            return null;
        }


        int dataSize = wavData.Length - dataStartIndex;
        if (dataSize <= 0)
        {
            Debug.LogError("WAV 파일에 유효한 데이터가 없습니다.");
            return null;
        }


        int sampleCount = dataSize / 2;


        float[] samples = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            short sample = System.BitConverter.ToInt16(wavData, dataStartIndex + (i * 2));
            samples[i] = sample / 32768f;
        }


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
        GameManager.instance.npcFinished[npcId] = 1;
        PlayerPrefs.SetInt($"NPC{npcId}_Finished", 1);
    }
    public void OnNpcNewGame(int npcId)
    {
        GameManager.instance.npcFinished[npcId] = 0;
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


            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            }


            foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
            {
                directory.Delete(true);
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
