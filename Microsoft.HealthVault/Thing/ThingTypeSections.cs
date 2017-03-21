// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// Enumeration used to specify the sections of thing type definition that should be returned.
    /// </summary>
    ///
    [Flags]
    public enum ThingTypeSections
    {
        /// <summary>
        /// Indicates no information about the thing type definition should be
        /// returned.
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Indicates the core information about the thing type definition
        /// should be returned.
        /// </summary>
        Core = 0x1,

        /// <summary>
        /// Indicates the schema of the thing type definition should be returned.
        /// </summary>
        Xsd = 0x2,

        /// <summary>
        /// Indicates the columns used by the thing type definition should be returned.
        /// </summary>
        Columns = 0x4,

        /// <summary>
        /// Indicates the transforms supported by the thing type definition
        /// should be returned.
        /// </summary>
        Transforms = 0x8,

        /// <summary>
        /// Indicates the transforms and their XSL source supported by the health record
        /// item type definition should be returned.
        /// </summary>
        TransformSource = 0x10,

        /// <summary>
        /// Indicates the versions of the thing type definition should be returned.
        /// </summary>
        Versions = 0x20,

        /// <summary>
        /// Indicates the effective date XPath of the thing type definition
        /// should be returned.
        /// </summary>
        EffectiveDateXPath = 0x40,

        /// <summary>
        /// Indicates all information for the thing type definition
        /// should be returned.
        /// </summary>
        All = Core | Xsd | Columns | Transforms | TransformSource | Versions | EffectiveDateXPath
    }
}