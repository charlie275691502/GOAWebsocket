using System;

namespace Common.Observable
{
    public class Observable<T>
    {
		private T _value;
        private Action<T> _action;

		public T Value
        {
            get
            {
                return _value;
            }

            set
            {
                if (_value.Equals(value))
                    return;

                _value = value;
                _action?.Invoke(_value);
            }
        }

        public Observable(T value, Action<T> action)
        {
            _value = value;
            _action = action;
        }
    }
}