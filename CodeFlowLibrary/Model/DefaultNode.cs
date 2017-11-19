using CodeFlowLibrary.History;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace CodeFlowLibrary.Model
{
    [Serializable]
    public class DefaultNode : VisualNode {

        #region Ctor

        public DefaultNode() { }
        public DefaultNode(HistoryController hc) : base(hc) { }

        #endregion

        #region Fields

        private Point _location;
        private Size  _size;
        private Color _background;
        private Color _foreground;
        private BorderType _borderType;
        private ShapeType  _shapeType;

        #endregion

        [Browsable(false)]
        public Point Location {
            get { return _location; }
            set {
                if (_location == value)
                    return;
                NotifyPropertyChanging(nameof(Location));
                _location = value;
                NotifyPropertyChanged(nameof(Location));
            }
        }

        [Browsable(false)]
        public Size Size {
            get { return _size; }
            set {
                if (_size == value) return;
                NotifyPropertyChanging(nameof(Size));
                _size = value;
                NotifyPropertyChanged(nameof(Size));
            }
        }

        [Category("Appearance")]
        public Color Background {
            get { return _background; }
            set {
                if (_background == value) return;
                NotifyPropertyChanging(nameof(Background));
                _background = value;
                NotifyPropertyChanged(nameof(Background));
            }
        }

        [Category("Appearance")]
        public Color Foreground {
            get { return _foreground; }
            set {
                if (_foreground == value) return;
                NotifyPropertyChanging(nameof(Foreground));
                _foreground = value;
                NotifyPropertyChanged(nameof(Foreground));
            }
        }

        [Category("Appearance")]
        [DisplayName("Border Type")]
        public BorderType BorderType {
            get { return _borderType; }
            set {
                if (_borderType == value) return;
                _borderType = value;
                NotifyPropertyChanged(nameof(BorderType));
            }
        }

        [Category("Appearance")]
        [DisplayName("Shape")]
        public ShapeType ShapeType {
            get { return _shapeType; }
            set {
                if (_shapeType == value) return;
                _shapeType = value;
                NotifyPropertyChanged(nameof(ShapeType));
            }
        }
    }
}
