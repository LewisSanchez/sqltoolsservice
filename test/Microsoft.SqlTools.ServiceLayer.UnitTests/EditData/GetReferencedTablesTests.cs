//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#nullable disable

using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SqlTools.ServiceLayer.EditData;
using Microsoft.SqlTools.ServiceLayer.EditData.Contracts;
using Microsoft.SqlTools.ServiceLayer.QueryExecution;
using Microsoft.SqlTools.ServiceLayer.Test.Common.RequestContextMocking;
using Moq;
using NUnit.Framework;

namespace Microsoft.SqlTools.ServiceLayer.UnitTests.EditData
{
    public class GetReferencedTablesTests
    {
        private const string TestOwnerUri = "test://referenced-tables";
        
        [Test]
        public async Task HandleGetReferencedTablesRequest_Success_WithReferencedTables()
        {
            // Arrange
            var expectedReferencedTables = new[]
            {
                new Microsoft.SqlTools.ServiceLayer.EditData.ReferencedTableInfo
                {
                    SchemaName = "dbo",
                    TableName = "Products",
                    FullyQualifiedName = "dbo.Products",
                    ForeignKeyName = "FK_Orders_Products",
                    SourceColumns = new[] { "ProductId" },
                    ReferencedColumns = new[] { "Id" }
                },
                new Microsoft.SqlTools.ServiceLayer.EditData.ReferencedTableInfo
                {
                    SchemaName = "dbo",
                    TableName = "Customers",
                    FullyQualifiedName = "dbo.Customers",
                    ForeignKeyName = "FK_Orders_Customers",
                    SourceColumns = new[] { "CustomerId" },
                    ReferencedColumns = new[] { "Id" }
                }
            };
            
            var eds = new EditDataService(null, null, null);
            var session = await CreateSessionWithReferencedTables(expectedReferencedTables);
            eds.ActiveSessions[TestOwnerUri] = session;
            
            var requestParams = new GetReferencedTablesParams { OwnerUri = TestOwnerUri };
            
            // Act & Assert
            var efv = new EventFlowValidator<GetReferencedTablesResult>()
                .AddResultValidation(result =>
                {
                    Assert.NotNull(result);
                    Assert.NotNull(result.ReferencedTables);
                    Assert.AreEqual(2, result.ReferencedTables.Length);
                    Assert.AreEqual("dbo.Products", result.ReferencedTables[0].FullyQualifiedName);
                    Assert.AreEqual("FK_Orders_Products", result.ReferencedTables[0].ForeignKeyName);
                    Assert.AreEqual("dbo.Customers", result.ReferencedTables[1].FullyQualifiedName);
                    Assert.AreEqual("FK_Orders_Customers", result.ReferencedTables[1].ForeignKeyName);
                })
                .Complete();
                
            await eds.HandleGetReferencedTablesRequest(requestParams, efv.Object);
            efv.Validate();
        }
        
        [Test]
        public async Task HandleGetReferencedTablesRequest_Success_WithNoReferencedTables()
        {
            // Arrange
            var eds = new EditDataService(null, null, null);
            var session = await CreateSessionWithReferencedTables(new Microsoft.SqlTools.ServiceLayer.EditData.ReferencedTableInfo[0]);
            eds.ActiveSessions[TestOwnerUri] = session;
            
            var requestParams = new GetReferencedTablesParams { OwnerUri = TestOwnerUri };
            
            // Act & Assert
            var efv = new EventFlowValidator<GetReferencedTablesResult>()
                .AddResultValidation(result =>
                {
                    Assert.NotNull(result);
                    Assert.NotNull(result.ReferencedTables);
                    Assert.AreEqual(0, result.ReferencedTables.Length);
                })
                .Complete();
                
            await eds.HandleGetReferencedTablesRequest(requestParams, efv.Object);
            efv.Validate();
        }
        
        [Test]
        public async Task HandleGetReferencedTablesRequest_Success_WithNullReferencedTables()
        {
            // Arrange
            var eds = new EditDataService(null, null, null);
            var session = await CreateSessionWithReferencedTables(null);
            eds.ActiveSessions[TestOwnerUri] = session;
            
            var requestParams = new GetReferencedTablesParams { OwnerUri = TestOwnerUri };
            
            // Act & Assert
            var efv = new EventFlowValidator<GetReferencedTablesResult>()
                .AddResultValidation(result =>
                {
                    Assert.NotNull(result);
                    Assert.NotNull(result.ReferencedTables);
                    Assert.AreEqual(0, result.ReferencedTables.Length);
                })
                .Complete();
                
            await eds.HandleGetReferencedTablesRequest(requestParams, efv.Object);
            efv.Validate();
        }
        
