using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // สำคัญมาก
        }
        else
        {
            Destroy(gameObject); // กันซ้ำ
        }
    }
}