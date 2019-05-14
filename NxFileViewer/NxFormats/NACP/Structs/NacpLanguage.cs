using System;

namespace Emignatik.NxFileViewer.NxFormats.NACP.Structs
{
    /// <summary>
    /// Base on "https://switchbrew.org/wiki/Control.nacp#Title_Entry"
    /// </summary>
    [Flags]
    public enum NacpLanguage : uint
    {
        AmericanEnglish      = 0b0000000000000001,
        BritishEnglish       = 0b0000000000000010,
        Japanese             = 0b0000000000000100,
        French               = 0b0000000000001000,
        German               = 0b0000000000010000,
        LatinAmericanSpanish = 0b0000000000100000,
        Spanish              = 0b0000000001000000,
        Italian              = 0b0000000010000000,
        Dutch                = 0b0000000100000000,
        CanadianFrench       = 0b0000001000000000,
        Portuguese           = 0b0000010000000000,
        Russian              = 0b0000100000000000,
        Korean               = 0b0001000000000000,
        TraditionalChinese   = 0b0010000000000000, // SwitchBrew specifies "Taiwanese" but it seems to be "TraditionalChinese"
        SimplifiedChinese    = 0b0100000000000000, // SwitchBrew specifies "Chinese" but it seems to be "SimplifiedChinese"
        Unknown              = 0b1000000000000000,
    }
}

