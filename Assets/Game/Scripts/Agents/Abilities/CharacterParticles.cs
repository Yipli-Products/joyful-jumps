using UnityEngine;
using System.Collections;

public class CharacterParticles : CharacterAbility
{
    [Header("Character Particles")]
    public ParticleSystem RunParticles;
    public ParticleSystem SliperryParticles;
    public ParticleSystem JumpParticles;

    protected ParticleSystem.EmissionModule _emissionModule;
    protected CharacterStates.MovementStates _stateLastFrame;

    public override void ProcessAbility()
    {
        base.ProcessAbility();
        HandleParticleSystem(RunParticles, CharacterStates.MovementStates.Running);
        HandleParticleSystem(SliperryParticles, CharacterStates.MovementStates.WalkingOnSlipery);
        HandleParticleSystem(JumpParticles, CharacterStates.MovementStates.Jumping);

        _stateLastFrame = _movement.CurrentState;
    }


    protected virtual void HandleParticleSystem(ParticleSystem system, CharacterStates.MovementStates state)
    {
        if (system == null)
        {
            return;
        }
        if (_movement.CurrentState == state)
        {
            if (!system.main.loop && _stateLastFrame != state)
            {
                system.Clear();
                system.Play();
            }
            _emissionModule = system.emission;
            _emissionModule.enabled = true;
        }
        else
        {
            _emissionModule = system.emission;
            _emissionModule.enabled = false;
        }
    }
}
