using System;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using LibHac.Ns;

namespace Emignatik.NxFileViewer.Views.TreeItems.Impl;

public class NacpItemViewModel : DirectoryEntryItemViewModel
{
    private readonly NacpItem _nacpItem;

    public NacpItemViewModel(NacpItem nacpItem, IServiceProvider serviceProvider)
        : base(nacpItem, serviceProvider)
    {
        _nacpItem = nacpItem;
    }

    [PropertyView]
    public string StartupUserAccount => _nacpItem.StartupUserAccount.ToString();

    [PropertyView]
    public string UserAccountSwitchLock => _nacpItem.UserAccountSwitchLock.ToString();
    
    [PropertyView]
    public string AddOnContentRegistrationType => _nacpItem.AddOnContentRegistrationType.ToString();

    [PropertyView]
    public string Attribute => _nacpItem.Attribute.ToString();

    [PropertyView]
    public ApplicationControlProperty.ParentalControlFlagValue ParentalControl => _nacpItem.ParentalControl;

    [PropertyView]
    public string Screenshot => _nacpItem.Screenshot.ToString();

    [PropertyView]
    public string VideoCapture => _nacpItem.VideoCapture.ToString();

    [PropertyView]
    public string DataLossConfirmation => _nacpItem.DataLossConfirmation.ToString();

    [PropertyView]
    public string PlayLogPolicy => _nacpItem.PlayLogPolicy.ToString();

    [PropertyView]
    public string PresenceGroupId => _nacpItem.PresenceGroupId;
    
    [PropertyView]
    public string DisplayVersion => _nacpItem.DisplayVersion; 
    
    [PropertyView]
    public string AddOnContentBaseId => _nacpItem.AddOnContentBaseId;   
    
    [PropertyView]
    public string SaveDataOwnerId => _nacpItem.SaveDataOwnerId;
    
    [PropertyView]
    public string ApplicationErrorCodeCategory => _nacpItem.ApplicationErrorCodeCategory;

    [PropertyView]
    public string LogoType => _nacpItem.LogoType.ToString();
    
    [PropertyView]
    public string LogoHandling => _nacpItem.LogoHandling.ToString(); 
    
    [PropertyView]
    public string StartupUserAccountOption => _nacpItem.StartupUserAccountOption.ToString();

    [PropertyView]
    public string Isbn => _nacpItem.Isbn;
    
    [PropertyView]
    public string BcatPassphrase => _nacpItem.BcatPassphrase;  
    

}