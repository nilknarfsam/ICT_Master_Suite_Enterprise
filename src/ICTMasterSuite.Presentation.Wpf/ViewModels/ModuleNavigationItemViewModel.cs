using CommunityToolkit.Mvvm.ComponentModel;
using ICTMasterSuite.Domain.Enums;

namespace ICTMasterSuite.Presentation.Wpf.ViewModels;

public partial class ModuleNavigationItemViewModel : ObservableObject
{
    public SystemModule Module { get; }
    public string Title { get; }
    public string Description { get; }

    public ModuleNavigationItemViewModel(SystemModule module, string title, string description)
    {
        Module = module;
        Title = title;
        Description = description;
    }
}
