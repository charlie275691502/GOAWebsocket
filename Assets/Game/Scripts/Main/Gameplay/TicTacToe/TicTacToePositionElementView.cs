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
            public record Circle(bool IsGhost) : State;
            public record Cross(bool IsGhost) : State;
			public record Empty() : State;
        }

        public record Property(State State);

		[SerializeField]
		private GameObject _circleGameObject;
		[SerializeField]
		private GameObject _circleGhostGameObject;
		[SerializeField]
		private GameObject _crossGameObject;
		[SerializeField]
		private GameObject _crossGhostGameObject;
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
				case State.Circle info:
					_circleGameObject.SetActive(!info.IsGhost);
					_circleGhostGameObject.SetActive(info.IsGhost);
					_crossGameObject.SetActive(false);
					_crossGhostGameObject.SetActive(false);
					break;

				case State.Cross info:
					_circleGameObject.SetActive(false);
					_circleGhostGameObject.SetActive(false);
					_crossGameObject.SetActive(!info.IsGhost);
					_crossGhostGameObject.SetActive(info.IsGhost);
					break;

				case State.Empty:
					_circleGameObject.SetActive(false);
					_circleGhostGameObject.SetActive(false);
					_crossGameObject.SetActive(false);
					_crossGhostGameObject.SetActive(false);
					break;

				default:
					break;
			}
		}
	}
}
