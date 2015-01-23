using UnityEditor.AnimatedValues;
using UnityEngine;

namespace FullInspector.Internal {
    /// <summary>
    /// Metadata used for the dropdown. A dropdown GUI is automatically displayed for all property
    /// editors that are invoked with IPropertyEditorExtensions.Edit/GetElementHeight.
    /// </summary>
    public class DropdownMetadata : IGraphMetadataItem {
        /// <summary>
        /// Is the foldout currently active, ie, is the rendered item being displayed or is the
        /// short-form foldout being displayed?
        /// </summary>
        public bool IsActive {
            get {
                return _isActive.value;
            }
            set {
                if (value != _isActive.target) {
                    _isActive.target = value;
                }
            }
        }

        /// <summary>
        /// What percentage are we at in the animation between active states?
        /// </summary>
        public float AnimPercentage {
            get {
                return _isActive.faded;
            }
        }
        [SerializeField]
        private AnimBool _isActive = new AnimBool(true);


        /// <summary>
        /// Are we currently animating between different states?
        /// </summary>
        public bool IsAnimating {
            get {
                return _isActive.isAnimating;
            }
        }

        /// <summary>
        /// Should we render a dropdown? This will be false if the override has been set *or* if
        /// the element is not above a certain minimum height.
        /// </summary>
        public bool ShowDropdown {
            get {
                return OverrideDisable == false && _showDropdown;
            }
            set {
                if (OverrideDisable && value) {
                    return;
                }
                _showDropdown = value;
            }
        }
        [SerializeField]
        private bool _showDropdown;

        /// <summary>
        /// Does the property editor use a non-standard label and should an extra indent be given
        /// when the arrow is being drawn?
        /// </summary>
        [SerializeField]
        public bool ExtraIndent;

        /// <summary>
        /// Should rendering of the dropdown be completely skipped?
        /// </summary>
        [SerializeField]
        public bool OverrideDisable;
    }

}