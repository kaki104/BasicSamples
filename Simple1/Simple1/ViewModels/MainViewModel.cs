using System.Windows.Input;
using Simple1.Helpers;

namespace Simple1.ViewModels
{
    public class MainViewModel : Observable
    {
        private string _inputNumbers;

        public MainViewModel()
        {
            Init();
        }

        /// <summary>
        ///     입력 커맨드
        /// </summary>
        public ICommand InputCommand { get; set; }

        /// <summary>
        ///     입력된 숫자를 출력할 문자열
        /// </summary>
        public string InputNumbers
        {
            get => _inputNumbers;
            set => Set(ref _inputNumbers, value);
        }

        private void Init()
        {
            InputNumbers = "0";
            InputCommand = new RelayCommand<object>(ExecuteInputCommand);
        }

        /// <summary>
        ///     인풋 커맨드 실행
        /// </summary>
        /// <param name="obj"></param>
        private void ExecuteInputCommand(object obj)
        {
            if (!(obj is string number)) return;

            var num = int.Parse(InputNumbers.Replace(",", ""));

            //backspace키는 shell에서 사용하고 있기 때문에 사용이 않되는 듯..
            switch (number)
            {
                case "*":
                    break;
                case "B":
                    num = num.ToString().Length == 1
                        ? 0
                        : int.Parse(num.ToString().Substring(0, num.ToString().Length - 1));
                    break;
                default:
                    num = int.Parse(num + number);
                    break;
            }

            InputNumbers = string.Format("{0:N0}", num);
        }
    }
}
