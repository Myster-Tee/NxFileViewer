using System;

namespace Emignatik.NxFileViewer.Models.Overview;

/// <summary>
/// Based on "https://switchbrew.org/wiki/ControlPartition.nacp#Title_Entry"
/// Values 0–15 correspond to LibHac's NacpLanguage enum and the legacy uncompressed NACP format.
/// Values 16+ are extended languages present only in the compressed title block introduced in newer firmware.
/// </summary>
[Flags]
public enum NacpLanguage : uint
{
    AmericanEnglish      = 0,
    BritishEnglish       = 1,
    Japanese             = 2,
    French               = 3,
    German               = 4,
    LatinAmericanSpanish = 5,
    Spanish              = 6,
    Italian              = 7,
    Dutch                = 8,
    CanadianFrench       = 9,
    Portuguese           = 10,
    Russian              = 11,
    Korean               = 12,
    TraditionalChinese   = 13, // SwitchBrew specifies "Taiwanese" but it seems to be "TraditionalChinese"
    SimplifiedChinese    = 14, // SwitchBrew specifies "Chinese" but it seems to be "SimplifiedChinese"
    BrazilianPortuguese  = 15,

    // Extended languages — present only in the new compressed NACP title block format
    Polish               = 16,
    Thai                 = 17,
    Indonesian           = 18,
    Romanian             = 19,
    Vietnamese           = 20,
    Arabic               = 21,
    Ukrainian            = 22,
    Czech                = 23,
    Slovak               = 24,
    Greek                = 25,
    Hungarian            = 26,
    Norwegian            = 27,
    Finnish              = 28,
    Swedish              = 29,
    Danish               = 30,
}

/// <summary>
/// A name/publisher pair for a single NACP language entry.
/// </summary>
public record NacpTitleEntry(string Name, string Publisher);