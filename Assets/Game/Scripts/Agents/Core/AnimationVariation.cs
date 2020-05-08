using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AnimVariation {
    public int maxVariation;
    public string animationString;
}

public class AnimationVariation : MonoBehaviour
{
    public AnimVariation[] totalVariations;
    private Animator _animator;

    private float _delta;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        SetVariation();
        _animator.SetBool("Idle", true);
    }

    protected virtual void Update()
    {
        if (_animator)
        {
            _delta += Time.deltaTime;
            if (_delta >= 5f)
            {
                _delta = 0f;
                SetVariation();
            }
        }
    }

    protected virtual void SetVariation() {
        for (int i = 0; i < totalVariations.Length; i++)
        {
            int random = Random.Range(0, totalVariations[i].maxVariation);
            _animator.SetInteger(totalVariations[i].animationString, random);
        }
    }
}
