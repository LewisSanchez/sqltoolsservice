//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#nullable disable

using Microsoft.SqlTools.Hosting.Protocol.Contracts;

namespace Microsoft.SqlTools.ServiceLayer.EditData.Contracts
{
    /// <summary>
    /// Parameters for getting tables referenced by the current edit session table
    /// </summary>
    public class GetReferencedTablesParams : SessionOperationParams
    {
    }

    /// <summary>
    /// Information about a referenced table
    /// </summary>
    public class ReferencedTableInfo
    {
        /// <summary>
        /// The schema name of the referenced table
        /// </summary>
        public string SchemaName { get; set; }

        /// <summary>
        /// The table name of the referenced table
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// The fully qualified name (schema.table)
        /// </summary>
        public string FullyQualifiedName { get; set; }

        /// <summary>
        /// The foreign key constraint name
        /// </summary>
        public string ForeignKeyName { get; set; }

        /// <summary>
        /// The columns in the current table that reference the foreign table
        /// </summary>
        public string[] SourceColumns { get; set; }

        /// <summary>
        /// The columns in the referenced table
        /// </summary>
        public string[] ReferencedColumns { get; set; }
    }

    /// <summary>
    /// Result containing tables referenced by the current edit session table
    /// </summary>
    public class GetReferencedTablesResult
    {
        /// <summary>
        /// List of tables that are referenced by foreign keys in the current table
        /// </summary>
        public ReferencedTableInfo[] ReferencedTables { get; set; }
    }

    /// <summary>
    /// Request definition for getting referenced tables
    /// </summary>
    public class GetReferencedTablesRequest
    {
        public static readonly RequestType<GetReferencedTablesParams, GetReferencedTablesResult> Type = 
            RequestType<GetReferencedTablesParams, GetReferencedTablesResult>.Create("edit/getReferencedTables");
    }
}