﻿using Cut_the_diet_game.Scripts.Managers;
using UnityEngine;

namespace Cut_the_diet_game.Scripts.UiScripts
{
    public class ResetProgressButton : MonoBehaviour {


        public void OnClick()
        {
            ProgressManager.Reset();
        }

    }
}