        [Test]
        public async Task HandleGetReferencedTablesRequest_SessionNotFound()
        {
            // Arrange
            var eds = new EditDataService(null, null, null);
            var requestParams = new GetReferencedTablesParams { OwnerUri = "nonexistent://uri" };
            var contextMock = RequestContextMocks.Create<GetReferencedTablesResult>(null);
            contextMock.Setup(c => c.SendError(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            
            // Act
            await eds.HandleGetReferencedTablesRequest(requestParams, contextMock.Object);
            
            // Assert
            contextMock.Verify(c => c.SendError(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()));
        }
        
        [Test]
        public async Task HandleGetReferencedTablesRequest_NullOwnerUri()
        {
            // Arrange
            var eds = new EditDataService(null, null, null);
            var requestParams = new GetReferencedTablesParams { OwnerUri = null };
            var contextMock = RequestContextMocks.Create<GetReferencedTablesResult>(null);
            contextMock.Setup(c => c.SendError(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            
            // Act
            await eds.HandleGetReferencedTablesRequest(requestParams, contextMock.Object);
            
            // Assert
            contextMock.Verify(c => c.SendError(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()));
        }
        
        [Test]
        public async Task HandleGetReferencedTablesRequest_EmptyOwnerUri()
        {
            // Arrange
            var eds = new EditDataService(null, null, null);
            var requestParams = new GetReferencedTablesParams { OwnerUri = "" };
            var contextMock = RequestContextMocks.Create<GetReferencedTablesResult>(null);
            contextMock.Setup(c => c.SendError(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            
            // Act
            await eds.HandleGetReferencedTablesRequest(requestParams, contextMock.Object);
            
            // Assert
            contextMock.Verify(c => c.SendError(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()));
        }
        
        [Test]
        public async Task HandleGetReferencedTablesRequest_SessionNotInitialized()
        {
            // Arrange
            var metadataFactory = new Mock<IEditMetadataFactory>();
            var eds = new EditDataService(null, null, metadataFactory.Object);
            
            // Create a session but don't initialize it
            var session = new EditSession(metadataFactory.Object);
            eds.ActiveSessions[TestOwnerUri] = session;
            
            var requestParams = new GetReferencedTablesParams { OwnerUri = TestOwnerUri };
            var contextMock = RequestContextMocks.Create<GetReferencedTablesResult>(null);
            contextMock.Setup(c => c.SendError(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            
            // Act
            await eds.HandleGetReferencedTablesRequest(requestParams, contextMock.Object);
            
            // Assert
            contextMock.Verify(c => c.SendError(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()));
        }
        
        [Test]
        public async Task HandleGetReferencedTablesRequest_ComplexReferencedTables()
        {
            // Arrange
            var expectedReferencedTables = new[]
            {
                new Microsoft.SqlTools.ServiceLayer.EditData.ReferencedTableInfo
                {
                    SchemaName = "sales",
                    TableName = "OrderDetails",
                    FullyQualifiedName = "sales.OrderDetails",
                    ForeignKeyName = "FK_Complex_Composite",
                    SourceColumns = new[] { "OrderId", "ProductId", "CustomerId" },
                    ReferencedColumns = new[] { "Id", "ProdId", "CustId" }
                },
                new Microsoft.SqlTools.ServiceLayer.EditData.ReferencedTableInfo
                {
                    SchemaName = "inventory",
                    TableName = "Warehouses",
                    FullyQualifiedName = "inventory.Warehouses",
                    ForeignKeyName = "FK_Single_Warehouse",
                    SourceColumns = new[] { "WarehouseId" },
                    ReferencedColumns = new[] { "Id" }
                }
            };
            
            var eds = new EditDataService(null, null, null);
            var session = await CreateSessionWithReferencedTables(expectedReferencedTables);
            eds.ActiveSessions[TestOwnerUri] = session;
            
            var requestParams = new GetReferencedTablesParams { OwnerUri = TestOwnerUri };
            
            // Act & Assert
            var efv = new EventFlowValidator<GetReferencedTablesResult>()
                .AddResultValidation(result =>
                {
                    Assert.NotNull(result);
                    Assert.NotNull(result.ReferencedTables);
                    Assert.AreEqual(2, result.ReferencedTables.Length);
                    
                    // Verify composite foreign key
                    var compositeFK = result.ReferencedTables[0];
                    Assert.AreEqual("sales", compositeFK.SchemaName);
                    Assert.AreEqual("OrderDetails", compositeFK.TableName);
                    Assert.AreEqual(3, compositeFK.SourceColumns.Length);
                    Assert.AreEqual(3, compositeFK.ReferencedColumns.Length);
                    Assert.Contains("OrderId", compositeFK.SourceColumns);
                    Assert.Contains("ProductId", compositeFK.SourceColumns);
                    Assert.Contains("CustomerId", compositeFK.SourceColumns);
                    
                    // Verify single column foreign key
                    var singleFK = result.ReferencedTables[1];
                    Assert.AreEqual("inventory", singleFK.SchemaName);
                    Assert.AreEqual("Warehouses", singleFK.TableName);
                    Assert.AreEqual(1, singleFK.SourceColumns.Length);
                    Assert.AreEqual("WarehouseId", singleFK.SourceColumns[0]);
                })
                .Complete();
                
            await eds.HandleGetReferencedTablesRequest(requestParams, efv.Object);
            efv.Validate();
        }
        
        private async Task<EditSession> CreateSessionWithReferencedTables(Microsoft.SqlTools.ServiceLayer.EditData.ReferencedTableInfo[] referencedTables)
        {
            // Create a session with a proper query and metadata
            Query q = QueryExecution.Common.GetBasicExecutedQuery();
            ResultSet rs = q.Batches[0].ResultSets[0];
            EditTableMetadata etm = Common.GetCustomEditTableMetadata(rs.Columns.Cast<DbColumn>().ToArray());
            etm.ReferencedTables = referencedTables;
            EditSession s = await Common.GetCustomSession(q, etm);
            return s;
        }
    }
}