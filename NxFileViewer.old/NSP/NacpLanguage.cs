﻿using System;

namespace Emignatik.NxFileViewer.NSP
{
    /// <summary>
    /// Base on "https://switchbrew.org/wiki/ControlPartition.nacp#Title_Entry"
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
        Unknown              = 15,
    }
}
