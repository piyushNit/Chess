using UnityEngine;
using System.Collections;

public class selfDestroy : MonoBehaviour {
    private ParticleSystem particleSystem;

    void Start() {
        particleSystem = this.GetComponent<ParticleSystem>();
    }

    void Update() {
        if (!particleSystem.isPlaying)
            Destroy(this.gameObject);
    }
}
