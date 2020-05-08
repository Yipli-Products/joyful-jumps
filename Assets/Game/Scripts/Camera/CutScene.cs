using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutScene : MonoBehaviour
{
    public Transform target;
    public float runningTime;

    protected PlayableDirector playableDirector;
    protected Animator animator;

    private bool _runningAnimation;

    private float _deltaTime;
    
    void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();
        animator = target.GetComponent<Animator>();
        animator.enabled = false;
    }

    public void PlayTimelineAnimation(Vector3 _position) {
        _position.y = 0;
        target.transform.localRotation = Quaternion.identity;
        target.transform.position = _position;
        animator.enabled = true;
        animator.SetTrigger("capsule_start");

        playableDirector.Play();
    }

    public void PlayEndCutSceneJump() {
        playableDirector.Stop();
        _runningAnimation = true;
    }

    public void StopEndCutScene() {
        _runningAnimation = false;
    }

    private void Update()
    {
        if (_runningAnimation) {
            _deltaTime += Time.deltaTime;
            if (_deltaTime >= runningTime)
            {

            }
            else {
                InputController.Instance.PlayMove(Move.Running);
            }
        }
    }
}
