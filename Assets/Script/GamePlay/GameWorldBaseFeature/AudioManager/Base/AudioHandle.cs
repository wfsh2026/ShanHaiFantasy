using UnityEngine;

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
