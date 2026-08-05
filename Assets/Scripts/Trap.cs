using System;
using UnityEngine;

public class Trap : MonoBehaviour, IInitialization
{
    public Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            _spellMain = GetComponent<SpellMain>();
            _spellMain.InitializeMe(_br);
        }
    }
    Brain _br;
    SpellMain _spellMain;
}
