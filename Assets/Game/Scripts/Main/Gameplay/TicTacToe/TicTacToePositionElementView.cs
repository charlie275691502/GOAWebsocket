using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.TicTacToe
{
    public class TicTacToePositionElementView : MonoBehaviour
    {
        public abstract record State()
        {
            public record Cross(bool IsGhost) : State;
            public record Circle(bool IsGhost) : State;
            public record Empty() : State;
        }

        public record Property(State State);

		[SerializeField]
		private GameObject _crossGameObject;
		[SerializeField]
		private GameObject _crossGhostGameObject;
		[SerializeField]
		private GameObject _circleGameObject;
		[SerializeField]
		private GameObject _circleGhostGameObject;
		[SerializeField]
		private Button _button;

		private Property _prop;

		public void RegisterCallback(Action onClickButton)
		{
			_button.onClick.AddListener(() => onClickButton?.Invoke());
		}

		public void Render(Property prop)
		{
			if (_prop == prop)
				return;
			_prop = prop;

			switch (prop.State)
			{
				case State.Cross info:
					_crossGameObject.SetActive(!info.IsGhost);
					_crossGhostGameObject.SetActive(info.IsGhost);
					_circleGameObject.SetActive(false);
					_circleGhostGameObject.SetActive(false);
					break;

				case State.Circle info:
					_crossGameObject.SetActive(false);
					_crossGhostGameObject.SetActive(false);
					_circleGameObject.SetActive(!info.IsGhost);
					_circleGhostGameObject.SetActive(info.IsGhost);
					break;

				case State.Empty:
					_crossGameObject.SetActive(false);
					_crossGhostGameObject.SetActive(false);
					_circleGameObject.SetActive(false);
					_circleGhostGameObject.SetActive(false);
					break;

				default:
					break;
			}
		}
	}
}
