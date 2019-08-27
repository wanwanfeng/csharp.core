using UnityEngine;
using UnityEngine.UI;

namespace Library
{
    public partial class CacheUIMonoBehaviour : CacheMonoBehaviour
    {
        private Canvas _canvas;
        public Canvas canvas
        {
            get { return _canvas == null ? (_canvas = gameObject.GetComponentInParent<Canvas>()) : _canvas; }
            set { _canvas = value; }
        }

        //public Image _panel;
        //public Image panel
        //{
        //    get { return _panel == null ? (_panel = gameObject.GetComponentInParent<Image>()) : _panel; }
        //    set { _panel = value; }
        //}

        private Image _image;
        public Image image
        {
            get { return _image == null ? (_image = gameObject.GetComponent<Image>()) : _image; }
            set { _image = value; }
        }

        private RawImage _rawImage;
        public RawImage rawImage
        {
            get { return _rawImage == null ? (_rawImage = gameObject.GetComponent<RawImage>()) : _rawImage; }
            set { _rawImage = value; }
        }

        private Text _text;
        public Text text
        {
            get { return _text == null ? (_text = gameObject.GetComponent<Text>()) : _text; }
            set { _text = value; }
        }

        //private Text[] _texts;
        //public Text[] texts
        //{
        //    get { return _texts == null ? (_texts = gameObject.GetComponentsInChildren<Text>(true)) : _texts; }
        //    set { _texts = value; }
        //}

        private Button _button;
        public Button button
        {
            get { return _button == null ? (_button = gameObject.GetComponent<Button>()) : _button; }
            set { _button = value; }
        }

        private Toggle _toggle;
        public Toggle toggle
        {
            get { return _toggle == null ? (_toggle = gameObject.GetComponent<Toggle>()) : _toggle; }
            set { _toggle = value; }
        }

        private InputField _inputField;
        public InputField inputField
        {
            get { return _inputField == null ? (_inputField = gameObject.GetComponent<InputField>()) : _inputField; }
            set { _inputField = value; }
        }

        private Slider _slider;
        public Slider slider
        {
            get { return _slider == null ? (_slider = gameObject.GetComponent<Slider>()) : _slider; }
            set { _slider = value; }
        }

        private Scrollbar _scrollbar;
        public Scrollbar scrollbar
        {
            get { return _scrollbar == null ? (_scrollbar = gameObject.GetComponent<Scrollbar>()) : _scrollbar; }
            set { _scrollbar = value; }
        }
    }
}