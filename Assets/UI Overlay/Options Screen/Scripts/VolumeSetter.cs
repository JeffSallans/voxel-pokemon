using UnityEngine;

public class VolumeSetter : MonoBehaviour
{
    private GameOptions gameOptions;
    private AudioSource music;

    // Start is called before the first frame update
    void Start()
    {
        music = GetComponent<Camera>().GetComponent<AudioSource>();
        if (music == null)
        {
            Debug.LogWarning("Couldn't get music AudioSource to set volume");
            return;
        }

        var character = FindObjectOfType<CharacterController>();
        if (character != null)
        {
            var playerObject = character.gameObject;
            gameOptions = playerObject.GetComponent<GameOptions>();

            music.volume = gameOptions.musicVolume;
        }
        else
        {
            // There's no player available; use a default.
            music.volume = GameOptions.DefaultMusicVolume;
        }

        // TODO: once the UI allows changing the volume, trigger setting it again somehow.
    }

    // Update is called once per frame
    void Update()
    {

    }
}
