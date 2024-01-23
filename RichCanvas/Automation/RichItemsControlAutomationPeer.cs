using Newtonsoft.Json;
using RichCanvas.Automation.ControlInformations;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace RichCanvas.Automation
{
    // TODO: ScrollingGridAutomationPeer : IScrollProvider
    public class RichItemsControlAutomationPeer : SelectorAutomationPeer, IValueProvider
    //ITransformProvider
    {
        protected RichItemsControl OwnerItemsControl => (RichItemsControl)Owner;

        /// <inheritdoc/>
        public bool IsReadOnly => true;

        /// <inheritdoc/>
        public string Value => JsonConvert.SerializeObject(new RichItemsControlData
        {
            CurrentState = OwnerItemsControl.CurrentState,
            TranslateTransformX = OwnerItemsControl.TranslateTransform.X,
            TranslateTransformY = OwnerItemsControl.TranslateTransform.Y
        }, Formatting.Indented, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

        public RichItemsControlAutomationPeer(RichItemsControl owner) : base(owner)
        {
        }

        /// <inheritdoc/>
        public void SetValue(string value)
        {
            //TODO: maybe deserialize a json form a specific types with allowed dependency props 
            // that are modifiable and serializable
            throw new System.NotSupportedException("This control does not allow setting the value.");
        }

        public override object GetPattern(PatternInterface patternInterface) => patternInterface switch
        {
            PatternInterface.Value => this,
            _ => base.GetPattern(patternInterface)
        };

        /// <inheritdoc/>
        protected override AutomationControlType GetAutomationControlTypeCore()
            => AutomationControlType.Custom;

        /// <inheritdoc/>
        protected override string GetClassNameCore() => Owner.GetType().Name;

        /// <inheritdoc/>
        protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
            => new RichItemContainerAutomationPeer(item, this);
    }
}
