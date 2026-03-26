using UnityEngine;

/// <summary>
/// 音频播放句柄。
/// 用于让业务层停止一次已经发起的播放请求。
/// </summary>
public sealed class AudioHandle {
    public string HandleId {
        get;
        private set;
    }

    internal AudioSource Source {
        get;
        set;
    }

    internal bool IsPooled {
        get;
        set;
    }

    internal string AudioId {
        get;
        set;
    }

    public bool IsPlaying {
        get {
            return Source != null && Source.isPlaying;
        }
    }

    public AudioHandle(string handleId) {
        HandleId = handleId;
    }

    public void Stop() {
        AudioManager.Instance.Stop(this);
    }
}
