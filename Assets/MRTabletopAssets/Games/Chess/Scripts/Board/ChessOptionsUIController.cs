using System;
using UnityEngine;
using UnityLabs.SmartUX.Interaction.Widgets;

namespace UnityLabs.Slices.Games.Chess
{
    public struct OptionState : IEquatable<OptionState>
    {
        public static float defaultTimeAmount => TimeIndexToValue(defaultTimeAmountIndex);

        public bool slideBoard;
        public bool showLegalMoves;
        public bool playSoundFx;
        public bool timeControl;
        public int timeAmountIndex;
        public bool pressConfirm;
        public float timeAmountValue => TimeIndexToValue(timeAmountIndex);

        const int defaultTimeAmountIndex = 3;

        public OptionState(bool slideBoard, bool showLegalMoves, bool playSoundFx, bool timeControl, int timeAmountIndex, bool pressConfirm)
        {
            this.slideBoard = slideBoard;
            this.showLegalMoves = showLegalMoves;
            this.playSoundFx = playSoundFx;
            this.timeControl = timeControl;
            this.timeAmountIndex = timeAmountIndex;
            this.pressConfirm = pressConfirm;
        }

        public OptionState(OptionState other)
        {
            this.slideBoard = other.slideBoard;
            this.showLegalMoves = other.showLegalMoves;
            this.playSoundFx = other.playSoundFx;
            this.timeControl = other.timeControl;
            this.timeAmountIndex = other.timeAmountIndex;
            this.pressConfirm = other.pressConfirm;
        }

        public static OptionState DefaultOptionsState()
        {
            return new OptionState(false, true, true, true, defaultTimeAmountIndex, false);
        }

        public bool Equals(OptionState other)
        {
            return slideBoard == other.slideBoard &&
                   showLegalMoves == other.showLegalMoves &&
                   playSoundFx == other.playSoundFx &&
                   timeControl == other.timeControl &&
                   timeAmountIndex == other.timeAmountIndex &&
                   pressConfirm == other.pressConfirm;
        }

        public override bool Equals(object obj)
        {
            return obj is OptionState other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = slideBoard.GetHashCode();
                hashCode = (hashCode * 397) ^ showLegalMoves.GetHashCode();
                hashCode = (hashCode * 397) ^ playSoundFx.GetHashCode();
                hashCode = (hashCode * 397) ^ timeControl.GetHashCode();
                hashCode = (hashCode * 397) ^ timeAmountIndex.GetHashCode();
                hashCode = (hashCode * 397) ^ pressConfirm.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"Options: slideboard {slideBoard} | showLegalMoves {showLegalMoves} | playSoundFx {playSoundFx} | timeControl{timeControl} | timeAmount{timeAmountIndex} | pressClock {pressConfirm}";
        }

        static float TimeIndexToValue(int timeIndex)
        {
            const float secondsInMinute = 60f;
            return timeIndex switch
            {
                0 => 5f * secondsInMinute,
                1 => 10f * secondsInMinute,
                2 => 20f * secondsInMinute,
                3 => 40f * secondsInMinute,
                _ => 0f
            };
        }
    }

    public class ChessOptionsUIController : MonoBehaviour
    {
        [SerializeField]
        XRToggle m_LegalMovesToggler = null;

        [SerializeField]
        XRToggle m_SoundFxToggler = null;

        [SerializeField]
        XRToggle m_AccessibilityToggler = null;

        [SerializeField]
        XRToggle m_TimeControlToggler = null;

        [SerializeField]
        XRSlider m_TimeAmountSlider = null;

        [SerializeField]
        XRToggle m_PressConfirmToggler = null;

        public OptionState optionsState
        {
            get => new OptionState(slideBoard, showLegalMoves, playSoundFx, timeControl, timeAmountIndex, pressConfirm);
            set
            {
                showLegalMoves = value.showLegalMoves;
                playSoundFx = value.playSoundFx;
                timeControl = value.timeControl;
                timeAmountIndex = value.timeAmountIndex;
                pressConfirm = value.pressConfirm;
                slideBoard = value.slideBoard;
            }
        }

        bool slideBoard { get => m_AccessibilityToggler.isToggled.Value; set => m_AccessibilityToggler.isToggled.Value = value; }

        bool showLegalMoves { get => m_LegalMovesToggler.isToggled.Value; set => m_LegalMovesToggler.isToggled.Value = value; }

        bool playSoundFx { get => m_SoundFxToggler.isToggled.Value; set => m_SoundFxToggler.isToggled.Value = value; }

        bool timeControl { get => m_TimeControlToggler.isToggled.Value; set => m_TimeControlToggler.isToggled.Value = value; }

        int timeAmountIndex { get => m_TimeAmountSlider.snapIndex.Value; set => m_TimeAmountSlider.SetSnapIndex(value); }

        bool pressConfirm { get => m_PressConfirmToggler.isToggled.Value; set => m_PressConfirmToggler.isToggled.Value = value; }
    }
}
