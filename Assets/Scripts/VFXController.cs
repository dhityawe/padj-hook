using UnityEngine;
using GabrielBigardi.SpriteAnimator;
using System;

public class VFXController : MonoBehaviour
{
    [SerializeField] private SpriteAnimator vfxAnimator;

    private void OnEnable()
    {
        Enemy.OnCursedEffect += PlayCursedExplosion;
    }

    private void OnDisable()
    {
        Enemy.OnCursedEffect -= PlayCursedExplosion;
    }

    public void PlayCursedExplosion()
    {
        vfxAnimator.Play("CursedExplosion");
    }
}
