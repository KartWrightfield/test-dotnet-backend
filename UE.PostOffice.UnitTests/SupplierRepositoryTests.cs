using Moq;
using UE.PostOffice.Core.Entities;
using UE.PostOffice.Core.Interfaces.Data;
using UE.PostOffice.Data.Repositories;
using Xunit;

namespace UE.PostOffice.UnitTests;

public class SupplierRepositoryTests
{
    private readonly Mock<IDbContext> _mockDbContext;
    private readonly SupplierRepository _repository;

    public SupplierRepositoryTests()
    {
        _mockDbContext = new Mock<IDbContext>();
        _repository = new SupplierRepository(_mockDbContext.Object);
    }

    [Fact]
    public async Task GetMaxLeadTimeForProducts_SingleProduct_ReturnsCorrectLeadTime()
    {
        // Arrange
        const int productId = 1;
        const int expectedLeadTime = 3;
        SetupMockData(new Dictionary<int, int> { { productId, expectedLeadTime } });

        // Act
        var result = await _repository.GetMaxLeadTimeForProducts([productId]);

        // Assert
        Assert.Equal(expectedLeadTime, result);
    }

    [Fact]
    public async Task GetMaxLeadTimeForProducts_MultipleProducts_ReturnsMaxLeadTime()
    {
        // Arrange
        var productsAndLeadTimes = new Dictionary<int, int>
        {
            { 1, 2 },
            { 2, 5 },
            { 3, 1 }
        };
        SetupMockData(productsAndLeadTimes);

        // Act
        var result = await _repository.GetMaxLeadTimeForProducts([1, 2, 3]);

        // Assert
        Assert.Equal(5, result);
    }

    [Fact]
    public async Task GetMaxLeadTimeForProducts_EmptyList_ReturnsZero()
    {
        // Arrange
        SetupMockData(new Dictionary<int, int>());

        // Act
        var result = await _repository.GetMaxLeadTimeForProducts([]);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task GetMaxLeadTimeForProducts_ProductsWithSameLeadTime_ReturnsCorrectLeadTime()
    {
        // Arrange
        var productsAndLeadTimes = new Dictionary<int, int>
        {
            { 1, 3 },
            { 2, 3 }
        };
        SetupMockData(productsAndLeadTimes);

        // Act
        var result = await _repository.GetMaxLeadTimeForProducts([1, 2]);

        // Assert
        Assert.Equal(3, result);
    }

    private void SetupMockData(Dictionary<int, int> productIdToLeadTime)
    {
        var products = productIdToLeadTime.Select((kvp, index) => new Product
        {
            ProductId = kvp.Key,
            SupplierId = index + 1 // Each product gets a unique supplier for simplicity
        }).AsQueryable();

        var suppliers = productIdToLeadTime.Select((kvp, index) => new Supplier
        {
            SupplierId = index + 1,
            LeadTime = kvp.Value
        }).AsQueryable();
        
        var mockProducts = new Mock<IQueryable<Product>>();
        mockProducts.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(products.Provider);
        mockProducts.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.Expression);
        mockProducts.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.ElementType);
        mockProducts.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.GetEnumerator);
        
        var mockSuppliers = new Mock<IQueryable<Supplier>>();
        mockSuppliers.As<IQueryable<Supplier>>().Setup(m => m.Provider).Returns(suppliers.Provider);
        mockSuppliers.As<IQueryable<Supplier>>().Setup(m => m.Expression).Returns(suppliers.Expression);
        mockSuppliers.As<IQueryable<Supplier>>().Setup(m => m.ElementType).Returns(suppliers.ElementType);
        mockSuppliers.As<IQueryable<Supplier>>().Setup(m => m.GetEnumerator()).Returns(suppliers.GetEnumerator);

        _mockDbContext.Setup(db => db.Products).Returns(mockProducts.Object);
        _mockDbContext.Setup(db => db.Suppliers).Returns(mockSuppliers.Object);
    }
}