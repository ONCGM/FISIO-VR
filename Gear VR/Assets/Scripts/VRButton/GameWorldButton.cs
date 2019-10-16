using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

// Holds all custom button classes for vr use.
namespace VRButton {
    /// <summary>
    /// A base button class to be placed in the game world.
    /// </summary>
    [RequireComponent(typeof(Button), typeof(Image))]
    public class GameWorldButton : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler  {

        public virtual void OnPointerDown(PointerEventData eventData) { }

        public virtual void OnPointerClick(PointerEventData eventData) { }
        
        public virtual void OnPointerUp(PointerEventData eventData) { }

        public virtual void OnPointerEnter(PointerEventData eventData) { }

        public virtual void OnPointerExit(PointerEventData eventData) { }
    }
}
