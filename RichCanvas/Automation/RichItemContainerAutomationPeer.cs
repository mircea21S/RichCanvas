using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

using Newtonsoft.Json;

using RichCanvas.Automation.ControlInformations;

namespace RichCanvas.Automation
{
    /// <summary>
    /// Exposes the <see cref="RichItemContainer"/> to UI Automation project.
    /// </summary>
    public class RichItemContainerAutomationPeer : SelectorItemAutomationPeer, IValueProvider
    {
        /// <summary>
        /// Gets the <see cref="RichItemsControl"/> that is associated with this <see cref="RichItemContainerAutomationPeer"/>.
        /// </summary>
        protected RichItemsControl OwnerItemsControl => (RichItemsControl)ItemsControlAutomationPeer.Owner;

        /// <summary>
        /// Gets the <see cref="RichItemContainer"/> that is associated with this <see cref="RichItemContainerAutomationPeer"/>.
        /// </summary>
        protected RichItemContainer Container => (RichItemContainer)OwnerItemsControl.ItemContainerGenerator.ContainerFromItem(Item);

        /// <summary>
        /// Gets the serialized json value of <see cref="RichItemContainerData"/> containing data about the associated <see cref="RichItemContainer"/>.
        /// </summary>
        public string Value => JsonConvert.SerializeObject(new RichItemContainerData
        {
            Top = Container.Top,
            Left = Container.Left,
            IsSelected = Container.IsSelected,
            ScaleX = Container.ScaleTransform?.ScaleX ?? -1,
            ScaleY = Container.ScaleTransform?.ScaleY ?? -1,
            DataContextType = Container.DataContext.GetType()
        });

        /// <inheritdoc/>
        public bool IsReadOnly => true;

        /// <summary>
        /// Initializes a new <see cref="RichItemContainerAutomationPeer"/> for a <see cref="RichItemContainer"/> in <see cref="RichItemsControl"/>.Items collection.
        /// </summary>
        /// <param name="item">The data item associated with a <see cref="RichItemContainer"/> inside a <see cref="RichItemsControl"/></param>
        /// <param name="itemsControlAutomationPeer">Owner <see cref="RichItemsControlAutomationPeer"/></param>
        public RichItemContainerAutomationPeer(object item, SelectorAutomationPeer itemsControlAutomationPeer) : base(item, itemsControlAutomationPeer)
        {
        }

        /// <inheritdoc/>
        public override object GetPattern(PatternInterface patternInterface) => patternInterface switch
        {
            PatternInterface.Value => this,
            _ => base.GetPattern(patternInterface)
        };

        /// <inheritdoc/>
        public void SetValue(string value)
        {
            throw new System.NotSupportedException("This control does not allow setting the value.");
        }

        /// <inheritdoc/>
        protected override AutomationControlType GetAutomationControlTypeCore()
            => AutomationControlType.Custom;

        /// <inheritdoc/>
        protected override string GetClassNameCore() => nameof(RichItemContainer);
    }
}