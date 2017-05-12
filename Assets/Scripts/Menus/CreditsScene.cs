using System.Collections;
using System.Collections.Generic;
using Prime31.TransitionKit;
using UnityEngine;

public class CreditsScene : MonoBehaviour {

    private Timer _playerPoseTimer;
    private Timer _abilityColorTimer;
    public  GameObject player;
    // Use this for initialization
    private bool _startPosing;

    public Material Material;
    private Color _nextColor;
    private Color _curColor;
    public Color[] Colors;


    private int cnt;
    void Start ()
    {
        _abilityColorTimer = new Timer(2f);
        StartCoroutine("BossDeafeatedCutscene");
    }

    private void UpdateAbilityColor()
    {
        if (_abilityColorTimer.Update(Time.deltaTime))
        {
            //Material.SetColor("_Color", _curColor);
            cnt++;
            if (cnt >= Colors.Length)
                cnt = 0;
            _curColor = _nextColor;
            _nextColor = Colors[cnt];
            _abilityColorTimer.ResetToSurplus();
        }
        else
        {
            Material.SetColor("_Color",Color.Lerp(_curColor, _nextColor, _abilityColorTimer.PercentDone));

        }
    }

    // Update is called once per frame
    void Update () {
        // Animation things
	    if (_startPosing)
	    {
	        if (_playerPoseTimer.Update(Time.deltaTime))
	        {
	            player.GetComponent<Animator>().SetTrigger("NextPose");
	            _playerPoseTimer.ResetToSurplus();
	        }

            UpdateAbilityColor();
        }


        // lerp hair here

	}

    private IEnumerator BossDeafeatedCutscene()
    {
        yield return new WaitForSeconds(3.0f);
        var time = GetComponent<MusicPlayer>().MusicClip.length;

        player.GetComponent<Animator>().SetTrigger("StartPose");
        var poseTime = 5f; // How long each pose will take
        _playerPoseTimer = new Timer(poseTime);
        _startPosing = true;

        yield return new WaitForSeconds(time - 1.5f); // HACK


        var gameManager = GameManager.Instance;
        //gameManager.BeatLevel(SceneManager.GetActiveScene().name);
        gameManager.SaveToMemory();
        gameManager.SaveToFiles();


        var fishEye = new FishEyeTransition()
        {
            nextScene = "Start",
            duration = 5.0f,
            size = 0.2f,
            zoom = 100.0f,
            colorSeparation = 0.1f
        };
        TransitionKit.instance.transitionWithDelegate(fishEye);
    }
}
