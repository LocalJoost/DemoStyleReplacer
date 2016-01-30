using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;

namespace WpWinNl.Behaviors
{
  public class StyleReplaceBehavior : Behavior<Page>
  {
    public StyleReplaceBehavior()
    {
      ObservedStyles = new ObservableCollection<ObservedStyle>();
    }

    protected override void OnAttached()
    {
      base.OnAttached();
      AssociatedObject.Loaded += AssociatedObjectLoaded;
    }

    private void AssociatedObjectLoaded(object sender, RoutedEventArgs e)
    {
      AssociatedObject.Loaded -= AssociatedObjectLoaded;
      ReplaceStyles(CurrentStyle);
    }

    private void ReplaceStyles(string newStyleName)
    {
      //Find the current style in the object's resources by name
      var wantedStyle = AssociatedObject.Resources.FirstOrDefault(p => p.Key.ToString() == newStyleName && p.Value is Style).Value as Style;

      // Find all other styles observed by this behavior
      var otherStyleNames = ObservedStyles.Where(p => p.Name != newStyleName).Select(p => p.Name);
      var otherStyles = AssociatedObject.Resources.Where(p => otherStyleNames.Contains(p.Key.ToString())).Select(p => p.Value);

      // Find all the elements having the other styles
      var elementsToStyle = AssociatedObject.GetVisualDescendents().Where(p => otherStyles.Contains(p.Style));

      // Style those with the new style
      foreach (var elementToStyle in elementsToStyle)
      {
        elementToStyle.Style = wantedStyle;
      }
    }

    #region CurrentStyle

    /// <summary>
    /// CurrentStyle Property name
    /// </summary>
    public const string CurrentStylePropertyName = "CurrentStyle";

    public string CurrentStyle
    {
      get { return (string)GetValue(CurrentStyleProperty); }
      set { SetValue(CurrentStyleProperty, value); }
    }

    /// <summary>
    /// CurrentStyle Property definition
    /// </summary>
    public static readonly DependencyProperty CurrentStyleProperty = DependencyProperty.Register(
        CurrentStylePropertyName,
        typeof(string),
        typeof(StyleReplaceBehavior),
        new PropertyMetadata(default(string), CurrentStyleChanged));

    /// <summary>
    /// CurrentStyle property changed callback.
    /// </summary>
    /// <param name="d">The depency object (i.e. the behavior).</param>
    /// <param name="e">The property event args <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>  
    public static void CurrentStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var thisobj = d as StyleReplaceBehavior;
      var newValue = (string)e.NewValue;
      thisobj?.ReplaceStyles(newValue);
    }

    #endregion

    #region ObservedStyles

    /// <summary>
    /// ObservedStyles Property name
    /// </summary>
    public const string ObservedStylesPropertyName = "ObservedStyles";

    public ObservableCollection<ObservedStyle> ObservedStyles
    {
      get { return (ObservableCollection<ObservedStyle>)GetValue(ObservedStylesProperty); }
      set { SetValue(ObservedStylesProperty, value); }
    }

    /// <summary>
    /// ObservedStyles Property definition
    /// </summary>
    public static readonly DependencyProperty ObservedStylesProperty = DependencyProperty.Register(
        ObservedStylesPropertyName,
        typeof(ObservableCollection<ObservedStyle>),
        typeof(StyleReplaceBehavior),
        new PropertyMetadata(default(ObservableCollection<ObservedStyle>)));

    #endregion
  }
}
