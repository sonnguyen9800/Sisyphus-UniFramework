using TMPro;

namespace SisyphusFramework.Utils
{
    public class TextMeshDebugger
    {
        private TextMeshPro _debugger = null;


        public TextMeshDebugger(TextMeshPro txt)
        {
            _debugger = txt;
        }
        
        public void ToggleEnable(bool enable)
        {
            _debugger.gameObject.SetActive(enable);
        }

        public void ShowText(string content)
        {
            _debugger.text = content;
        }
    }
}