using System.Collections.Generic;

using UnityEngine;

namespace Game
{
    public interface ICrosshairSelector
    {
        public static ICrosshairSelector GetInstance() => CrosshairSelectorBehaviour.GetInstance();

        ICrosshairSelectableObject? FindInSelection<T>() where T : Component, ICrosshairSelectableObject;
        
        void HideText();

        void ShowTarget(ICrosshairSelectableObject target, bool active);
    }
}