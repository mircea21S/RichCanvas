using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

using Newtonsoft.Json;

using RichCanvas.Automation.ControlInformations;

namespace RichCanvas.Automation
{
    /// <summary>
    /// Exposes the <see cref="RichCanvasContainer"/> to UI Automation project.
    /// </summary>
    public class RichCanvasContainerAutomationPeer : SelectorItemAutomationPeer, IValueProvider
    {
        /// <summary>
        /// Gets the <see cref="RichCanvas"/> that is associated with this <see cref="RichCanvasContainerAutomationPeer"/>.
        /// </summary>
        protected RichCanvas OwnerRichCanvas => (RichCanvas)ItemsControlAutomationPeer.Owner;

        /// <summary>
        /// Gets the <see cref="RichCanvasContainer"/> that is associated with this <see cref="RichCanvasContainerAutomationPeer"/>.
        /// </summary>
        protected RichCanvasContainer Container => (RichCanvasContainer)OwnerRichCanvas.ItemContainerGenerator.ContainerFromItem(Item);

        /// <summary>
        /// Gets the serialized json value of <see cref="RichCanvasContainerData"/> containing data about the associated <see cref="RichCanvasContainer"/>.
        /// </summary>
        public string Value => JsonConvert.SerializeObject(new RichCanvasContainerData
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
        /// Initializes a new <see cref="RichCanvasContainerAutomationPeer"/> for a <see cref="RichCanvasContainer"/> in <see cref="RichCanvas"/>.Items collection.
        /// </summary>
        /// <param name="item">The data item associated with a <see cref="RichCanvasContainer"/> inside a <see cref="RichCanvas"/></param>
        /// <param name="itemsControlAutomationPeer">Owner <see cref="RichCanvasAutomationPeer"/></param>
        public RichCanvasContainerAutomationPeer(object item, SelectorAutomationPeer itemsControlAutomationPeer) : base(item, itemsControlAutomationPeer)
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
        protected override string GetClassNameCore() => nameof(RichCanvasContainer);
    }
}