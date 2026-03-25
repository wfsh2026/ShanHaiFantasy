using UnityEngine;

public sealed class UITestAudioDemoController : MonoBehaviour {
    private const string DEMO_OBJECT_NAME = "UITestAudioEmitterCube";
    private GameObject demoEmitterObject;

    private void Awake() {
        EnsureDemoEmitterObject();
    }

    public void ToggleDemoEmitterObject() {
        EnsureDemoEmitterObject();
        if (demoEmitterObject == null) {
            return;
        }

        bool nextActive = !demoEmitterObject.activeSelf;
        demoEmitterObject.SetActive(nextActive);
        AudioManager.Instance.PlayWorldSfx("scene_reload", demoEmitterObject.transform.position);
    }

    private void EnsureDemoEmitterObject() {
        if (demoEmitterObject != null) {
            return;
        }

        GameObject foundObject = GameObject.Find(DEMO_OBJECT_NAME);
        if (foundObject != null) {
            demoEmitterObject = foundObject;
            EnsureEmitterComponent(demoEmitterObject);
            return;
        }

        demoEmitterObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        demoEmitterObject.name = DEMO_OBJECT_NAME;
        demoEmitterObject.transform.position = new Vector3(0f, 1f, 4f);
        demoEmitterObject.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
        EnsureEmitterComponent(demoEmitterObject);

        Renderer renderer = demoEmitterObject.GetComponent<Renderer>();
        if (renderer != null) {
            renderer.material.color = new Color(0.3f, 0.75f, 1f, 1f);
        }
    }

    private static void EnsureEmitterComponent(GameObject targetObject) {
        AudioEmitter emitter = targetObject.GetComponent<AudioEmitter>();
        if (emitter == null) {
            emitter = targetObject.AddComponent<AudioEmitter>();
        }

        emitter.AudioId = "demo_emitter_loop";
        emitter.PlayOnEnable = true;
        emitter.StopOnDisable = true;
        emitter.PlayOnVisible = false;
        emitter.StopOnInvisible = false;
    }
}
