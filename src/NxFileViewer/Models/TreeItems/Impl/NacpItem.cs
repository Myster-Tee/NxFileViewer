using System;
using Emignatik.NxFileViewer.Utils;
using LibHac.Ns;
using LibHac.Tools.Fs;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

public class NacpItem : DirectoryEntryItem
{
    public const string NACP_FILE_NAME = "control.nacp";

    public NacpItem(ApplicationControlProperty nacp, SectionItem parentItem, DirectoryEntryEx directoryEntry)
        : base(parentItem, directoryEntry)
    {
        Nacp = nacp;
        ParentItem = parentItem ?? throw new ArgumentNullException(nameof(parentItem));
        AddOnContentBaseId = Nacp.AddOnContentBaseId.ToStrId();
        PresenceGroupId = Nacp.PresenceGroupId.ToStrId();
        SaveDataOwnerId = Nacp.SaveDataOwnerId.ToStrId();
    }


    public new SectionItem ParentItem { get; }

    public ApplicationControlProperty Nacp { get; }

    public override string Format => nameof(Nacp);

    public ApplicationControlProperty.StartupUserAccountValue StartupUserAccount => Nacp.StartupUserAccount;

    public ApplicationControlProperty.UserAccountSwitchLockValue UserAccountSwitchLock => Nacp.UserAccountSwitchLock;

    public ApplicationControlProperty.AddOnContentRegistrationTypeValue AddOnContentRegistrationType => Nacp.AddOnContentRegistrationType;

    public ApplicationControlProperty.AttributeFlagValue Attribute => Nacp.AttributeFlag;
    
    public ApplicationControlProperty.ParentalControlFlagValue ParentalControl => Nacp.ParentalControlFlag;

    public ApplicationControlProperty.ScreenshotValue Screenshot => Nacp.Screenshot;

    public ApplicationControlProperty.VideoCaptureValue VideoCapture => Nacp.VideoCapture;

    public ApplicationControlProperty.DataLossConfirmationValue DataLossConfirmation => Nacp.DataLossConfirmation;

    public ApplicationControlProperty.PlayLogPolicyValue PlayLogPolicy => Nacp.PlayLogPolicy;

    public string PresenceGroupId { get; }

    public string DisplayVersion => Nacp.DisplayVersionString.ToString();
    
    public string AddOnContentBaseId { get; }

    public string SaveDataOwnerId { get; }

    public string ApplicationErrorCodeCategory => Nacp.ApplicationErrorCodeCategoryString.ToString();

    public ApplicationControlProperty.LogoTypeValue LogoType => Nacp.LogoType;

    public ApplicationControlProperty.LogoHandlingValue LogoHandling => Nacp.LogoHandling;

    public ApplicationControlProperty.StartupUserAccountOptionFlagValue StartupUserAccountOption => Nacp.StartupUserAccountOption;
    
    public string Isbn => Nacp.IsbnString.ToString();
    
    public string BcatPassphrase => Nacp.BcatPassphraseString.ToString();

}